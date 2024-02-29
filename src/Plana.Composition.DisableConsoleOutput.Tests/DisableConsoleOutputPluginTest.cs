// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Testing;

namespace Plana.Composition.DisableConsoleOutput.Tests;

public class DisableConsoleOutputPluginTest
{
    [Fact]
    public async Task CanInstantiateWithDefaults()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output");
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Disable Console Output", instance.Name);
        Assert.Equal(1, instance.Options.Count);

        Assert.Empty(instance.Symbols);
    }

    [Fact]
    public async Task CanInstantiateWithTypeArgs()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=T:System.Diagnostics.Debug");
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Disable Console Output", instance.Name);
        Assert.Equal(1, instance.Options.Count);

        Assert.Single(instance.Symbols);
    }

    [Fact]
    public async Task CanInstantiateWithMethodArgs()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=M:System.Console.WriteLine");
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Disable Console Output", instance.Name);
        Assert.Equal(1, instance.Options.Count);

        Assert.Single(instance.Symbols);
    }

    [Fact]
    public async Task CanInstantiateWithUnknownArgs()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=N:System");
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Disable Console Output", instance.Name);
        Assert.Equal(1, instance.Options.Count);

        Assert.Empty(instance.Symbols);
    }

    [Fact]
    public async Task RemoveType()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=T:System.Diagnostics.Debug");
        await container.RunAsync();

        var source = await container.GetSourceByPathAsync("Plana.Logging/Logger.cs");

        Assert.False(await source.ContainsAsync("Debug.WriteLine"));
        Assert.True(await source.ContainsAsync("private void __LogStubInternal()"));
    }

    [Fact]
    public async Task RemoveMethod()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=M:System.Console.WriteLine");
        await container.RunAsync();

        var source = await container.GetSourceByPathAsync("Plana.Logging/Logger.cs");

        Assert.False(await source.ContainsAsync("Console.WriteLine"));
        Assert.True(await source.ContainsAsync("private void __LogStubInternal()"));
    }

    [Fact]
    public async Task RemoveUnknownType_NoSourceDiffs()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=M:System.Diagnostics.Console");
        await container.RunAsync();

        var source = await container.GetSourceByPathAsync("Plana.Logging/Logger.cs");
        await source.NoDiffs();
    }

    [Fact]
    public async Task RemoveUnknownMethod_NoSourceDiffs()
    {
        var container = new PlanaContainer<DisableConsoleOutputPlugin>("disable-console-output", "disable-symbols=M:System.Console.WriteLine22");
        await container.RunAsync();

        var source = await container.GetSourceByPathAsync("Plana.Logging/Logger.cs");
        await source.NoDiffs();
    }
}