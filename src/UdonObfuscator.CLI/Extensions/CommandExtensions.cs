// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Binding;
using System.CommandLine.Invocation;

using UdonObfuscator.CLI.Commands.Abstractions;

namespace UdonObfuscator.CLI.Extensions;

internal static class CommandExtensions
{
    public static void AddCommands(this Command command, params Command[] commands)
    {
        foreach (var cmd in commands)
            command.AddCommand(cmd);
    }

    public static void AddOptions(this Command command, params Option[] options)
    {
        foreach (var option in options)
            command.AddOption(option);
    }

    public static void SetHandlerEx<T>(this Command command, Func<InvocationContext, T, CancellationToken, Task> handle, IValueDescriptor<T> symbol)
    {
        command.Handler = new AnonymousCommandHandler(context =>
        {
            var value1 = GetValueForHandlerParameter(symbol, context);
            var ct = context.GetCancellationToken();
            return handle(context, value1!, ct);
        });
    }

    public static void SetHandlerEx<T1, T2>(this Command command, Func<InvocationContext, T1, T2, CancellationToken, Task> handle, IValueDescriptor<T1> symbol1, IValueDescriptor<T2> symbol2)
    {
        command.Handler = new AnonymousCommandHandler(context =>
        {
            var value1 = GetValueForHandlerParameter(symbol1, context);
            var value2 = GetValueForHandlerParameter(symbol2, context);
            var ct = context.GetCancellationToken();
            return handle(context, value1!, value2!, ct);
        });
    }

    public static void SetHandlerEx<T1, T2, T3>(this Command command, Func<InvocationContext, T1, T2, T3, CancellationToken, Task> handle, IValueDescriptor<T1> symbol1, IValueDescriptor<T2> symbol2, IValueDescriptor<T3> symbol3)
    {
        command.Handler = new AnonymousCommandHandler(context =>
        {
            var value1 = GetValueForHandlerParameter(symbol1, context);
            var value2 = GetValueForHandlerParameter(symbol2, context);
            var value3 = GetValueForHandlerParameter(symbol3, context);
            var ct = context.GetCancellationToken();
            return handle(context, value1!, value2!, value3!, ct);
        });
    }

    private static T? GetValueForHandlerParameter<T>(IValueDescriptor<T> symbol, InvocationContext context)
    {
        if (symbol is IValueSource source && source.TryGetValue(symbol, context.BindingContext, out var ret) && ret is T value)
            return value;

        return symbol switch
        {
            Argument<T> argument => context.ParseResult.GetValueForArgument(argument),
            Option<T> option => context.ParseResult.GetValueForOption(option),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}