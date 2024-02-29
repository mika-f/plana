// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Composition.DisableConsoleOutput;

internal class CSharpSyntaxProcessor(IDocument document, List<CSharpSyntaxNode> invocations) : CSharpSyntaxRewriter
{
    private const string LoggerStubMethodIdentifier = "__LogStubInternal";

    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var newNode = base.VisitClassDeclaration(node);
        if (newNode is ClassDeclarationSyntax declaration)
        {
            var m = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), SyntaxFactory.Identifier(LoggerStubMethodIdentifier));
            var b = SyntaxFactory.Block();
            return declaration.AddMembers(m.WithBody(b).WithModifiers(SyntaxTokenList.Create(SyntaxFactory.Token(SyntaxKind.PrivateKeyword))));
        }

        return newNode;
    }

    public override SyntaxNode? VisitExpressionStatement(ExpressionStatementSyntax node)
    {
        var expression = node.Expression;
        if (expression is InvocationExpressionSyntax invocation)
            if (invocations.Any(w => w.IsEquivalentTo(invocation)))
            {
                var expr = SyntaxFactory.InvocationExpression(SyntaxFactory.IdentifierName(LoggerStubMethodIdentifier));
                return SyntaxFactory.ExpressionStatement(expr);
            }

        return node;
    }
}