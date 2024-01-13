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

public class SolutionWorkspace(FileInfo sln, ILogger? logger) : IWorkspace
{
    private Solution _solution = null!;
    private MSBuildWorkspace _workspace = null!;

    public string Path => sln.FullName;

    public async Task ActivateWorkspaceAsync(CancellationToken ct)
    {
        logger?.LogDebug("loading workspace as Visual Studio Solution with MSBuild......");

        MSBuildLocator.RegisterDefaults();

        _workspace = MSBuildWorkspace.Create();
        _solution = await _workspace.OpenSolutionAsync(sln.FullName, null, ct);
    }

    public async Task<List<IProject>> GetProjectsAsync(CancellationToken ct)
    {
        var projects = _solution.Projects.Select(WithPreprocessorSymbols).Select(project => new CSharpProject(project, logger)).ToList();
        foreach (var project in projects)
        {
            ct.ThrowIfCancellationRequested();

            await project.InflateDocumentsAsync(ct);
        }

        return projects.Cast<IProject>().ToList();
    }

    private static Project WithPreprocessorSymbols(Project project)
    {
        var options = (CSharpParseOptions?)project.ParseOptions;
        return options == null ? project : project.WithParseOptions(options.WithPreprocessorSymbols("UDONSHARP", "COMPILER_UDONSHARP"));
    }
}