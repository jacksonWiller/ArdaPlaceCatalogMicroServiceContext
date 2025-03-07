# Catalog!

Bem-vindo ao **Catalog**! 🎉 Esse projeto não é nada sério, nem revolucionário, nem o próximo unicórnio do mercado de tecnologia. Na verdade, ele é apenas um **hobby** para explorar conceitos legais de arquitetura de software e boas práticas. Se você está aqui esperando "o melhor design de software do mundo", sinto muito por isso. 😆

## O que tem aqui?

O **Catalog** é um repositório para testar e demonstrar padrões arquiteturais modernos de uma forma prática e sem pressão. O projeto segue conceitos como:

- **Arquitetura Limpa (Clean Architecture)** 🏛️
- **Domain-Driven Design (DDD)** 🎯
- **Event-Driven Design (Comunicação baseada em eventos para melhor desacoplamento)** 📢
- **CQRS (Incompleto, usando apenas um banco para leitura e escrita)** 🔄
- **SOLID (Sim, aquele SOLID de sempre)** 🛠️
- **Clean Code (ou pelo menos tentei)** ✨

## Mas por que isso tudo?

Porque estudar é divertido! 📖 Esse repositório serve como um playground para experimentar ideias e ver como tudo se encaixa (ou não). Não há garantias de que esse é o jeito mais perfeito e otimizado de se fazer as coisas, mas com certeza há muito aprendizado envolvido.

***

# 💡 Clean Architecture no Projeto Catalog: Vamos Falar Sobre a Estrutura!

A **Clean Architecture** é aquela abordagem que deixa o seu sistema limpo, organizado e bem fácil de testar. Ela separa tudo em camadas, cada uma com uma responsabilidade única, e faz o sistema funcionar de forma super independente. No projeto **Catalog**, aplicamos essa arquitetura com muito amor e carinho, seguindo os princípios mais importantes, como inversão de dependência e separação de responsabilidades.

## 🧩 Como Funciona a Estrutura de Camadas?

No projeto **Catalog**, temos quatro camadas principais que fazem toda a mágica acontecer:

### 1. 🔑 Camada de Domínio (Onde a Magia Acontece!)

A camada de **domínio** (`3-Catalog.Domain`) é o coração do sistema. Ela contém as **entidades de negócio** e as **regras de negócio** (sim, essas são as coisas mais importantes), e é completamente independente das outras camadas.

Aqui, as **entidades de negócio** representam os conceitos principais do sistema. Imagine que a nossa entidade **Category** (categoria) é o centro de tudo no mundo **Catalog**. Ela tem uma estrutura assim:

```csharp
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Product> Products { get; private set; } = [];

    public bool _isDeleted { get; private set; } = false;

    public Category() {}

    public Category(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryCreatedEvent(this));
    }

    public void Update(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }
    
    // Mais métodos de domínio...
}
```

Aqui, definimos até as **interfaces** que serão implementadas nas camadas externas, como a interface `ICatalogDbContext` que cuida de tudo com o banco de dados:

```csharp
public interface ICatalogDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    int SaveChanges();
    ChangeTracker ChangeTracker { get; }
    DatabaseFacade Database { get; }
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    // Outros membros...
}
```

### 2. 🏃‍♂️ Camada de Aplicação (Os Bastidores)

A camada de **aplicação** (`2-Catalog.Application`) é onde a **mágica** realmente acontece, mas sem adicionar regras de negócio, ok? Aqui, tudo é orquestrado e as operações entre o **domínio** e a **infraestrutura** são coordenadas. Em vez de fazer as coisas acontecerem diretamente, ela organiza tudo como um maestro!

```xml
<ProjectReference Include="..\..\3-Catalog.Domain\Catalog.Domain\Catalog.Domain.csproj" />
<ProjectReference Include="..\..\Catalog.Core\Catalog.Core.csproj" />
```

