// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Workspace.Abstractions;

public interface IWorkspace
{
    Task ActivateWorkspaceAsync(CancellationToken ct);
}