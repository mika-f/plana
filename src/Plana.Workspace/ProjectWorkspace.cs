// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

using Plana.Composition.Abstractions.Analysis;
using Plana.Logging.Abstractions;
using Plana.Workspace.Abstractions;

namespace Plana.Workspace;

public class ProjectWorkspace(FileInfo csproj, ILogger? logger) : IWorkspace
{
    private Project _project = null!;
    private MSBuildWorkspace _workspace = null!;

    public string Path => csproj.FullName;

    public async Task ActivateWorkspaceAsync(CancellationToken ct)
    {
        logger?.LogDebug("loading workspace as Visual Studio C# Project with MSBuild......");

        try
        {
            MSBuildLocator.RegisterDefaults();
        }
        catch (InvalidOperationException)
        {
            // ignored
        }

        _workspace = MSBuildWorkspace.Create();

        var project = await _workspace.OpenProjectAsync(csproj.FullName, null, ct);
        var options = (CSharpParseOptions?)project.ParseOptions;

        _project = options == null ? project : project.WithParseOptions(options.WithPreprocessorSymbols("UDONSHARP", "COMPILER_UDONSHARP"));
    }

    public async Task<List<IProject>> GetProjectsAsync(CancellationToken ct)
    {
        var projects = new List<CSharpProject> { new(_project, logger) };
        foreach (var project in projects)
        {
            ct.ThrowIfCancellationRequested();

            await project.InflateDocumentsAsync(ct);
        }

        return projects.Cast<IProject>().ToList();
    }
}