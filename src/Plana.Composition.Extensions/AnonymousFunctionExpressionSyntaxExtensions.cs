// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.Extensions;

public static class AnonymousFunctionExpressionSyntaxExtensions
{
    public static bool HasModifier(this AnonymousFunctionExpressionSyntax node, SyntaxToken modifier)
    {
        return node.Modifiers.Any(w => w.IsEquivalentTo(modifier));
    }
}