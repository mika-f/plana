// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Parsing;

using Plana.Composition.Abstractions.Algorithm;

namespace Plana.CLI.Commands.Abstractions;

internal class ObfuscatorParameterBinder(ParseResult context, Dictionary<IObfuscatorAlgorithmOption, Option> dict) : IObfuscatorParameterBinder
{
    public bool GetValue(IObfuscatorAlgorithmOption option)
    {
        if (dict.TryGetValue(option, out var val))
        {
            var ret = context.GetValueForOption(val);
            if (ret is bool b)
                return b;
        }

        return option.GetDefaultValue();
    }

    public T GetValue<T>(IObfuscatorAlgorithmOption<T> option)
    {
        if (dict.TryGetValue(option, out var val))
        {
            var isImplicit = context.FindResultFor(val)?.IsImplicit ?? true;
            if (isImplicit)
                return option.GetDefaultValue();

            var ret = context.GetValueForOption(val);
            if (ret is T t)
                return t;
        }

        return option.GetDefaultValue();
    }
}