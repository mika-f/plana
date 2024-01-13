// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine.Invocation;

namespace Plana.CLI.Commands.Abstractions;

internal class AnonymousCommandHandler(Func<InvocationContext, Task> handler) : ICommandHandler
{
    public int Invoke(InvocationContext context)
    {
        throw new NotSupportedException();
    }

    public async Task<int> InvokeAsync(InvocationContext context)
    {
        var value = (object)handler(context);
        switch (value)
        {
            case Task<int> exitCodeTask:
                return await exitCodeTask;

            case Task task:
                await task;
                return context.ExitCode;

            case int exitCode:
                return exitCode;

            default:
                return context.ExitCode;
        }
    }
}