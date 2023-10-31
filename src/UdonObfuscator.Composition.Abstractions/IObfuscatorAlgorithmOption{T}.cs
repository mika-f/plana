// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions;

public interface IObfuscatorAlgorithmOption<out T> : IObfuscatorAlgorithmOption
{
    Func<T> GetDefaultValue { get; }
}