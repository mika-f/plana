// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine.Binding;

using Plana.CLI.Commands;
using Plana.Logging;
using Plana.Logging.Abstractions;

namespace Plana.CLI.Bindings;

internal class LoggerBinder : BinderBase<ILogger>
{
    protected override ILogger GetBoundValue(BindingContext bindingContext)
    {
        return GetLogger(bindingContext);
    }

    private static ILogger GetLogger(BindingContext context)
    {
        var value = context.ParseResult.GetValueForOption(GlobalCommandLineOptions.LogLevel);
        var r = context.ParseResult.GetValueForOption(GlobalCommandLineOptions.RetrieveArgs);
        var logger = new Logger(r ? LogLevel.Silent : value);

        context.AddService(typeof(ILogger), _ => logger);
        return logger;
    }
}