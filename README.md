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

# **Arquitetura Limpa (Clean Architecture)** 🏛️

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

# **Domain-Driven Design (DDD)** 🎯

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

***

# 🚀 CQRS no Projeto Catalog: Implementação com MediatR

Opa, dev! 😎 Já ouviu falar do CQRS (Command Query Responsibility Segregation)?
Se não, relaxa que a gente te explica de um jeito simples e direto! Se sim, vem ver como aplicamos isso no projeto **Catalog** usando **MediatR**! 🏗️

## 🔥 O que é CQRS?

CQRS é aquele padrão arquitetural maroto que separa operações de **leitura** (Queries) das de **escrita** (Commands). Ou seja, nada de misturar tudo num CRUDzão da vida! 😅

No nosso projeto, a implementação é parcial (usamos um único banco de dados), mas já garante:

✅ Separação clara entre **Commands** e **Queries**
✅ Código mais organizado e modular
✅ Facilidade para escalar e evoluir para Event Sourcing no futuro

## 🏗️ Como implementamos?

A estrutura básica da nossa implementação segue três componentes:

1️⃣ **Commands & Queries** – Definem o que queremos modificar ou buscar 📝
2️⃣ **Handlers** – Processam os commands e queries 🏗️
3️⃣ **MediatR** – O chefão que conecta tudo 💬

---

## 🛠️ Configurando o MediatR

Primeiro, registramos o MediatR e nossos handlers no `ConfigureServices`:

```csharp
public static IServiceCollection AddCommandHandlers(this IServiceCollection services)
{
    var assembly = Assembly.GetExecutingAssembly();
    return services
        .AddValidatorsFromAssembly(assembly)
        .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
        .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
}
```

Isso aqui:
- Registra automaticamente os handlers 🎯
- Adiciona validações com FluentValidation ✅
- Insere um behavior de logging pra monitorar os comandos 👀

---

## ⚡ Criando Commands

Os **Commands** representam **intenção de mudança** no sistema. Exemplo: criar uma categoria.

```csharp
public class CreateCategoryCommand : IRequest<Result<CreateCategoryResponse>>
{
    public string Name { get; set; }
    public string Description { get; set; }
}
```

Agora, o **handler** desse command, que vai processar a solicitação:

```csharp
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(ICatalogDbContext context, IValidator<CreateCategoryCommand> validator, IUnitOfWork unitOfWork)
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

        var category = new Category(request.Name, request.Description);
        _context.Set<Category>().Add(category);
        await _unitOfWork.SaveChangesAsync();

        return Result<CreateCategoryResponse>.Success(new CreateCategoryResponse(category.Id), "Categoria criada com sucesso!");
    }
}
```

Aqui:
✅ Validamos os dados com FluentValidation 🔎
✅ Criamos a entidade **Category** 📦
✅ Salvamos as mudanças com **UnitOfWork** 💾
✅ Retornamos um resultado encapsulado 🎯

---

## 🔍 Criando Queries

Enquanto Commands modificam o sistema, **Queries** são responsáveis por buscar dados.

```csharp
public class GetCategoryByIdQuery(Guid id) : IRequest<Result<GetCategoryByIdQueryResponse>>
{
    public Guid Id { get; set; } = id;
}
```

E o handler:

```csharp
public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, Result<GetCategoryByIdQueryResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<GetCategoryByIdQuery> _validator;

    public GetCategoryByIdQueryHandler(ICatalogDbContext context, IValidator<GetCategoryByIdQuery> validator)
    {
        _context = context;
        _validator = validator;
    }

    public async Task<Result<GetCategoryByIdQueryResponse>> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<GetCategoryByIdQueryResponse>.Invalid(validationResult.AsErrors());
        }

        var category = await _context.Set<Category>().Where(p => p.Id == request.Id).SingleOrDefaultAsync(cancellationToken);
        if (category == null)
            return Result.NotFound($"Categoria não encontrada: {request.Id}");

        return Result<GetCategoryByIdQueryResponse>.Success(new GetCategoryByIdQueryResponse { Name = category.Name, Description = category.Description }, "Categoria encontrada!");
    }
}
```

