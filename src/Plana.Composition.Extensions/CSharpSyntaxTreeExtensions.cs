// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Plana.Composition.Extensions;

public static class CSharpSyntaxTreeExtensions
{
    public static string ToNormalizedFullString(this CSharpSyntaxTree tree)
    {
        return tree.GetCompilationUnitRoot().NormalizeWhitespace().ToFullString();
    }
}