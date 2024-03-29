﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Exceptions;

namespace Plana.Composition.Extensions.Tests;

public class PlanaPluginOptionTest
{
    [Fact]
    public void FriendlyNameIsNotSameValueWithNamePassedInConstructor()
    {
        var instance = new PlanaPluginOption("name", "friendly name", "description", false);
        Assert.Equal("friendly name", instance.FriendlyName);
    }

    [Fact]
    public void FriendlyNameIsSameValueWithNameNotPassedInConstructor()
    {
        var instance = new PlanaPluginOption("name", "description", false);
        Assert.Equal(instance.Name, instance.FriendlyName);
    }

    [Fact]
    public void HasDescription()
    {
        var instance = new PlanaPluginOption("name", "description", false);
        Assert.Equal("description", instance.Description);
    }

    [Fact]
    public void NameContainsOnlyAlphaNumerical()
    {
        var exception1 = Record.Exception(() => PlanaPluginOption.ValidateName("name"));
        Assert.Null(exception1);

        var exception2 = Record.Exception(() => PlanaPluginOption.ValidateName("name-a"));
        Assert.Null(exception2);

        var exception3 = Record.Exception(() => PlanaPluginOption.ValidateName("name_b"));
        Assert.Null(exception3);

        var exception4 = Record.Exception(() => PlanaPluginOption.ValidateName("name-09"));
        Assert.Null(exception4);

        var exception5 = Record.Exception(() => PlanaPluginOption.ValidateName("Name"));
        Assert.Null(exception5);

        Assert.Throws<InvalidFormatException>(() => PlanaPluginOption.ValidateName("@X"));
        Assert.Throws<InvalidFormatException>(() => PlanaPluginOption.ValidateName("こんにちは"));
    }

    [Fact]
    public void NameDoesNotStartsWithHyphen()
    {
        Assert.Throws<InvalidFormatException>(() => PlanaPluginOption.ValidateName("-name"));
        Assert.Throws<InvalidFormatException>(() => PlanaPluginOption.ValidateName("--name"));
    }
}