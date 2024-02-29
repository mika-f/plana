// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions;

public interface IPlanaPluginOption
{
    string Name { get; }

    string FriendlyName { get; }

    string Description { get; }

    object? DefaultValue { get; }

    Type ValueType { get; }
}