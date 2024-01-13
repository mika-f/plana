// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions.Algorithm;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Attributes;

namespace Plana.Composition.DisableConsoleOutput;

[ObfuscatorAlgorithm("disable-console-output")]
public class DisableConsoleOutputPlugin : IObfuscatorAlgorithm
{
    public IReadOnlyCollection<IObfuscatorAlgorithmOption> Options => new List<IObfuscatorAlgorithmOption>();

    public string Name => "Disable Console Output";

    public void BindParameters(IObfuscatorParameterBinder binder)
    {
        // Nothing to do
    }

    public async Task ObfuscateAsync(List<IProject> projects, CancellationToken ct)
    {
        foreach (var document in projects.SelectMany(w => w.Documents))
        {
            var log = document.SemanticModel.Compilation.GetTypeByMetadataName("UnityEngine.Debug")!;
            var oldNode = await document.SyntaxTree.GetRootAsync(ct);

            // one class one document

            foreach (var @class in oldNode.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                var invocations = @class.DescendantNodes()
                                        .OfType<InvocationExpressionSyntax>()
                                        .Where(w => IsReceiverIsUnityEngineDebug(document.SemanticModel, w, log))
                                        .Select(w => document.SemanticModel.GetSymbolInfo(w).Symbol!)
                                        .ToList();

                if (invocations.Count != 0)
                {
                    var rewriter = new CSharpSyntaxProcessor(document, invocations);
                    var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);
                    var newTree = CSharpSyntaxTree.Create(newNode, document.SyntaxTree.Options, document.SyntaxTree.FilePath, document.SyntaxTree.Encoding);

                    await document.WriteSyntaxTreeAsync(newTree, ct);
                }
            }
        }
    }

    private bool IsReceiverIsUnityEngineDebug(SemanticModel sm, InvocationExpressionSyntax invocation, INamedTypeSymbol t)
    {
        var si = sm.GetSymbolInfo(invocation);
        if (si.Symbol is IMethodSymbol symbol)
        {
            var receiver = symbol.ReceiverType;
            return receiver?.Equals(t, SymbolEqualityComparer.Default) ?? false;
        }

        return false;
    }
}