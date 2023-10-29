// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine.Binding;

using UdonObfuscator.CLI.Commands;
using UdonObfuscator.HostInfrastructure;
using UdonObfuscator.Logging.Abstractions;

namespace UdonObfuscator.CLI.Bindings;

internal class PluginBinder : BinderBase<PluginResolver>
{
    protected override PluginResolver GetBoundValue(BindingContext bindingContext)
    {
        return GetResolver(bindingContext);
    }

    private static PluginResolver GetResolver(BindingContext context)
    {
        var value = context.ParseResult.GetValueForOption(GlobalCommandLineOptions.LoadFrom) ?? throw new InvalidOperationException();
        var logger = (ILogger?)context.GetService(typeof(ILogger));

        return new PluginResolver(value, logger);
    }
}