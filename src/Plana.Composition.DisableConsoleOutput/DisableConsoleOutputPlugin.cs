// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

namespace Plana.Composition.DisableConsoleOutput;

[PlanaPlugin("disable-console-output")]
public class DisableConsoleOutputPlugin : IPlanaPlugin2
{
    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption>();

    public string Name => "Disable Console Output";

    public void BindParameters(IPlanaPluginParameterBinder binder)
    {
        // Nothing to do
    }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var log = document.SemanticModel.Compilation.GetTypeByMetadataName("UnityEngine.Debug")!;
            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);

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

                    await document.WriteSyntaxTreeAsync(newTree, context.CancellationToken);
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