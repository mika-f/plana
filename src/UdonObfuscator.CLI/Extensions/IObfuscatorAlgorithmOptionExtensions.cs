// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

using UdonObfuscator.Composition.Abstractions.Algorithm;

namespace UdonObfuscator.CLI.Extensions;

// ReSharper disable once InconsistentNaming
internal static class IObfuscatorAlgorithmOptionExtensions
{
    public static Option? ToOption(this IObfuscatorAlgorithmOption option)
    {
        var t = typeof(Option<>).MakeGenericType(option.ValueType);
        return Activator.CreateInstance(t, option.Name, option.GetDefaultValue, option.Description) as Option;
    }
}