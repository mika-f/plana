// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UdonObfuscator.Composition.ShuffleDeclarations;

internal class CSharpDeclarationRewriter(Random random) : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var members = node.Members.ToArray();
        random.Shuffle(members);

        return node.WithMembers(SyntaxFactory.List(members));
    }
}