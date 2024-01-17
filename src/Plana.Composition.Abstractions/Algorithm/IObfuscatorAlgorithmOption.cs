﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions.Algorithm;

public interface IObfuscatorAlgorithmOption
{
    string Name { get; }

    string Description { get; }

    Func<bool> GetDefaultValue => () => false;

    Type ValueType => typeof(bool);
}