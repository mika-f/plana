// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

using NatsunekoLaboratory.Plana.Models.Abstractions;

namespace NatsunekoLaboratory.Plana.Components.Abstractions
{
    internal interface IClickable
    {
        void AddClickEventHandler(Action<IAsyncCallbackHandler> listener);
    }
}