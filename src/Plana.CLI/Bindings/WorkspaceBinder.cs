// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Binding;

using Plana.Logging.Abstractions;
using Plana.Workspace;
using Plana.Workspace.Abstractions;

namespace Plana.CLI.Bindings;

internal class WorkspaceBinder(Option<FileInfo> workspace) : BinderBase<IWorkspace>
{
    protected override IWorkspace GetBoundValue(BindingContext bindingContext)
    {
        return GetWorkspace(bindingContext);
    }

    private IWorkspace GetWorkspace(BindingContext context)
    {
        var path = context.ParseResult.GetValueForOption(workspace)!;
        var logger = (ILogger?)context.GetService(typeof(ILogger));

        return path.Extension == ".sln" ? new SolutionWorkspace(path, logger) : new ProjectWorkspace(path, logger);
    }
}