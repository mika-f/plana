// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions;

public interface IObfuscatorAlgorithm
{
    IReadOnlyCollection<IObfuscatorAlgorithmOption> Options { get; }

    void BindParameters(IObfuscatorParameterBinder binder);
}