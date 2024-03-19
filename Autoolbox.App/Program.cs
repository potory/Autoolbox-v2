using Autoolbox.App.Infrastructure;
using Autoolbox.App.Infrastructure.Installers;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

var registrations = new ServiceCollection()
    .AddConfiguration()
    .AddAutomaticDependencies()
    .AddSonScriptDependencies();

var registrar = new TypeRegistrar(registrations);
var app = new CommandApp(registrar);

app.Configure(c => c.AddSonScriptCommands());
await app.RunAsync(args);