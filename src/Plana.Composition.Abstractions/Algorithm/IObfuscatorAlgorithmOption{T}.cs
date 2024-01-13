// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions.Algorithm;

public interface IObfuscatorAlgorithmOption<out T> : IObfuscatorAlgorithmOption
{
    new Func<T> GetDefaultValue { get; }

    new Type ValueType => typeof(T);
}