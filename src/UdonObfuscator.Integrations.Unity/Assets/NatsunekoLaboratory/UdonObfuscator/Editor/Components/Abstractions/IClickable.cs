// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

using NatsunekoLaboratory.UdonObfuscator.Models.Abstractions;

namespace NatsunekoLaboratory.UdonObfuscator.Components.Abstractions
{
    internal interface IClickable
    {
        void AddClickEventHandler(Action<IAsyncCallbackHandler> listener);
    }
}