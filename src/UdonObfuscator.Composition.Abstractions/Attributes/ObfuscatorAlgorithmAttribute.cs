// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class ObfuscatorAlgorithmAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}