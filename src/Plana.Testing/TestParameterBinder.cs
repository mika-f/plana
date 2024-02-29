// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;

namespace Plana.Testing;

internal class TestParameterBinder(Dictionary<string, object> dict) : IPlanaPluginParameterBinder
{
    public bool GetValue(IPlanaPluginOption option)
    {
        if (dict.TryGetValue(option.Name, out var val))
            if (val is bool b)
                return b;

        return (bool)option.DefaultValue!;
    }

    public T? GetValue<T>(IPlanaPluginOption<T> option)
    {
        if (dict.TryGetValue(option.Name, out var val))
            if (val is T t)
                return t;

        return (T?)option.DefaultValue;
    }
}