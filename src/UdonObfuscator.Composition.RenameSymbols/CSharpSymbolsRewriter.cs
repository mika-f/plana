// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Composition.RenameSymbols;

internal class CSharpSymbolsRewriter(IDocument document, IReadOnlyDictionary<ISymbol, string> dict) : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var newNode = base.VisitClassDeclaration(node);
        if (newNode is ClassDeclarationSyntax @class)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return @class.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitEnumDeclaration(EnumDeclarationSyntax node)
    {
        var newNode = base.VisitEnumDeclaration(node);
        if (newNode is EnumDeclarationSyntax @enum)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return @enum.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
    {
        var newNode = base.VisitMethodDeclaration(node);
        if (newNode is MethodDeclarationSyntax method)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return method.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitVariableDeclarator(VariableDeclaratorSyntax node)
    {
        var newNode = base.VisitVariableDeclarator(node);
        if (newNode is VariableDeclaratorSyntax variable)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return variable.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var newNode = base.VisitIdentifierName(node);
        if (newNode is IdentifierNameSyntax identifier)
        {
            var si = document.SemanticModel.GetSymbolInfo(node);
            var symbol = si.Symbol;
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return identifier.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }
}