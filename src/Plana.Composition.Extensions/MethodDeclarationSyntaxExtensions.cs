// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.Extensions;

public static class MethodDeclarationSyntaxExtensions
{
    public static bool HasModifier(this MethodDeclarationSyntax m, SyntaxKind modifier)
    {
        return m.Modifiers.Any(w => w.IsKind(modifier));
    }
}