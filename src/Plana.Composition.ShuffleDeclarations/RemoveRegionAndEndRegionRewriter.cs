// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.ShuffleDeclarations;

internal class RemoveRegionAndEndRegionRewriter() : CSharpSyntaxRewriter(true)
{
    public override SyntaxNode? VisitRegionDirectiveTrivia(RegionDirectiveTriviaSyntax node)
    {
        return SyntaxFactory.SkippedTokensTrivia();
    }

    public override SyntaxNode? VisitEndRegionDirectiveTrivia(EndRegionDirectiveTriviaSyntax node)
    {
        return SyntaxFactory.SkippedTokensTrivia();
    }
}