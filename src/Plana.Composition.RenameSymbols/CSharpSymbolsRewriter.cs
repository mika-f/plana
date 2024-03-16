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

internal class CSharpSymbolsRewriter(ISolution solution, IDocument document, bool keepNameOnInspector, IReadOnlyDictionary<ISymbol, string> dict) : CSharpSyntaxRewriter
{
    private readonly List<INamedTypeSymbol> _symbols = [];

    public override SyntaxNode? VisitCompilationUnit(CompilationUnitSyntax node)
    {
        var newNode = base.VisitCompilationUnit(node);
        if (newNode is CompilationUnitSyntax compilation)
        {
            var originalUsings = node.Usings;
            var currentUsings = compilation.Usings;
            var usings = new List<UsingDirectiveSyntax>();

            foreach (var us in originalUsings.Select((w, i) => (Syntax: w, Index: i)))
            {
                var si = document.SemanticModel.GetSymbolInfo(us.Syntax.Name!);
                var ns = si.Symbol;

                if (ns != null)
                {
                    // if any parts does not rewrite, keep original name
                    if (currentUsings[us.Index].Name!.ToFullString().Split(".").Any(w => ns.ToDisplayString().Contains(w)))
                    {
                        usings.Add(us.Syntax);
                        continue;
                    }

                    // if any declaration in external source, keep both original and rewrite
                    if (ns.IsAnyDeclarationIsNotInWorkspace(solution))
                    {
                        usings.Add(us.Syntax);
                        usings.Add(currentUsings[us.Index]);
                        continue;
                    }

                    usings.Add(currentUsings[us.Index]);
                }
            }

            var namespaces = node.DescendantNodes().OfType<BaseNamespaceDeclarationSyntax>().ToList();
            foreach (var decl in namespaces)
            {
                var si = document.SemanticModel.GetSymbolInfo(decl.Name);
                var ns = si.Symbol as INamespaceSymbol;

                if (ns == null)
                    continue;


                while (true)
                {
                    if (ns == null || ns.IsGlobalNamespace)
                        break;

                    if (ns.IsAnyDeclarationIsNotInWorkspace(solution))
                    {
                        // nothing to do
                    }

                    if (_symbols.Any(w => w.ContainingNamespace.ToDisplayString().Contains(ns.ToDisplayString())))
                        usings.Add(SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName(ns.ToDisplayString())));

                    ns = ns.ContainingNamespace;
                }
            }

            return compilation.WithUsings(SyntaxFactory.List(usings));
        }

        return newNode;
    }

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

    public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
    {
        var newNode = base.VisitInterfaceDeclaration(node);
        if (newNode is InterfaceDeclarationSyntax @interface)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return @interface.WithIdentifier(SyntaxFactory.Identifier(value));
        }

        return newNode;
    }

    public override SyntaxNode? VisitRecordDeclaration(RecordDeclarationSyntax node)
    {
        var newNode = base.VisitRecordDeclaration(node);
        if (newNode is RecordDeclarationSyntax @interface)
        {
            var symbol = document.SemanticModel.GetDeclaredSymbol(node);
            if (symbol != null && dict.TryGetValue(symbol, out var value))
                return @interface.WithIdentifier(SyntaxFactory.Identifier(value));
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

    public override SyntaxNode? VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        var newNode = base.VisitMemberAccessExpression(node);
        if (newNode is MemberAccessExpressionSyntax accessor)
        {
            var si = document.SemanticModel.GetSymbolInfo(node.Expression);
            var symbol = si.Symbol;

            if (symbol != null)
                if (symbol is INamespaceSymbol ns)
                {
                    // if receiver is namespace, convert to fully qualified namespace
                    if (dict.ContainsKey(ns))
                    {
                        var parts = new List<string>();
                        var s = ns;

                        while (true)
                        {
                            if (s.IsGlobalNamespace)
                                break;

                            parts.Add(dict[s]);
                            s = s.ContainingNamespace;
                        }

                        parts.Reverse();

                        var identifier = string.Join(".", parts);
                        return accessor.WithExpression(SyntaxFactory.IdentifierName(identifier));
                    }

                    return accessor.WithExpression(SyntaxFactory.IdentifierName(ns.ToDisplayString()));
                }
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

                _symbols.Add(t);
            }

            if (symbol is IMethodSymbol m)
            {
                var s = m.ConstructedFrom;
                if (dict.TryGetValue(s, out var value))
                    return generic.WithIdentifier(SyntaxFactory.Identifier(value));
            }
        }

        return newNode;
    }

    public override SyntaxNode? VisitIdentifierName(IdentifierNameSyntax node)
    {
        var newNode = base.VisitIdentifierName(node);
        if (node.IsVar)
            return newNode;

        if (newNode is IdentifierNameSyntax identifier)
        {
            var si = document.SemanticModel.GetSymbolInfo(node);
            var symbol = si.Symbol;
            if (symbol != null)
            {
                if (symbol is IMethodSymbol { IsExtensionMethod: true } m)
                {
                    if (m.ReducedFrom != null)
                        if (dict.TryGetValue(m.ReducedFrom, out var value1))
                            return identifier.WithIdentifier(SyntaxFactory.Identifier(value1));

                    if (dict.TryGetValue(m.OriginalDefinition, out var value2))
                        return identifier.WithIdentifier(SyntaxFactory.Identifier(value2));
                }
                else
                {
                    if (dict.TryGetValue(symbol, out var value1))
                        return identifier.WithIdentifier(SyntaxFactory.Identifier(value1));

                    if (dict.TryGetValue(symbol.OriginalDefinition, out var value2))
                        return identifier.WithIdentifier(SyntaxFactory.Identifier(value2));

                    if (symbol is INamedTypeSymbol t)
                        _symbols.Add(t);
                }
            }
        }

        return newNode;
    }
}