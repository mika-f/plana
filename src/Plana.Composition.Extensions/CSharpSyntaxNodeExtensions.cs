// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

namespace Plana.Composition.Extensions;

public static class CSharpSyntaxNodeExtensions
{
    public static bool HasAnnotationComment(this CSharpSyntaxNode node)
    {
        return node.HasAnnotationComment(AnnotationComment.DefaultAnnotationComment);
    }

    public static bool HasAnnotationComment(this CSharpSyntaxNode node, AnnotationComment annotation)
    {
        if (node.HasLeadingTrivia)
        {
            var trivia = node.GetLeadingTrivia();
            return trivia.ToFullString().Trim() == annotation.Comment;
        }

        return false;
    }

    public static TNode? FirstAncestor<TNode>(this CSharpSyntaxNode node, Func<TNode, bool>? predicate = null, bool ascendOutOfTrivia = true) where TNode : CSharpSyntaxNode
    {
        var f = node.FirstAncestorOrSelf(predicate, ascendOutOfTrivia);
        return f == node ? null : f;
    }
}