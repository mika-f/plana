// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

[assembly: InternalsVisibleTo("Plana.Composition.DisableConsoleOutput.Tests")]

namespace Plana.Composition.DisableConsoleOutput;

[PlanaPlugin("disable-console-output")]
public class DisableConsoleOutputPlugin : IPlanaPlugin2
{
    private static readonly PlanaPluginOption<string> DisableSymbols = new("disable-symbols", "Disable Symbols", "symbols for removing from source, T: for type, M: for methods (e.g. T:System.Diagnostics.Debug)", string.Empty);

    internal readonly List<RoughSymbol> Symbols = new();

    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption> { DisableSymbols };

    public string Name => "Disable Console Output";

    public void BindParameters(IPlanaPluginParameterBinder binder)
    {
        var symbols = binder.GetValue(DisableSymbols);
        foreach (var s in symbols.Split(","))
        {
            if (s.StartsWith("T:"))
            {
                Symbols.Add(new RoughSymbol(s["T:".Length..], null, SymbolKind.Type));
                continue;
            }

            if (s.StartsWith("M:"))
            {
                var str = s["M:".Length..].Split(".");
                var t = string.Join(".", str.TakeWhile((w, i) => i + 1 < str.Length));
                var m = str.Last();

                Symbols.Add(new RoughSymbol(t, m, SymbolKind.Method));
            }
        }
    }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            var invocations = new List<CSharpSyntaxNode>();

            foreach (var symbol in Symbols)
            {
                if (!symbol.GetSymbol(document.SemanticModel))
                    continue;

                invocations.AddRange(oldNode.DescendantNodes()
                                            .OfType<InvocationExpressionSyntax>()
                                            .Where(w => IsMatchTo(document.SemanticModel, w, symbol.Symbol))
                                            .ToList());
            }

            if (invocations.Count != 0)
            {
                var rewriter = new CSharpSyntaxProcessor(document, invocations);
                var newNode = (CSharpSyntaxNode)rewriter.Visit(oldNode);

                await document.ApplyChangesAsync(newNode, context.CancellationToken);
            }
        }
    }

    private bool IsMatchTo(SemanticModel sm, InvocationExpressionSyntax invocation, ISymbol t)
    {
        var si = sm.GetSymbolInfo(invocation);
        if (si.Symbol is not IMethodSymbol symbol)
            return false;


        if (t is IMethodSymbol)
        {
            var format = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, memberOptions: SymbolDisplayMemberOptions.IncludeContainingType);
            return symbol.ToDisplayString(format) == t.ToDisplayString(format);
        }

        if (t is ITypeSymbol)
        {
            var receiver = symbol.ReceiverType;
            return receiver?.Equals(t, SymbolEqualityComparer.Default) ?? false;
        }

        return false;
    }

    internal class RoughSymbol(string type, string? method, SymbolKind kind)
    {
        public ISymbol? Symbol { get; private set; }

        [MemberNotNullWhen(true, nameof(Symbol))]
        public bool GetSymbol(SemanticModel sm)
        {
            var t = sm.Compilation.GetTypeByMetadataName(type);
            if (t == null)
                return false;

            if (kind == SymbolKind.Type)
            {
                Symbol = t;
                return true;
            }

            if (string.IsNullOrWhiteSpace(method))
                throw new InvalidOperationException(nameof(method));

            var m = t.GetMembers(method).FirstOrDefault();
            if (m == null)
                return false;

            Symbol = m;
            return true;
        }
    }

    internal enum SymbolKind
    {
        Type,
        Method
    }
}