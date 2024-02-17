// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Text.RegularExpressions;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Exceptions;

namespace Plana.Composition.Extensions;

public class PlanaPluginOption : IPlanaPluginOption
{
    private static readonly Regex NameValidator = new("^[a-zA-Z0-9][a-zA-Z0-9_-]+$", RegexOptions.Compiled);

    public PlanaPluginOption(string name, string description, bool defaultValue) : this(name, name, description, defaultValue) { }

    public PlanaPluginOption(string name, string friendlyName, string description, bool defaultValue)
    {
        ValidateName(name);

        Name = name;
        FriendlyName = friendlyName;
        Description = description;
        DefaultValue = defaultValue;
    }

    public string Name { get; }

    public string FriendlyName { get; }

    public string Description { get; }

    public bool DefaultValue { get; }

    public static void ValidateName(string name)
    {
        if (NameValidator.IsMatch(name))
            return;

        throw new InvalidFormatException(nameof(name));
    }
}