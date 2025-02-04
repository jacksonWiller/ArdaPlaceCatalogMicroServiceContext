using System.IO;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Lambda;

public class Startup
{
    public static IServiceCollection BuildContainer()
    {
        var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddEnvironmentVariables()
          .Build();

        return ConfigureServices(configuration);
    }

    private static IServiceCollection ConfigureServices(IConfigurationRoot configurationRoot)
    {
        var services = new ServiceCollection();

        return services;
    }
}
