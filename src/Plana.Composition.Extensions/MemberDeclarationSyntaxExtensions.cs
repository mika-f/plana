// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.Extensions;

public static class MemberDeclarationSyntaxExtensions
{
    public static bool HasModifier(this MemberDeclarationSyntax node, SyntaxToken modifier)
    {
        return node.Modifiers.Any(w => w.IsEquivalentTo(modifier));
    }
}