E aqui está um exemplo de como a **injeção de dependência** funciona. O `CreateCategoryCommandHandler` só precisa se preocupar em chamar o que foi definido no domínio, sem saber os detalhes de implementação:

```csharp
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICatalogDbContext context,
        IValidator<CreateCategoryCommand> validator,
        IUnitOfWork unitOfWork
    )
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateCategoryResponse>.Invalid(validationResult.AsErrors());
        }

        var product = new Category(request.Name, request.Description);
        _context.Set<Category>().Add(product);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateCategoryResponse(product.Id);
        return Result<CreateCategoryResponse>.Success(response, "Product created successfully.");
    }
}
```

### 3. ⚙️ Camada de Infraestrutura (A Parte Técnica)

A camada de **infraestrutura** (`4-Catalog.Infrastructure`) cuida de todos os detalhes técnicos, como persistência de dados, mapeamento ORM e integração com o banco. Se fosse um show, a **infraestrutura** seria os bastidores, garantindo que tudo aconteça sem erros.

```csharp
public class CatalogDbContext(DbContextOptions<CatalogDbContext> dbOptions) : BaseDbContext<CatalogDbContext>(dbOptions), ICatalogDbContext
{
    public DbSet<EventStore> EventStores { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new EventStoreConfiguration());
    }
}
```

### 4. 🌍 Camada de Apresentação (Onde Tudo Começa)

Por fim, temos a camada de **apresentação** (`1-Catalog.Presentation`), que é onde as coisas começam a acontecer com o usuário. Ela recebe as requisições HTTP e as envia para a camada de aplicação. Aqui, fazemos toda a conexão entre as camadas!

```csharp
services.AddScoped<ICatalogDbContext, CatalogDbContext>()
.AddScoped<IUnitOfWork, UnitOfWork>();

services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql("Host=localhost;Port=5432;Database=CatalogContext;Username=postgres;Password=postgres");
});
```

## 🔄 Como os Dados Fluem?

O fluxo de dados no sistema funciona assim:

1. A camada de apresentação recebe uma requisição HTTP.
2. Um **command/query** é criado e enviado para um **handler** na camada de aplicação.
3. O **handler** utiliza as interfaces definidas no domínio, implementadas pela infraestrutura.
4. As operações acontecem nas **entidades de domínio**.
5. As mudanças são persistidas pela infraestrutura.

Tudo isso acontece por causa da **inversão de dependência**, que garante que as camadas de **domínio** e **aplicação** dependem apenas das interfaces, e a **infraestrutura** implementa essas interfaces.

## 🎉 O Que Ganhamos Com Isso?

1. **Testabilidade**: Fácil de substituir implementações reais por mocks nos testes.
2. **Manutenibilidade**: Alterações em uma camada não afetam as outras.
3. **Flexibilidade**: Dá para trocar as implementações da infraestrutura sem mexer no código da lógica de negócios.
4. **Separação de Responsabilidades**: Cada camada tem seu papel bem claro, sem confusão.

Essa implementação da **Clean Architecture** no **Catalog** mostra como essa abordagem pode criar um sistema robusto, flexível e super fácil de manter. Prontinho para evoluir sem dor de cabeça!

***

# 🚀 DDD no Projeto Catalog: Entidades, Value Objects e Agregados

O **Domain-Driven Design (DDD)** tem sido um dos principais guias na modelagem do **Catalog**, garantindo um código mais organizado, expressivo e alinhado com o domínio do negócio. A aplicação do DDD reforça a separação de responsabilidades e melhora a testabilidade, proporcionando uma arquitetura sustentável.

A estrutura do projeto foi construída sobre três pilares fundamentais:

- **Entidades** 🆔  
- **Value Objects** 🎭  
- **Agregados** 📦  

Embora **Contextos Delimitados** sejam um aspecto essencial do DDD, a abordagem adotada até o momento focou nos elementos centrais que estruturam o domínio. Esse será um tema a ser abordado futuramente, dado seu impacto na segmentação e organização dos modelos.

