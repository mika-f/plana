// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine.Binding;

using Plana.CLI.Commands;
using Plana.Hosting;
using Plana.Hosting.Abstractions;
using Plana.Logging.Abstractions;

namespace Plana.CLI.Bindings;

internal class HostingContainerBinder : BinderBase<IHostingContainer>
{
    protected override IHostingContainer GetBoundValue(BindingContext bindingContext)
    {
        return GetResolver(bindingContext);
    }

    private static IHostingContainer GetResolver(BindingContext context)
    {
        var value = context.ParseResult.GetValueForOption(GlobalCommandLineOptions.Plugins) ?? throw new InvalidOperationException();
        var logger = (ILogger?)context.GetService(typeof(ILogger));

        return new HostingContainer(value, logger);
    }
}