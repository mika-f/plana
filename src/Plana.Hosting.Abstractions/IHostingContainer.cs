﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;

namespace Plana.Hosting.Abstractions;

public interface IHostingContainer
{
    IReadOnlyCollection<(IPlanaPlugin, PlanaPluginAttribute)> Items { get; }

    Task ResolveAsync(CancellationToken ct);
}