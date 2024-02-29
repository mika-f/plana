// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

using Plana.Composition.Abstractions;

namespace Plana.CLI.Extensions;

// ReSharper disable once InconsistentNaming
internal static class IPlanaPluginOptionExtensions
{
    public static Option? ToOptions(this IPlanaPluginOption option)
    {
        var i = option.ValueType;
        var t = typeof(Option<>).MakeGenericType(i);


        if (i == typeof(bool))
        {
            var d = (bool)option.DefaultValue!;
            if (d)
                // if default value is true, add --no-xxx option
                // .ctor(name, getDefaultValue, description)
                return new Option<bool>($"--no-{option.Name}", option.Description);

            // if default value is false or unset, add --xxx option
            // .ctor(name, description)
            return new Option<bool>($"--{option.Name}", option.Description);
        }

        if (i == typeof(string))
            return new Option<string>($"--{option.Name}", () => (string)option.DefaultValue!, option.Description);

        // .ctor(name, getDefaultValue, description)
        return Activator.CreateInstance(t, $"--{option.Name}", () => option.DefaultValue, option.Description) as Option;
    }
}