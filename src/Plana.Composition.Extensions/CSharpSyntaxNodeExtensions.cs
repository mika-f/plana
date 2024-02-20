// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Plana.Composition.Extensions;

public static class CSharpSyntaxNodeExtensions
{
    public static string ToNormalizedFullString(this CSharpSyntaxNode node)
    {
        return node.NormalizeWhitespace().ToFullString();
    }

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
}