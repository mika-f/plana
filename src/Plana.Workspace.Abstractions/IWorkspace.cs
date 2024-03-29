﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Workspace.Abstractions;

public interface IWorkspace
{
    string Path { get; }

    Task<IReadOnlyCollection<IProject>> GetProjectsAsync(CancellationToken ct);

    Task<ISolution> ToSolutionAsync(CancellationToken ct);

    Task CommitAsync(CancellationToken ct);

    Task RollbackAsync(CancellationToken ct);
}