// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Enum;

namespace Plana.Composition.Abstractions;

public interface IPlanaPluginRunContext
{
    ISolution Solution { get; }

    RunKind Kind { get; }

    IPlanaRandom Random { get; }

    IPlanaSecureRandom SecureRandom { get; }

    CancellationToken CancellationToken { get; }
}