## 📌 Blocos Fundamentais do DDD

A modelagem do **Catalog** foi organizada da seguinte forma:

- **Entidades** → Possuem identidade única e persistem ao longo do tempo, mesmo com mudanças de atributos.
- **Value Objects** → Não possuem identidade própria e são definidos apenas pelos valores que carregam.
- **Agregados** → Representam um grupo coeso de entidades e value objects, garantindo consistência nas alterações.

Essa estrutura permite um modelo de domínio mais claro, onde os objetos possuem regras bem definidas e mantêm a integridade dos dados.

## 🆔 Entidades no Domínio

As **entidades** do projeto seguem um padrão de encapsulamento que protege seus estados internos. No **Catalog**, todas as entidades herdam de `BaseEntity`, garantindo um identificador único e suporte a eventos de domínio:

```csharp
public abstract class BaseEntity : IEntity<Guid>
{
    private readonly List<BaseEvent> _domainEvents = [];

    protected BaseEntity() => Id = Guid.NewGuid();
    protected BaseEntity(Guid id) => Id = id;

    public IEnumerable<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();
    public Guid Id { get; private init; }
}
```

A **Category** representa um exemplo de entidade que encapsula regras de negócio e mantém uma lista de produtos:

```csharp
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Product> Products { get; private set; } = [];
    private bool _isDeleted = false;

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        // Evento de domínio...
    }

    public void Delete()
    {
        if (_isDeleted) return;
        _isDeleted = true;
        // Evento de domínio...
    }
}
```

## 🎭 Value Objects no Modelo

Os **Value Objects** são imutáveis e não possuem identidade própria. No projeto, a classe `Image` ilustra bem esse conceito:

```csharp
public class Image
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; }
    public string Prefix { get; private set; }
    public string Url { get; private set; }

    public Image(string prefix, string name)
    {
        Prefix = prefix;
        Name = name;
        Url = $"{prefix}/{name}";
    }
}
```

A persistência desse value object no banco de dados é realizada via **EF Core**, garantindo sua associação com a entidade pai:

```csharp
builder.OwnsMany(product => product.Images, p =>
{
    p.WithOwner().HasForeignKey("ProductId");
    p.Property<Guid>("Id");
    p.HasKey("Id");
    p.Property(image => image.Name).IsRequired().HasMaxLength(255);
    p.Property(image => image.Url).IsRequired().HasMaxLength(255);
    p.ToTable("ProductImages");
});
```

## 📦 Agregados e Consistência

Os **agregados** organizam o domínio em torno de um ponto central de modificação de dados. No **Catalog**, `Product` age como um **agregado raiz**, garantindo que alterações sejam controladas de forma consistente:

```csharp
public class Product : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; }
    public decimal Price { get; private set; }
    public List<Category> Categories { get; private set; } = [];
    public List<Image> Images { get; private set; } = [];

    public void AddImage(List<Image> images)
    {
        Images = images;
        // Evento de domínio...
    }

    public void AddCategory(Category category)
    {
        Categories.Add(category);
        // Evento de domínio...
    }
}
```

A interface `IAggregateRoot` sinaliza que `Product` é um agregado raiz, permitindo que repositórios interajam com ele como uma única unidade:

```csharp
public interface IAggregateRoot;
```

## 🎯 Benefícios da Aplicação do DDD no Catalog

- 💎 **Modelo Rico e Expressivo** → Código que reflete fielmente o domínio do negócio.  
- 🛡 **Encapsulamento** → Protege regras de negócio dentro das entidades.  
- 🔄 **Consistência** → Os agregados garantem integridade nos dados.  
- 🗣 **Linguagem Ubíqua** → Facilita a comunicação entre desenvolvedores e especialistas do domínio.  
- 🧪 **Testabilidade** → Código altamente testável sem dependências de infraestrutura.  

## 🎬 Considerações Finais

