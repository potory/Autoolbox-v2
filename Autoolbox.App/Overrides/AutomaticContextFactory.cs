using Autoolbox.App.Exceptions;
using Microsoft.Extensions.Configuration;
using SonScript2.Core.Compilation.Abstraction;
using SonScript2.Core.Evaluation;
using SonScript2.Core.Evaluation.Abstraction;

namespace Autoolbox.App.Overrides;

public class AutomaticContextFactory : IContextFactory
{
    private readonly BaseContext _globalContext;
    private readonly IFileContext _fileContext;
    private readonly ILibrary _library;

    private readonly string _previousResultInjection;

    public AutomaticContextFactory(IConfiguration config, BaseContext globalContext, IFileContext fileContext, ILibrary library)
    {
        _globalContext = globalContext;
        _fileContext = fileContext;
        _library = library;

        _previousResultInjection = config[Contracts.Config.Injection.PreviousResult] ?? 
                                   throw new AutoolboxConfigurationException("...");
    }

    public BaseContext CreateScope(ICompiler? compiler)
    {
        return new AutomaticContext(_previousResultInjection,
            globalContext: _globalContext, 
            fileContext: _fileContext,
            library: _library,
            compiler: compiler);
    }
}