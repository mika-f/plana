// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;
using Plana.Composition.Extensions.Unity;

namespace Plana.Composition.ShuffleSymbols;

internal class CSharpSymbolWalker(IDocument document, List<ISymbol> symbols) : CSharpSyntaxWalker
{
    public override void VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        if (!node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);

            if (AllowSymbolShuffle(symbol))
                symbols.Add(symbol);
        }

        base.VisitEnumDeclaration(node);
    }

    public override void VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        if (!node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);

            if (AllowSymbolShuffle(symbol))
                symbols.Add(symbol);
        }

        base.VisitEnumMemberDeclaration(node);
    }

    public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        if (!node.HasAnnotationComment() && !node.IsUnityMessagingMethod(document.SemanticModel))
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);

            if (AllowSymbolShuffle(symbol))
                symbols.Add(symbol);
        }

        base.VisitMethodDeclaration(node);
    }

    public override void VisitParameter(ParameterSyntax node)
    {
        if (node.Parent?.Parent is CSharpSyntaxNode declaration && !declaration.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);

            if (AllowSymbolShuffle(symbol))
                symbols.Add(symbol);
        }

        base.VisitParameter(node);
    }

    public override void VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (!node.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);

            if (AllowSymbolShuffle(symbol))
                symbols.Add(symbol);
        }

        base.VisitPropertyDeclaration(node);
    }

    public override void VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        if (node.Parent?.Parent is CSharpSyntaxNode declaration && !declaration.HasAnnotationComment())
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);

            if (AllowSymbolShuffle(symbol))
                symbols.Add(symbol);
        }

        base.VisitVariableDeclarator(node);
    }

    private static bool AllowSymbolShuffle([NotNullWhen(true)] ISymbol? symbol)
    {
        if (symbol == null || symbol.IsOverride)
            return false;
        return true;
    }
}