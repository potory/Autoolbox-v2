using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Autoolbox.App.Infrastructure.Installers;

public static class ServicesInstaller
{
    public static IServiceCollection AddConfiguration(this IServiceCollection collection)
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(path: Contracts.ConfigPath, optional: false);

        IConfiguration config = builder.Build();
        collection.AddSingleton(config);

        return collection;
    }

    public static IServiceCollection AddSonScriptDependencies(this IServiceCollection collection)
    {
        return collection;
    }
}