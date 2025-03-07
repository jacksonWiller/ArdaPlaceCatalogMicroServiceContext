namespace Catalog.Lambda;
using Catalog.Api.Hosted.Controllers.V1;
using Catalog.Application;
using Catalog.Core.AppSettings;
using Catalog.Core.Extensions;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Infrastructure.PostgreSql.Data;
using Catalog.Infrastructure.PostgreSql.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers()
            .AddApplicationPart(assembly: typeof(ProductsController).Assembly);

        // Adicionar infraestrutura
        services.AddScoped<ICatalogDbContext, CatalogDbContext>()
        .AddScoped<IUnitOfWork, UnitOfWork>();

        var options = Configuration.GetOptions<ConnectionOptions>();

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql("Host=localhost;Port=5432;Database=CatalogContext;Username=postgres;Password=postgres");
        });

        services.AddCommandHandlers();

        // Register IHttpClientFactory
        services.AddHttpClient();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Welcome to running ASP.NET Core on AWS Lambda");
            });
        });
    }
}