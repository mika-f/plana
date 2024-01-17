﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Composition.Abstractions.Algorithm;

public interface IObfuscatorAlgorithm
{
    IReadOnlyCollection<IObfuscatorAlgorithmOption> Options { get; }

    string Name { get; }

    void BindParameters(IObfuscatorParameterBinder binder);

    Task ObfuscateAsync(List<IProject> projects, CancellationToken ct);
}