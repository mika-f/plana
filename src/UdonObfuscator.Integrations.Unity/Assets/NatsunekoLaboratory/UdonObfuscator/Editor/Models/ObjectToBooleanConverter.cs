// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;

namespace NatsunekoLaboratory.UdonObfuscator.Models
{
    internal class ObjectToBooleanConverter : IValueConverter<object, bool>
    {
        public bool Convert(object value, Type targetType)
        {
            return bool.Parse(value.ToString());
        }

        public object ConvertBack(bool value, Type targetType)
        {
            return value;
        }
    }
}