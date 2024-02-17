// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;

namespace Plana.Testing;

public class InlinePlanaPlugin : IPlanaPlugin
{
    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption>().AsReadOnly();

    public string Name => nameof(InlinePlanaPlugin);

    public void BindParameters(IPlanaPluginParameterBinder binder) { }

    public Task RunAsync(IPlanaPluginRunContext context)
    {
        return Task.CompletedTask;
    }
}