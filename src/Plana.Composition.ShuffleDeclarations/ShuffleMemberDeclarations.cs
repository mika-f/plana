// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

namespace Plana.Composition.ShuffleDeclarations;

[PlanaPlugin("shuffle-declarations")]
public class ShuffleMemberDeclarations : IPlanaPlugin2
{
    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption>().AsReadOnly();

    public string Name => "Shuffle Declarations";

    public void BindParameters(IPlanaPluginParameterBinder binder)
    {
        // Nothing to do
    }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var rewriter = new CSharpDeclarationRewriter(context.SecureRandom);
            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);
            var newTree = CSharpSyntaxTree.Create(newNode, document.SyntaxTree.Options, document.SyntaxTree.FilePath, document.SyntaxTree.Encoding);

            await document.WriteSyntaxTreeAsync(newTree, context.CancellationToken);
        }
    }
}