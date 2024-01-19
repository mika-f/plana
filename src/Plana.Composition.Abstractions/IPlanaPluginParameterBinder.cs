// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions;

public interface IPlanaPluginParameterBinder
{
    bool GetValue(IPlanaPluginOption option);

    T GetValue<T>(IPlanaPluginOption<T> option);
}