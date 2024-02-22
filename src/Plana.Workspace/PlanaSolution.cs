// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Workspace;

internal class PlanaSolution(PlanaWorkspaceContext context, List<IProject> projects) : ISolution
{
    public IReadOnlyCollection<IProject> Projects { get; } = projects.AsReadOnly();

    public ISourceMap SourceMap { get; } = null!;

    public async Task ApplyChangesAsync(CancellationToken ct)
    {
        await context.FlushChangesAsync(ct);
    }
}