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
```

***



