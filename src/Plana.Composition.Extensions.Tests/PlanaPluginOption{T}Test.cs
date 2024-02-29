// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Exceptions;

namespace Plana.Composition.Extensions.Tests;

public class PlanaPluginOptionTestT
{
    private static PlanaPluginOption<T?> Construct<T>(string name)
    {
        return new PlanaPluginOption<T?>(name, "", default);
    }

    [Fact]
    public void FriendlyNameIsNotSameValueWithNamePassedInConstructor()
    {
        var instance = new PlanaPluginOption<string?>("name", "friendly name", "description", "default");
        Assert.Equal("friendly name", instance.FriendlyName);
    }

    [Fact]
    public void FriendlyNameIsSameValueWithNameNotPassedInConstructor()
    {
        var instance = new PlanaPluginOption<string?>("name", "description", "default");
        Assert.Equal(instance.Name, instance.FriendlyName);
    }

    [Fact]
    public void HasDescription()
    {
        var instance = new PlanaPluginOption<string?>("name", "description", "default");
        Assert.Equal("description", instance.Description);
    }

    [Fact]
    public void HasNonBoolDefaultValue()
    {
        var instance = new PlanaPluginOption<string?>("name", "description", "it's default value!");
        Assert.Equal("it's default value!", instance.DefaultValue);
    }

    [Fact]
    public void NameContainsOnlyAlphaNumerical()
    {
        var exception1 = Record.Exception(() => Construct<string>("name"));
        Assert.Null(exception1);

        var exception2 = Record.Exception(() => Construct<string>("name-a"));
        Assert.Null(exception2);

        var exception3 = Record.Exception(() => Construct<string>("name_b"));
        Assert.Null(exception3);

        var exception4 = Record.Exception(() => Construct<string>("name-09"));
        Assert.Null(exception4);

        var exception5 = Record.Exception(() => Construct<string>("Name"));
        Assert.Null(exception5);

        Assert.Throws<InvalidFormatException>(() => Construct<string>("@X"));
        Assert.Throws<InvalidFormatException>(() => Construct<string>("こんにちは"));
    }

    [Fact]
    public void NameDoesNotStartsWithHyphen()
    {
        Assert.Throws<InvalidFormatException>(() => Construct<string>("-name"));
        Assert.Throws<InvalidFormatException>(() => Construct<string>("-name"));
    }
}