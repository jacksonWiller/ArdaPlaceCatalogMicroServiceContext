#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Catalog.PublicApi/Catalog.PublicApi.csproj", "src/Catalog.PublicApi/"]
COPY ["src/Catalog.Application/Catalog.Application.csproj", "src/Catalog.Application/"]
COPY ["src/Catalog.Core/Catalog.Core.csproj", "src/Catalog.Core/"]
COPY ["src/Catalog.Domain/Catalog.Domain.csproj", "src/Catalog.Domain/"]
COPY ["src/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "src/Catalog.Infrastructure/"]
RUN dotnet restore "./src/Catalog.PublicApi/Catalog.PublicApi.csproj"
COPY . .
WORKDIR "/src/src/Catalog.PublicApi"
RUN dotnet build "./Catalog.PublicApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Catalog.PublicApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Catalog.PublicApi.dll"]