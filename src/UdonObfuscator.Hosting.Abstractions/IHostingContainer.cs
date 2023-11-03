// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UdonObfuscator.Composition.Abstractions.Algorithm;
using UdonObfuscator.Composition.Abstractions.Attributes;

namespace UdonObfuscator.Hosting.Abstractions;

public interface IHostingContainer
{
    IReadOnlyCollection<(IObfuscatorAlgorithm, ObfuscatorAlgorithmAttribute)> Items { get; }

    Task ResolveAsync(CancellationToken ct);
}