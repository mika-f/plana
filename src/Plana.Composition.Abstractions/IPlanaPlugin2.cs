// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Enum;

namespace Plana.Composition.Abstractions;

public interface IPlanaPlugin2 : IPlanaPlugin
{
    Task IPlanaPlugin.RunAsync(IPlanaPluginRunContext context)
    {
        switch (context.Kind)
        {
            case RunKind.Obfuscate:
                return ObfuscateAsync(context);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        return Task.CompletedTask;
    }
}