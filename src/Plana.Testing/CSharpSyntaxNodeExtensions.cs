// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Plana.Testing;

public static class CSharpSyntaxNodeExtensions
{
    private static readonly Regex Hexadecimal = new("^0x[a-zA-Z0-9]+$", RegexOptions.Compiled);

    public static bool ToHaveHexadecimalLikeString(this CSharpSyntaxNode obj, string prefix = "_", string suffix = "")
    {
        var str = obj.ToFullString().Trim();
        if (!str.StartsWith(prefix))
            return false;

        if (!str.EndsWith(suffix))
            return false;

        return Hexadecimal.IsMatch(str[prefix.Length..^suffix.Length]);
    }

    public static string ToNormalizedTrimmedFullString(this CSharpSyntaxNode obj)
    {
        return obj.NormalizeWhitespace().ToFullString().Trim();
    }

    public static string ToIdentifier(this CSharpSyntaxNode obj)
    {
        return obj.ToNormalizedTrimmedFullString();
    }
}