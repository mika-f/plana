// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Parsing;

using Plana.Composition.Abstractions;

namespace Plana.CLI.Commands.Abstractions;

internal class ObfuscatorParameterBinder(ParseResult context, Dictionary<IPlanaPluginOption, Option> dict) : IPlanaPluginParameterBinder
{
    public bool GetValue(IPlanaPluginOption option)
    {
        if (dict.TryGetValue(option, out var val))
        {
            var ret = context.GetValueForOption(val);
            if (ret is bool b)
                return b;
        }

        return option.DefaultValue;
    }

    public T GetValue<T>(IPlanaPluginOption<T> option)
    {
        if (dict.TryGetValue(option, out var val))
        {
            var isImplicit = context.FindResultFor(val)?.IsImplicit ?? true;
            if (isImplicit)
                return option.DefaultValue;

            var ret = context.GetValueForOption(val);
            if (ret is T t)
                return t;
        }

        return option.DefaultValue;
    }
}