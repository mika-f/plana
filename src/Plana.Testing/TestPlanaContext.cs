// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Enum;

namespace Plana.Testing;

internal class TestPlanaContext : IPlanaPluginRunContext
{
    public ISolution Solution { get; init; }

    public RunKind Kind { get; init; }

    public IPlanaRandom Random { get; init; }

    public IPlanaSecureRandom SecureRandom { get; init; }

    public CancellationToken CancellationToken { get; init; }
}