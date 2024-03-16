// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;

namespace Plana.Testing;

public static class StringExtensions
{
    private static readonly Regex Hexadecimal = new("^0x[a-zA-Z0-9]+$", RegexOptions.Compiled);

    public static bool ToHaveHexadecimalLikeString(this string obj, string prefix = "_", string suffix = "")
    {
        var str = obj.Trim();
        if (!str.StartsWith(prefix))
            return false;

        if (!str.EndsWith(suffix))
            return false;

        return Hexadecimal.IsMatch(str[prefix.Length..^suffix.Length]);
    }
}