// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions.Algorithm;

public class ObfuscatorAlgorithmOption<T>(string name, string description, Func<T> getDefaultValue) : IObfuscatorAlgorithmOption<T>
{
    public string Name { get; } = name;

    public string Description { get; } = description;

    public Func<T> GetDefaultValue { get; } = getDefaultValue;
}