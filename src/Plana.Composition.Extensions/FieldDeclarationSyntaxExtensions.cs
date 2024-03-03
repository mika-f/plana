// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.Extensions;

public static class FieldDeclarationSyntaxExtensions
{
    public static bool HasModifier(this FieldDeclarationSyntax m, SyntaxKind modifier)
    {
        return m.HasModifiers(modifier);
    }

    public static bool HasNotModifier(this FieldDeclarationSyntax m, SyntaxKind modifier)
    {
        return !m.HasModifier(modifier);
    }

    public static bool HasModifiers(this FieldDeclarationSyntax m, params SyntaxKind[] modifiers)
    {
        return modifiers.All(w => m.Modifiers.Any(v => v.IsKind(w)));
    }
}