// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine.Binding;

using UdonObfuscator.CLI.Commands;
using UdonObfuscator.Logging;
using UdonObfuscator.Logging.Abstractions;

namespace UdonObfuscator.CLI.Bindings;

internal class LoggerBinder : BinderBase<ILogger>
{
    protected override ILogger GetBoundValue(BindingContext bindingContext)
    {
        return GetLogger(bindingContext);
    }

    private static ILogger GetLogger(BindingContext context)
    {
        var value = context.ParseResult.GetValueForOption(GlobalCommandLineOptions.LogLevel);
        var logger = new Logger(value);

        context.AddService(typeof(ILogger), _ => logger);
        return logger;
    }
}