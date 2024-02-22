// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

using Plana.Composition.Abstractions.Analysis;
using Plana.Logging.Abstractions;
using Plana.Workspace.Abstractions;

namespace Plana.Workspace;

public class ProjectWorkspace : IWorkspace
{
    private readonly ILogger? _logger;
    private readonly PlanaWorkspaceContext _context;

    public string Path => _context.FullName;

    private ProjectWorkspace(PlanaWorkspaceContext context, ILogger? logger)
    {
        _context = context;
        _logger = logger;
    }

    public static async Task<ProjectWorkspace> CreateWorkspaceAsync(FileInfo csproj, ILogger? logger, CancellationToken ct)
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

        var workspace = MSBuildWorkspace.Create();
        var project = await workspace.OpenProjectAsync(csproj.FullName, null, ct);
        var context = new PlanaWorkspaceContext(workspace, project, logger);

        return new ProjectWorkspace(context, logger);
    }

    public async Task<IReadOnlyCollection<IProject>> GetProjectsAsync(CancellationToken ct)
    {
        return await _context.GetProjectsAsync(ct);
    }

    public async Task<ISolution> ToSolutionAsync(CancellationToken ct)
    {
        var projects = await GetProjectsAsync(ct);
        return new PlanaSolution(_context, [.. projects]);
    }

    public async Task CommitAsync(CancellationToken ct)
    {
        // throw new NotImplementedException();
    }

    public async Task RollbackAsync(CancellationToken ct)
    {
        // throw new NotImplementedException();
    }
}