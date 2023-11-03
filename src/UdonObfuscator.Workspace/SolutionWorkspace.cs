// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

using UdonObfuscator.Composition.Abstractions.Analysis;
using UdonObfuscator.Logging.Abstractions;
using UdonObfuscator.Workspace.Abstractions;

namespace UdonObfuscator.Workspace;

public class SolutionWorkspace(FileInfo sln, ILogger? logger) : IWorkspace
{
    private Solution _solution = null!;
    private MSBuildWorkspace _workspace = null!;

    public async Task ActivateWorkspaceAsync(CancellationToken ct)
    {
        logger?.LogDebug("loading workspace as Visual Studio Solution with MSBuild......");

        MSBuildLocator.RegisterDefaults();

        _workspace = MSBuildWorkspace.Create();
        _solution = await _workspace.OpenSolutionAsync(sln.FullName, null, ct);
    }
}