Aqui:
✅ Validamos a **query** antes de executá-la 🔍
✅ Buscamos os dados no **banco de dados** 💾
✅ Retornamos um DTO formatado corretamente 🎯

---

## 📌 Adicionando um Logging Behavior

Queremos logar tudo o que acontece? Criamos um **behavior** no MediatR!

```csharp
public class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var commandName = request.GetGenericTypeName();
        logger.LogInformation("🔵 Executando comando '{CommandName}'", commandName);
        var response = await next();
        logger.LogInformation("🟢 Comando '{CommandName}' finalizado", commandName);
        return response;
    }
}
```

Agora qualquer **Command** ou **Query** será logado automaticamente! 📜😎

---

## 🎯 Conclusão

CQRS com MediatR torna nossa arquitetura mais **organizada**, **testável** e **escalável**! 🏆

✅ **Commands e Queries separados** 📌
✅ **Handlers desacoplados** 🏗️
✅ **Behaviors para logging e validações** 🔍
✅ **Pronto para Event Sourcing no futuro** 🚀

***

# SOLID no Projeto Catalog: Princípios Aplicados na Prática 🚀💡

Os princípios SOLID são a base para um design de software sustentável, escalável e fácil de manter. Neste artigo, exploraremos como cada princípio SOLID é aplicado no projeto **Catalog**, com exemplos reais de código para ilustrar sua implementação. Vamos lá! 🎯✨

---

## 🟢 S - Princípio da Responsabilidade Única (Single Responsibility Principle)

Uma classe deve ter **apenas uma razão para mudar**, ou seja, deve ter uma única responsabilidade.

### 📌 Exemplo no Projeto

No **Catalog**, o `CreateCategoryCommandHandler` exemplifica este princípio:

```csharp
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<CreateCategoryCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(
        ICatalogDbContext context,
        IValidator<CreateCategoryCommand> validator,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        // Validação do comando ✅
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result<CreateCategoryResponse>.Invalid(validationResult.AsErrors());
        }

        var category = new Category(request.Name, request.Description);
        _context.Set<Category>().Add(category);
        await _unitOfWork.SaveChangesAsync();

        var response = new CreateCategoryResponse(category.Id);
        return Result<CreateCategoryResponse>.Success(response, "Category created successfully.");
    }
}
```

🔹 O handler tem **apenas uma responsabilidade**: processar o comando de criação de categoria. 
🔹 A validação, criação e persistência são delegadas para classes específicas. 
🔹 Código mais limpo, modular e fácil de manter! ✅

---

## 🔵 O - Princípio Aberto/Fechado (Open/Closed Principle)

Entidades de software devem estar **abertas para extensão**, mas **fechadas para modificação**.

### 📌 Exemplo no Projeto

O sistema de **eventos de domínio** ilustra bem esse princípio:

```csharp
public abstract class CategoryBaseEvent(Category category) : BaseEvent
{
    public Category Category { get; private init; } = category;
}

public class CategoryCreatedEvent(Category category) : CategoryBaseEvent(category) {}
public class CategoryUpdatedEvent(Category category) : CategoryBaseEvent(category) {}
```

🔹 **BaseEvent** está **fechado para modificação**, mas **aberto para extensão** por meio de novas subclasses. 
🔹 Podemos adicionar novos eventos sem modificar código existente! 🔥

Outro exemplo é o sistema de behaviors do **MediatR**, onde podemos adicionar novos behaviors sem alterar o código existente:

```csharp
services
    .AddValidatorsFromAssembly(assembly)
    .AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly))
    .AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

---

## 🟠 L - Princípio da Substituição de Liskov (Liskov Substitution Principle)

Objetos de uma classe derivada devem poder substituir objetos da classe base **sem quebrar o sistema**. 🔄

### 📌 Exemplo no Projeto

A estrutura de **entidades base** respeita esse princípio:

```csharp
public abstract class BaseEntity : IEntity<Guid>
{
    protected BaseEntity() => Id = Guid.NewGuid();
    protected BaseEntity(Guid id) => Id = id;

