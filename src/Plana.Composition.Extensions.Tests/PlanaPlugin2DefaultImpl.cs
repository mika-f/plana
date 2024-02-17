// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Enum;

namespace Plana.Composition.Extensions.Tests;

public class PlanaPlugin2DefaultImpl
{
    private class PlanaPlugin2WithObfuscateImpl : IPlanaPlugin2
    {
        public bool IsRunObfuscateAsync { get; private set; }

        public IReadOnlyCollection<IPlanaPluginOption> Options => [];

        public string Name => "";

        public void BindParameters(IPlanaPluginParameterBinder binder) { }

        public Task ObfuscateAsync(IPlanaPluginRunContext context)
        {
            IsRunObfuscateAsync = true;
            return Task.CompletedTask;
        }
    }

    private class PlanaPlugin2WithDefaults : IPlanaPlugin2
    {
        public IReadOnlyCollection<IPlanaPluginOption> Options => [];

        public string Name => "";

        public void BindParameters(IPlanaPluginParameterBinder binder) { }
    }


    private class PlanaPluginContext(RunKind kind) : IPlanaPluginRunContext
    {
        public ISolution Solution { get; } = null!;
        public RunKind Kind { get; } = kind;
        public IPlanaRandom Random { get; } = null!;
        public IPlanaSecureRandom SecureRandom { get; } = null!;
        public CancellationToken CancellationToken { get; } = CancellationToken.None!;
    }

    [Fact]
    public async Task RunObfuscateAsyncWhenKindIsObfuscate()
    {
        var instance = new PlanaPlugin2WithObfuscateImpl();
        var context = new PlanaPluginContext(RunKind.Obfuscate);

        await ((IPlanaPlugin2)instance).RunAsync(context);

        Assert.True(instance.IsRunObfuscateAsync);
    }

    [Fact]
    public async Task RunObfuscateAsyncWhenKindIsObfuscateWithDefaultImpl()
    {
        var instance = new PlanaPlugin2WithDefaults();
        var context = new PlanaPluginContext(RunKind.Obfuscate);

        var exception = await Record.ExceptionAsync(async () => await ((IPlanaPlugin2)instance).RunAsync(context));
        Assert.Null(exception);
    }
}