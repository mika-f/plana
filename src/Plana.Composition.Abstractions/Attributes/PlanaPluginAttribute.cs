// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class PlanaPluginAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}