    public Guid Id { get; private init; }
}

public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Product> Products { get; private set; } = [];
}
```

🔹 `Category` **herda** de `BaseEntity` mantendo compatibilidade.
🔹 Qualquer código que utilize `BaseEntity` funcionará corretamente com `Category`. 🎯

---

## 🟣 I - Princípio da Segregação de Interface (Interface Segregation Principle)

Interfaces devem ser **pequenas e específicas**, evitando que clientes dependam de métodos que não usam. 🚀

### 📌 Exemplo no Projeto

A interface `IAggregateRoot` demonstra esse princípio:

```csharp
public interface IAggregateRoot;
```

🔹 **Interface de marcação**, sem métodos desnecessários.
🔹 Evita impor responsabilidades extras para classes que a implementam. ✅

Outro exemplo: separação da interface do contexto de dados:

```csharp
public interface ICatalogDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
```

🔹 Apenas os métodos **necessários** são expostos! 🛠️

---

## 🔴 D - Princípio da Inversão de Dependência (Dependency Inversion Principle)

Módulos de alto nível **não devem depender** de módulos de baixo nível. Ambos devem depender de **abstrações**. 🏗️

### 📌 Exemplo no Projeto

```csharp
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly HttpClient _httpClient;

    public CreateProductCommandHandler(
        ICatalogDbContext context,
        IValidator<CreateProductCommand> validator,
        IUnitOfWork unitOfWork,
        IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _validator = validator;
        _unitOfWork = unitOfWork;
        _httpClient = httpClientFactory.CreateClient();
    }
}
```

🔹 O handler **não depende** de implementações concretas, apenas de abstrações! 🔥

Outro exemplo na configuração do **Startup.cs**:

```csharp
services.AddScoped<ICatalogDbContext, CatalogDbContext>()
        .AddScoped<IUnitOfWork, UnitOfWork>();
```

🔹 As implementações são resolvidas **em tempo de execução**, garantindo flexibilidade. 🏗️

---

## 🎯 Conclusão

Os princípios SOLID são aplicados **consistentemente** no projeto Catalog, garantindo:

✅ **Código mais limpo** e organizado.
✅ **Facilidade de manutenção** e evolução.
✅ **Alta coesão e baixo acoplamento**.

💡 **Resumo rápido dos princípios:**

1️⃣ **S** - Responsabilidade única: cada classe tem apenas uma função.
2️⃣ **O** - Aberto/Fechado: código pode ser estendido sem ser modificado.
3️⃣ **L** - Substituição de Liskov: heranças bem planejadas.
4️⃣ **I** - Segregação de Interface: interfaces específicas e bem definidas.
5️⃣ **D** - Inversão de Dependência: módulos dependem de abstrações.

🚀 **Com isso, o projeto se mantém escalável, sustentável e pronto para crescer!** 🚀

***

# **Clean Code (ou pelo menos tentei) ✨**

## 🚀 Clean Code: Princípios e Aplicações no Projeto Catalog

Clean Code (Código Limpo) refere-se a um conjunto de práticas para escrever código que seja **legível**, **manutenível** e **fácil de entender**. Popularizado por **Robert C. Martin (Uncle Bob)**, esses princípios ajudam desenvolvedores a criar software de alta qualidade e de fácil manutenção. 

Analisaremos como os princípios de Clean Code são aplicados no projeto **Catalog** e identificaremos oportunidades de melhoria. 🔍

---

## 🏆 Princípios do Clean Code

### 🔤 1. Nomes Significativos

Escolher **bons nomes** é essencial para a clareza do código! Nada de abreviações obscuras ou nomes genéricos! ❌

#### ✅ Bom Exemplo:
```csharp
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Product> Products { get; private set; } = [];
    public bool IsDeleted { get; private set; } = false;

    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        AddDomainEvent(new CategoryUpdatedEvent(this));
    }
}
```
✅ **Clareza total nos nomes!** Nada de `a`, `b`, `data` ou `xpto`! 🚀

#### 🔍 Oportunidade de Melhoria:
```csharp
public Category(string nome, string descricao)
{
    Name = nome;
    Description = descricao;
    AddDomainEvent(new CategoryCreatedEvent(this));
}
```
🔴 **Inconsistência detectada!** ⚠️ Mistura de idiomas nos nomes! O ideal é manter tudo em **inglês**:
```csharp
public Category(string name, string description)
{
    Name = name;
    Description = description;
    AddDomainEvent(new CategoryCreatedEvent(this));
}
```
✅ **Agora sim!** Consistência é fundamental! 😃

---

### 🔧 2. Funções Pequenas e Focadas

Cada função deve **fazer uma única coisa e fazer bem feito**! 🔥

#### ✅ Bom Exemplo:
```csharp
public void Delete()
{
    if (IsDeleted) return;

    IsDeleted = true;
    AddDomainEvent(new CategoryDeletedEvent(this));
}
```
✅ **Simples, direto ao ponto e sem enrolação!** 🎯

---

### 📂 3. Estrutura Bem Organizada (CQRS)

Separação clara entre **Comandos e Queries** para manter a organização! 🏗️

#### ✅ Exemplo de um Command Handler bem estruturado:
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

        var category = new Category(request.Name, request.Description);

        _context.Set<Category>().Add(category);
        await _unitOfWork.SaveChangesAsync();

        return Result<CreateCategoryResponse>.Success(new CreateCategoryResponse(category.Id), "Category created successfully.");
    }
}
```
✅ **Estrutura clara e bem definida!** Sem bagunça! 📌

