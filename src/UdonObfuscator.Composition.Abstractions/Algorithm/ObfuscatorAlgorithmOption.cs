// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions.Algorithm;

public class ObfuscatorAlgorithmOption(string name, string description) : IObfuscatorAlgorithmOption
{
    public string Name { get; } = name;

    public string Description { get; } = description;
}