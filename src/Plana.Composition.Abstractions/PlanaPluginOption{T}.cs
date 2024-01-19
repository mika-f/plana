﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions;

public class PlanaPluginOption<T> : IPlanaPluginOption<T>
{
    public PlanaPluginOption(string name, string description, T defaultValue) : this(name, name, description, defaultValue) { }

    public PlanaPluginOption(string name, string friendlyName, string description, T defaultValue)
    {
        PlanaPluginOption.ValidateName(name);

        Name = name;
        FriendlyName = friendlyName;
        Description = description;
        DefaultValue = defaultValue;
    }

    public string Name { get; }

    public string FriendlyName { get; }

    public string Description { get; }

    public T DefaultValue { get; }
}