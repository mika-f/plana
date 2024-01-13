// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace NatsunekoLaboratory.Plana.Components.Abstractions
{
    internal interface IValuable<T>
    {
        T Value { get; set; }
    }
}