// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions;

public interface IPlanaPlugin
{
    IReadOnlyCollection<IPlanaPluginOption> Options { get; }

    string Name { get; }

    void BindParameters(IPlanaPluginParameterBinder binder);

    Task RunAsync(IPlanaPluginRunContext context);
}