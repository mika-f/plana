// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Enum;

namespace Plana;

internal class PlanaPluginRunContext(ISolution solution, RunKind kind, CancellationToken ct) : IPlanaPluginRunContext
{
    public ISolution Solution { get; } = solution;

    public RunKind Kind { get; } = kind;

    public CancellationToken CancellationToken { get; } = ct;
}