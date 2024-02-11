// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Plana.Composition.Abstractions;
using Plana.Testing.Logging;
using Plana.Workspace;
using Plana.Workspace.Abstractions;

namespace Plana.Testing;

public class PlanaContainer<T> where T : IPlanaPlugin, new()
{
    private readonly Dictionary<string, object> _dict;

    public PlanaContainer(params string[] args)
    {
        _dict = new Dictionary<string, object>();

        foreach (var arg in args)
            _dict.Add(arg, true);
    }

    public PlanaContainer(Dictionary<string, object> dict)
    {
        _dict = dict;
    }

    [MemberNotNull(nameof(Workspace), nameof(Sources))]
    public async Task RunAsync(string solution = "../../../../Plana.sln")
    {
        var logger = new Logger();

        Workspace = solution.EndsWith(".sln") ? new SolutionWorkspace(new FileInfo(solution), logger) : new ProjectWorkspace(new FileInfo(solution), logger);

        var instance = new T();
        instance.BindParameters(new TestParameterBinder(_dict));

        var obfuscator = new Obfuscator(Workspace, [instance], logger);
        Sources = await obfuscator.ObfuscateAsync(new CancellationToken());
    }

    public IWorkspace? Workspace { get; private set; }

    public IReadOnlyDictionary<string, string>? Sources { get; private set; }

}