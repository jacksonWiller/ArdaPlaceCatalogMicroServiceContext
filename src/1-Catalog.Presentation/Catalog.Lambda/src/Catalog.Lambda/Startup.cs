namespace Catalog.Lambda;

using System.Configuration;
using System.Reflection;
using Catalog.Application;
using Catalog.Core;
using Catalog.Core.AppSettings;
using Catalog.Core.Extensions;
using Catalog.Core.SharedKernel;
using Catalog.Domain.DataContext;
using Catalog.Infrastructure;
using Catalog.Infrastructure.Data;
using Catalog.Infrastructure.Data.Context;
using Catalog.PublicApi.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

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
        services.AddControllers();

        // Adicionar infraestrutura
        services.AddScoped<ICatalogDbContext, CatalogDbContext>()
        .AddScoped<IUnitOfWork, UnitOfWork>();

        var options = Configuration.GetOptions<ConnectionOptions>();

        services.AddDbContext<CatalogDbContext>(options =>
        {
            options.UseNpgsql("Host=fullendpoint.cluster-custom-cbecewa044ei.us-east-1.rds.amazonaws.com;Port=5432;Database=CatalogDB;Username=curso_aws;Password=postgres91162822");
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