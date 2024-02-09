// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

namespace Plana.Composition.ShuffleSymbols;

[PlanaPlugin("shuffle-symbols")]
public class ShuffleSymbols : IPlanaPlugin2
{
    public IReadOnlyCollection<IPlanaPluginOption> Options => [];

    public string Name => "Shuffle Symbols";

    public void BindParameters(IPlanaPluginParameterBinder binder) { }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        var symbols = new List<ISymbol>();

        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var walker = new CSharpSymbolWalker(document, symbols);
            var node = await document.SyntaxTree.GetRootAsync(context.CancellationToken);

            walker.Visit(node);
        }

        Dictionary<ISymbol, string> dict = [];
        var random = new Random();
        var names = symbols.Select(w => w.Name).ToArray();

        foreach (var name in names.Select((w, i) => (Value: w, Index: i)))
            dict.Add(symbols[name.Index], name.Value);

        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var rewriter = new CSharpSymbolRewriter(document, dict);
            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);
            var newTree = CSharpSyntaxTree.Create(newNode, document.SyntaxTree.Options, document.SyntaxTree.FilePath, document.SyntaxTree.Encoding);

            await document.WriteSyntaxTreeAsync(newTree, context.CancellationToken);
        }
    }
}