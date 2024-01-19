// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions;

public interface IPlanaPluginOption<out T> : IPlanaPluginOption
{
    new T DefaultValue { get; }

    new Type ValueType => typeof(T);
}