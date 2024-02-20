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

public class SolutionWorkspace : IWorkspace
{
    private readonly ILogger? _logger;
    private readonly PlanaWorkspaceContext _context;

    private SolutionWorkspace(PlanaWorkspaceContext context, ILogger? logger)
    {
        _context = context;
        _logger = logger;
    }

    public string Path => _context.FullName;


    public async Task<IReadOnlyCollection<IProject>> GetProjectsAsync(CancellationToken ct)
    {
        return await _context.GetProjectsAsync(ct);
    }

    public static async Task<SolutionWorkspace> CreateWorkspaceAsync(FileInfo sln, ILogger? logger, CancellationToken ct)
    {
        logger?.LogDebug("loading workspace as Visual Studio Solution with MSBuild......");

        try
        {
            MSBuildLocator.RegisterDefaults();
        }
        catch (InvalidOperationException)
        {
            //ignored
        }

        var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(sln.FullName, null, ct);
        var context = new PlanaWorkspaceContext(workspace, solution, logger);

        return new SolutionWorkspace(context, logger);
    }

    public async Task<ISolution> ToSolutionAsync(CancellationToken ct)
    {
        var projects = await GetProjectsAsync(ct);
        return new PlanaSolution([.. projects]);
    }
}