A aplicação do **DDD** no **Catalog** estruturou um modelo de software sólido e alinhado com as necessidades do negócio. A segmentação entre **Entidades, Value Objects e Agregados** proporcionou um código sustentável e modular.

O próximo passo será aprofundar os **Contextos Delimitados**, um conceito essencial para dividir o domínio em partes coesas e evitar acoplamento excessivo. Esse será o tema de uma discussão futura. 🚀

***

# 📌 Event Sourcing: O Que é e Como Ajuda no Desacoplamento de Serviços

Se você já precisou auditar uma mudança no sistema ou quis reverter algo sem saber exatamente o que aconteceu, provavelmente sentiu falta de um bom Event Sourcing. Mas relaxa, porque hoje vamos descomplicar essa parada! 😎

---

## 🎯 O que é Event Sourcing?

Ao invés de simplesmente armazenar o **estado atual** de uma entidade (como um produto ou uma categoria), o Event Sourcing guarda **todas as mudanças** que aconteceram com ela ao longo do tempo. Ou seja, em vez de sobrescrever os dados, cada alteração vira um **evento imutável**! 🔄

### 🛠️ Benefícios do Event Sourcing
✅ **Reconstrução de estado** a qualquer momento no tempo.  
✅ **Auditoria completa** das mudanças, sem perda de informações.  
✅ **Depuração e debugging** mais fáceis, já que você tem um histórico detalhado.  
✅ **Time-travel!** ⏳ Volte no tempo e veja como os dados evoluíram.  

---

## 🔌 Como o Event Sourcing Desacopla Serviços?

Aqui entra a parte mais interessante! Com Event Sourcing, podemos notificar outros serviços sobre mudanças **sem criar dependências diretas entre eles**. Isso significa:

🔹 **Menos acoplamento**: Um serviço pode emitir um evento, e vários outros podem reagir a ele **sem precisarem se conhecer**.  
🔹 **Resiliência**: Se um serviço estiver fora do ar, ele pode processar os eventos quando voltar. 🔄  
🔹 **Escalabilidade**: Ao invés de sobrecarregar um sistema central, cada serviço pode processar eventos separadamente. 🚀  

---

## 🏗️ Implementação no Projeto Catalog

No projeto, segui essa abordagem para deixar tudo mais organizado e flexível. Começamos criando uma estrutura base para eventos:

```csharp
public abstract class BaseEvent : INotification
{
    public string MessageType { get; protected init; }
    public Guid AggregateId { get; protected init; }
    public DateTime OccurredOn { get; private init; } = DateTime.Now;
}
```

### 📝 Criando Eventos de Domínio
Cada entidade pode **gerar eventos** sempre que acontece uma mudança importante:

```csharp
public class Category : BaseEntity
{
    public Category(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryCreatedEvent(this));
    }

    public void Update(string nome, string descricao)
    {
        Name = nome;
        Description = descricao;
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }
}
```

Agora, quando uma **categoria for criada ou atualizada**, um evento será disparado! ⚡

---

## 🔄 Processamento dos Eventos com Unit of Work

O **Unit of Work** garante que os eventos sejam salvos e publicados corretamente:

```csharp
public async Task SaveChangesAsync()
{
    var (domainEvents, eventStores) = BeforeSaveChanges();
    await _context.SaveChangesAsync();
    await AfterSaveChangesAsync(domainEvents, eventStores);
}
```

Ele coleta os eventos antes de salvar, garante a persistência no banco e, depois, dispara os eventos para que outros serviços possam reagir. 📢

---

## 🎬 Conclusão

Adotar Event Sourcing é um verdadeiro **game-changer** quando falamos de escalabilidade e desacoplamento! 🏆

✅ **Serviços independentes** que não precisam se conhecer.  
✅ **Histórico completo de dados**, sem perda de informações.  
✅ **Maior resiliência** e **flexibilidade** para crescimento futuro.  

Se você quer um sistema preparado para escalar e evoluir sem dores de cabeça, essa abordagem é uma excelente escolha! 🚀🔥




