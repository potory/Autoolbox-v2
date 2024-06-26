﻿using Autoolbox.App.Wrappers;
using SonScript2.Core.Compilation.Abstraction;
using SonScript2.Core.Evaluation;
using SonScript2.Core.Evaluation.Abstraction;
using SonScript2.Core.Services;
using Spectre.Console;

namespace Autoolbox.App.Overrides;

public class AutomaticContext : BaseContext
{
    public AutomaticContext(string previousResult,
        BaseContext? globalContext = null, IFileContext? fileContext = null,
        ICompiler? compiler = null, ILibrary? library = null, IRandomService? randomService = null)
        : base(globalContext, fileContext, compiler, library, randomService)
    {
        InjectResult = $"\"{previousResult}\"";
    }

    public string InjectResult { get; }

    public RequestWrapper PreviousRequest { get; set; }
    public int Level { get; set; }
    public int Iteration { get; set; }

    public bool PrevBool(string path) => PreviousRequest.Value<bool>(path);
    public float PrevFloat(string path) => PreviousRequest.Value<float>(path);
    public int PrevInt(string path) => PreviousRequest.Value<int>(path);
    public string PrevString(string path) => PreviousRequest.Value<string>(path);

    public string Read(string message, string? def = null)
    {
        Console.Write(message);
        var input = Console.ReadLine();

        if (string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(def))
        {
            return def;
        }

        return input;
    }

    public void Log(object obj)
    {
        AnsiConsole.WriteLine(obj.ToString() ?? string.Empty);
    }
}