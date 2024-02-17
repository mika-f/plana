// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Enum;

namespace Plana;

internal class PlanaPluginRunContext(ISolution solution, RunKind kind, IPlanaRandom random, IPlanaSecureRandom sr, CancellationToken ct) : IPlanaPluginRunContext
{
    public ISolution Solution { get; } = solution;

    public RunKind Kind { get; } = kind;

    public IPlanaRandom Random { get; } = random;

    public IPlanaSecureRandom SecureRandom { get; } = sr;

    public CancellationToken CancellationToken { get; } = ct;
}