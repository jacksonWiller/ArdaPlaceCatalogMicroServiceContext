# CatalogMicroServiceContext!

Bem-vindo ao **Catalog**! 🎉 Esse projeto não é nada sério, nem revolucionário, nem o próximo unicórnio do mercado de tecnologia. Na verdade, ele é apenas um **hobby** para explorar conceitos legais de arquitetura de software e boas práticas. Se você está aqui esperando "o melhor design de software do mundo", sinto muito por isso. 😆

## O que tem aqui?

O **CatalogMicroServiceContext** é um repositório para testar e demonstrar padrões arquiteturais modernos de uma forma prática e sem pressão. O projeto segue conceitos como:

- **Arquitetura Limpa (Clean Architecture)** 🏛️
- **Domain-Driven Design (DDD)** 🎯
- **Event-Driven Design (Comunicação baseada em eventos para melhor desacoplamento)** 📢
- **CQRS (Incompleto, usando apenas um banco para leitura e escrita)** 🔄
- **SOLID (Sim, aquele SOLID de sempre)** 🛠️
- **Clean Code (ou pelo menos tentei)** ✨

## Mas por que isso tudo?

Porque estudar é divertido! 📖 Esse repositório serve como um playground para experimentar ideias e ver como tudo se encaixa (ou não). Não há garantias de que esse é o jeito mais perfeito e otimizado de se fazer as coisas, mas com certeza há muito aprendizado envolvido.

***

# 🚀 Clean Architecture no Projeto Catalog: Um Playground de Arquitetura!

A **Clean Architecture** não é apenas um nome bonito, mas uma forma de estruturar o código de um jeito mais **organizado, testável e independente**. No **Catalog**, seguim essa seguinte abordagem porque... bom, porque achei legal! 😆

## 🏛️ Estrutura de Camadas

O projeto está dividido em **quatro camadas** principais, cada uma com seu papel bem definido:

### 🔹 1. Domínio (O Coração do Sistema)

Aqui mora a alma do projeto! 💙 A camada de domínio (`3-Catalog.Domain`) não sabe que o resto do mundo existe. Ela contém:

- **Entidades de negócio** (as estrelas do show)  
- **Interfaces** (contratos para o resto do sistema)  
- **Regras de negócio** (onde a mágica acontece ✨)  

Exemplo de uma entidade poderosa:

```csharp
public class Category : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Product> Products { get; private set; } = [];
    
    public Category(string name, string description)
    {
        Name = name;
        Description = description;
        AddDomainEvent(new CategoryCreatedEvent(this));
    }
}
```

Ela não sabe nada sobre banco de dados, frameworks ou APIs. E é assim que deve ser! 😎

---

### 🛠️ 2. Aplicação (Os Orquestradores)

A camada de aplicação (`2-Catalog.Application`) é como um maestro, organizando as interações entre domínio e infraestrutura.

✅ **Não contém regras de negócio** (essas ficam no domínio)  
✅ **Usa os contratos do domínio** para acessar dados  
✅ **Gerencia fluxos de operações**  

Exemplo de um Handler (executor de comandos):

```csharp
public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<CreateCategoryResponse>>
{
    private readonly ICatalogDbContext _context;

    public async Task<Result<CreateCategoryResponse>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name, request.Description);
        _context.Set<Category>().Add(category);
        await _context.SaveChangesAsync();
        return Result<CreateCategoryResponse>.Success(new CreateCategoryResponse(category.Id), "Category created!");
    }
}
```

Perceba que ele **não fala diretamente com o banco de dados**! Ele só pede para alguém fazer isso por ele. 🤝

---

### 💾 3. Infraestrutura (Os Bastidores)

A camada de infraestrutura (`4-Catalog.Infrastructure`) faz o trabalho sujo: **salvar no banco, enviar eventos, logar coisas, etc.** Ela implementa interfaces definidas pelo domínio.

```csharp
public class CatalogDbContext : DbContext, ICatalogDbContext
{
    public DbSet<Category> Categories { get; set; }
}
```

Aqui também vive o **Unit of Work**, garantindo que tudo seja salvo no momento certo.

```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ICatalogDbContext _context;
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
```

---

### 🌍 4. Camada de Apresentação (Onde Tudo Começa)

A camada de apresentação (`1-Catalog.Presentation`) é onde tudo começa! Ela recebe requisições e manda a aplicação resolver os problemas.

```csharp
services.AddScoped<ICatalogDbContext, CatalogDbContext>();
services.AddScoped<IUnitOfWork, UnitOfWork>();
```

---

## 🔄 Fluxo de Dependências

1️⃣ A API recebe uma requisição (exemplo: criar uma categoria).  
2️⃣ O handler da aplicação recebe o comando.  
3️⃣ Ele chama o domínio para criar a entidade.  
4️⃣ A infraestrutura salva tudo no banco.  
5️⃣ A API retorna um **"Deu certo!"** 🎉  

---

## 🎯 Benefícios Dessa Arquitetura

✅ **Testabilidade**: Cada parte pode ser testada isoladamente.  
✅ **Manutenibilidade**: Alterações em um lugar não quebram o sistema todo.  
✅ **Flexibilidade**: Podemos trocar bancos de dados, frameworks e APIs sem dor de cabeça.  
✅ **Separacão de Responsabilidades**: Cada camada faz o que foi feita para fazer.  