---

### 📝 4. Comentários Apropriados

Comentários devem **explicar o porquê**, e não o **como**! ❌ Nada de comentários óbvios!

#### ✅ Bom Exemplo:
```csharp
builder
    .Property(eventStore => eventStore.Data)
    .IsRequired()
    .HasColumnType("text") // 🔥 Usando TEXT porque é ilimitado no PostgreSQL
    .HasComment("JSON serialized event");
```
✅ **Explicação útil e concisa!** 🎯

---

### 🔥 5. Tratamento de Erros

Erro **não pode passar batido**! 👀

#### 🔍 Oportunidade de Melhoria:
```csharp
try
{
    var rowsAffected = await _context.SaveChangesAsync();
    await transaction.CommitAsync();
}
catch (Exception ex)
{
    await transaction.RollbackAsync();
    throw;
}
```
🔴 **Sem logging detalhado!** Poderia ser melhorado assim:
```csharp
catch (Exception ex)
{
    _logger.LogError(ex, "Erro ao salvar mudanças no banco de dados");
    await transaction.RollbackAsync();
    throw;
}
```
✅ **Agora sim!** Logging ajuda na depuração! 🔍

---

### 🗑️ 6. Código Comentado = Código Morto

❌ **Se não usa, delete!**

```csharp
// var url = "https://localhost:44329/api/files/get-by-key?bucketName=jacksonlocal&key=catalog%2Fdownload.jpeg";
// var response = await _httpClient.GetAsync(url);
// response.EnsureSuccessStatusCode();
```
🔴 **Remova ou justifique!**

✅ **Código limpo = código sem lixo!** 🧹

---

## 📌 Conclusão

O projeto **Catalog** já segue muitas boas práticas de **Clean Code**, mas sempre há espaço para melhorias! 🔥

### 🚀 **Principais recomendações:**
✅ Padronizar nomes (**inglês sempre!**)
✅ Completar validadores
✅ Remover código morto
✅ Melhorar logs de erro
✅ Seguir convenções de nomenclatura

Clean Code **não é um destino, mas uma jornada**! ✈️ **Refatoração constante** é essencial para manter um código saudável! 💪





