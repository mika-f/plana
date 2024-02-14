// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Enum;
using Plana.Testing.Logging;
using Plana.Workspace;
using Plana.Workspace.Abstractions;

namespace Plana.Testing;

public class PlanaContainer<T> where T : IPlanaPlugin, new()
{
    private readonly Dictionary<string, object> _dict;

    private string? _root;

    public IWorkspace? Workspace { get; private set; }

    public IReadOnlyDictionary<string, string>? Sources { get; private set; }

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

    [MemberNotNull(nameof(Workspace), nameof(Sources), nameof(_root))]
    public async Task RunAsync(string path = "../../../../Plana.sln", int seed = 150)
    {
        var logger = new Logger();
        var source = new CancellationTokenSource();

        Workspace = path.EndsWith(".sln") ? new SolutionWorkspace(new FileInfo(path), logger) : new ProjectWorkspace(new FileInfo(path), logger);
        _root = Path.GetDirectoryName(Path.GetFullPath(path))!;

        var instance = new T();
        instance.BindParameters(new TestParameterBinder(_dict));

        await Workspace.ActivateWorkspaceAsync(source.Token);

        var projects = await Workspace.GetProjectsAsync(source.Token);
        var solution = new PlanaSolution(projects);

        var obfuscator = new Obfuscator(Workspace, [instance], logger);
        var context = new TestPlanaContext
        {
            Solution = solution,
            Kind = RunKind.Obfuscate,
            Random = new PlanaRandom(seed),
            SecureRandom = new PlanaRandom(seed),
            CancellationToken = source.Token
        };

        Sources = await obfuscator.ObfuscateAsync(context, source.Token);
    }

    public async Task<InlineSource> GetSourceByPathAsync(string path)
    {
        var actual = Path.GetFullPath(Path.Combine(_root!, path));
        if (Sources!.TryGetValue(actual, out var val))
        {
            var original = await GetOriginalSourceByPathAsync(actual);
            return new InlineSource(path, val, original);
        }

        return new InlineSource(path, null, null);
    }

    public async Task<InlineSymbol> GetSymbolByPathAsync(string path)
    {
        throw new NotImplementedException();
    }

    private async Task<string?> GetOriginalSourceByPathAsync(string path)
    {
        var projects = await Workspace!.GetProjectsAsync(CancellationToken.None);
        var solution = new PlanaSolution(projects);
        var document = solution.Projects.SelectMany(w => w.Documents).FirstOrDefault(w => w.Path == path)!;
        var node = await document.OriginalSyntaxTree.GetRootAsync();

        return node.NormalizeWhitespace().ToFullString();
    }
}