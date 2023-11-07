﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Composition.DisableConsoleOutput;

internal class CSharpSyntaxProcessor(IDocument document, List<ISymbol> invocations) : CSharpSyntaxRewriter
{
    private const string LoggerStubMethodIdentifier = "_UnityEngineDebugLogStub";

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var newNode = base.VisitClassDeclaration(node);
        if (newNode is ClassDeclarationSyntax declaration)
        {
            var m = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), SyntaxFactory.Identifier(LoggerStubMethodIdentifier));
            var b = SyntaxFactory.Block();
            return declaration.AddMembers(m.WithBody(b));
        }

        return newNode;
    }

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        var expression = node.Expression;
        if (expression is InvocationExpressionSyntax invocation)
        {
            var si = document.SemanticModel.GetSymbolInfo(invocation);
            if (si.Symbol is IMethodSymbol m && invocations.Contains(m))
            {
                var expr = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(LoggerStubMethodIdentifier));
                return SyntaxFactory.ExpressionStatement(expr);
            }
        }

        return node;
    }
}