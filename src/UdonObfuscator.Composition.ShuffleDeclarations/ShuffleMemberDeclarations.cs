// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

using UdonObfuscator.Composition.Abstractions.Algorithm;
using UdonObfuscator.Composition.Abstractions.Analysis;
using UdonObfuscator.Composition.Abstractions.Attributes;

namespace UdonObfuscator.Composition.ShuffleDeclarations;

[ObfuscatorAlgorithm("shuffle-declarations")]
public class ShuffleMemberDeclarations : IObfuscatorAlgorithm
{
    public IReadOnlyCollection<IObfuscatorAlgorithmOption> Options => new List<IObfuscatorAlgorithmOption>().AsReadOnly();

    public string Name => "Shuffle Declarations";

    public void BindParameters(IObfuscatorParameterBinder binder)
    {
        // Nothing to do
    }

    public async Task ObfuscateAsync(List<IProject> projects, CancellationToken ct)
    {
        var random = new Random();

        foreach (var document in projects.SelectMany(w => w.Documents))
        {
            var rewriter = new CSharpDeclarationRewriter(random);
            var oldNode = await document.SyntaxTree.GetRootAsync(ct);
            var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);
            var newTree = CSharpSyntaxTree.Create(newNode, document.SyntaxTree.Options, document.SyntaxTree.FilePath, document.SyntaxTree.Encoding);

            await document.WriteSyntaxTreeAsync(newTree, ct);
        }
    }
}