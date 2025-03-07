# Navegue até o diretório do projeto que contém o DbContext
cd src/1-Catalog.Presentation/Catalog.WebApi

# Reverter todas as migrações
dotnet ef database update 0 --project "../../4-Catalog.Infrastructure/Catalog.Infrastructure/Catalog.Infrastructure.csproj" --startup-project "Catalog.WebApi.csproj" --context Catalog.Infrastructure.Data.Context.CatalogDbContext --configuration Debug

# Reaplicar as migrações
dotnet ef database update --project "../../4-Catalog.Infrastructure/Catalog.Infrastructure/Catalog.Infrastructure.csproj" --startup-project "Catalog.WebApi.csproj" --context Catalog.Infrastructure.Data.Context.CatalogDbContext --configuration Debug



# Remova todas as migrações
dotnet ef migrations remove --project "../../4-Catalog.Infrastructure/Catalog.Infrastructure/Catalog.Infrastructure.csproj" --startup-project "Catalog.WebApi.csproj" --context Catalog.Infrastructure.Data.Context.CatalogDbContext --configuration Debug

# Adicione uma nova migração
dotnet ef migrations add Initial --project "../../4-Catalog.Infrastructure/Catalog.Infrastructure/Catalog.Infrastructure.csproj" --startup-project "Catalog.WebApi.csproj" --context Catalog.Infrastructure.Data.Context.CatalogDbContext --configuration Debug

# Atualize o banco de dados
dotnet ef database update --project "../../4-Catalog.Infrastructure/Catalog.Infrastructure/Catalog.Infrastructure.csproj" --startup-project "Catalog.WebApi.csproj" --context Catalog.Infrastructure.Data.Context.CatalogDbContext --configuration Debug
   