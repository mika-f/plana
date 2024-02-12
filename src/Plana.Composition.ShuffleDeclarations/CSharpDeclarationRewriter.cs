// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;

namespace Plana.Composition.ShuffleDeclarations;

internal class CSharpDeclarationRewriter(IPlanaSecureRandom random) : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        var members = new Span<MemberDeclarationSyntax>([.. node.Members]);
        random.Shuffle(members);

        return node.WithMembers(SyntaxFactory.List(members.ToArray()));
    }
}