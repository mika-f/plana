// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components.Abstractions
{
    internal interface IValueChangedEvent<T>
    {
        void AddValueChangedEventListener(Action<ChangeEvent<T>> listener);
    }
}