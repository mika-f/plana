// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Workspace;

public class PlanaSolution(List<IProject> projects) : ISolution
{
    public IReadOnlyCollection<IProject> Projects { get; } = projects.AsReadOnly();

    public ISourceMap SourceMap { get; }
}