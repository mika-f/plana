// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

namespace NatsunekoLaboratory.UdonObfuscator.Components.Abstractions
{
    internal interface IValueConverter<TValue1, TValue2>
    {
        TValue2 Convert(TValue1 value, Type targetType);

        TValue1 ConvertBack(TValue2 value, Type targetType);
    }
}