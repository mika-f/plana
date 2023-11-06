﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Workspace.Abstractions;

public interface IWorkspace
{
    string Path { get; }

    Task ActivateWorkspaceAsync(CancellationToken ct);

    Task<List<IProject>> GetProjectsAsync(CancellationToken ct);
}