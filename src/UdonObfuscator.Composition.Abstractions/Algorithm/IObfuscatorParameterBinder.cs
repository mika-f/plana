// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions.Algorithm;

public interface IObfuscatorParameterBinder
{
    bool GetValue(IObfuscatorAlgorithmOption option);

    T GetValue<T>(IObfuscatorAlgorithmOption<T> option);
}