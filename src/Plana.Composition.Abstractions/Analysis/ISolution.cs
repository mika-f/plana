// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions.Analysis;

public interface ISolution
{
    IReadOnlyCollection<IProject> Projects { get; }

    ISourceMap SourceMap { get; }

    Task ApplyChangesAsync(CancellationToken ct);
}