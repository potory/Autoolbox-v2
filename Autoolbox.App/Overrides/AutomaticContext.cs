using SonScript2.Core.Compilation.Abstraction;
using SonScript2.Core.Evaluation;
using SonScript2.Core.Evaluation.Abstraction;

namespace Autoolbox.App.Overrides;

public class AutomaticContext : BaseContext
{
    public AutomaticContext(string previousResult, 
        BaseContext? globalContext = null, IFileContext? fileContext = null, 
        ICompiler? compiler = null, ILibrary? library = null) : base(globalContext, fileContext, compiler, library)
    {
        PreviousResult = $"\"{previousResult}\"";
    }

    public string PreviousResult { get; }

    public int Level { get; set; }
    public int Iteration { get; set; }
}