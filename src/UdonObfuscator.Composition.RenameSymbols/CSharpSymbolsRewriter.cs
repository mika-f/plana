// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Composition.RenameSymbols;

internal class CSharpSymbolsRewriter(IDocument document, Dictionary<ISymbol, string> dict) : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
    {
        var declaration = base.VisitNamespaceDeclaration(node);
        if (declaration is NamespaceDeclarationSyntax @namespace)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return @namespace.WithName(SyntaxFactory.IdentifierName(value));
        }

        return declaration;
    }

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var declaration = base.VisitClassDeclaration(node);
        if (declaration is ClassDeclarationSyntax @class)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return @class.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return declaration;
    }
}