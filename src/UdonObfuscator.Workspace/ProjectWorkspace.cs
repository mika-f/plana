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

public class ProjectWorkspace(FileInfo csproj, ILogger? logger) : IWorkspace
{
    private Project _project = null!;
    private MSBuildWorkspace _workspace = null!;

    public async Task ActivateWorkspaceAsync(CancellationToken ct)
    {
        logger?.LogDebug("loading workspace as Visual Studio C# Project with MSBuild......");

        MSBuildLocator.RegisterDefaults();

        _workspace = MSBuildWorkspace.Create();
        _project = await _workspace.OpenProjectAsync(csproj.FullName, null, ct);
    }
}