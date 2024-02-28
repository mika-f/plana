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
        var transformers = new List<CSharpSyntaxRewriter>
        {
            new RemoveRegionAndEndRegionRewriter(),
            new CSharpDeclarationRewriter(context.SecureRandom)
        };

        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var node = await document.SyntaxTree.GetRootAsync(context.CancellationToken);

            foreach (var transformer in transformers)
                node = (CSharpSyntaxNode)transformer.Visit(node);

            await document.ApplyChangesAsync(node, context.CancellationToken);
        }
    }
}