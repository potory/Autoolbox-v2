using Autoolbox.App.Configuration.Abstraction;
using Autoolbox.App.Configuration.Implementation;
using Autoolbox.App.Overrides;
using Autoolbox.App.Services;
using Autoolbox.App.Services.Abstraction;
using Autoolbox.App.Services.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SonScript2.Core.Compilation.Abstraction;
using SonScript2.Core.Compilation.Implementation;
using SonScript2.Core.Evaluation;
using SonScript2.Core.Evaluation.Abstraction;
using SonScript2.Core.Evaluation.Implementation;

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

    public static IServiceCollection AddAutomaticDependencies(this IServiceCollection collection)
    {
        collection.AddTransient<IConfigReader, ConfigReader>();
        collection.AddTransient<IRequestFactory, RequestFactory>();
        collection.AddTransient<IRequestStreamer, RequestStreamer>();
        collection.AddTransient<IRequestSender, RequestSender>();
        collection.AddTransient<ILogger, Logger>();
        collection.AddTransient<ISortRunner, SortRunner>();
        
        return collection;
    }

    public static IServiceCollection AddSonScriptDependencies(this IServiceCollection collection)
    {
        var globalContext = new BaseContext();

        collection.AddTransient<IDecomposer, DefaultDecomposer>();
        collection.AddTransient<INodeFactory, DefaultNodeFactory>();
        collection.AddTransient<IContextFactory, AutomaticContextFactory>();
        collection.AddTransient<ICompiler, DefaultCompiler>();
        collection.AddSingleton<ILibrary, DefaultLibrary>();
        collection.AddTransient<IFileContext, DefaultFileContext>();
        collection.AddSingleton(globalContext);

        return collection;
    }
}