// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.Plana.Components.Abstractions
{
    internal interface IValueChangeNotifiable<T> : IValuable<T>
    {
        void AddValueChangedEventListener(Action<ChangeEvent<T>> listener);
    }
}