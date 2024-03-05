// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;

namespace Plana.Composition.RenameSymbols;

internal class CSharpSymbolsRewriter(IDocument document, bool keepNameOnInspector, IReadOnlyDictionary<ISymbol, string> dict) : CSharpSyntaxRewriter
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

    public override SyntaxNode? VisitEnumMemberDeclaration(EnumMemberDeclarationSyntax node)
    {
        var newNode = base.VisitEnumMemberDeclaration(node);
        if (newNode is EnumMemberDeclarationSyntax member)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
            {
                if (keepNameOnInspector && !node.HasAttribute(document.SemanticModel, "UnityEngine.InspectorNameAttribute"))
                {
                    // UnityEngine.InspectorNameAttribute for Inspector
                    var arg = SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(symbol.Name)));
                    var args = SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new[] { arg }));
                    var attr = SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("global::UnityEngine.InspectorName"), args);
                    var attrs = SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new[] { attr }));

                    return member.WithIdentifier(SyntaxFactory.Identifier(value)).AddAttributeLists(attrs);
                }

                return member.WithIdentifier(SyntaxFactory.Identifier(value));
            }
        }

        return newNode;
    }

    public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        var newNode = base.VisitConstructorDeclaration(node);
        if (newNode is ConstructorDeclarationSyntax constructor)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            // constructor name must be equals to containing type name
            if (symbol != null && dict.TryGetValue(symbol.ContainingType, out var value))
                return constructor.WithIdentifier(SyntaxFactory.Identifier(value));
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

    public override SyntaxNode? VisitParameter(ParameterSyntax node)
    {
        var newNode = base.VisitParameter(node);
        if (newNode is ParameterSyntax parameter)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return parameter.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        var newNode = base.VisitPropertyDeclaration(node);
        if (newNode is PropertyDeclarationSyntax property)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return property.WithIdentifier(SyntaxFactory.Identifier(value));
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

    public override SyntaxNode? VisitForEachStatement(ForEachStatementSyntax node)
    {
        var newNode = base.VisitForEachStatement(node);
        if (newNode is ForEachStatementSyntax statement)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return statement.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitGenericName(GenericNameSyntax node)
    {
        var newNode = base.VisitGenericName(node);
        if (newNode is GenericNameSyntax generic)
        {
            var si = document.SemanticModel.GetSymbolInfo(node);
            var symbol = si.Symbol;

            if (symbol is INamedTypeSymbol t)
            {
                var s = t.ConstructedFrom;
                if (dict.TryGetValue(s, out var value))
                    return generic.WithIdentifier(SyntaxFactory.Identifier(value));
            }
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
            if (symbol != null)
            {
                if (symbol is IMethodSymbol { IsExtensionMethod: true } m)
                {
                    if (dict.TryGetValue(m.ReducedFrom!, out var value1))
                        return identifier.WithIdentifier(SyntaxFactory.Identifier(value1));
                }
                else
                {
                    if (dict.TryGetValue(symbol, out var value1))
                        return identifier.WithIdentifier(SyntaxFactory.Identifier(value1));

                    if (dict.TryGetValue(symbol.OriginalDefinition, out var value2))
                        return identifier.WithIdentifier(SyntaxFactory.Identifier(value2));
                }
            }
        }

        return newNode;
    }
}