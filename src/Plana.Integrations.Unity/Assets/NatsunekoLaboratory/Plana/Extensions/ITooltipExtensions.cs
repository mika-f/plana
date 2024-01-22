// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using NatsunekoLaboratory.Plana.Components.Abstractions;

namespace NatsunekoLaboratory.Plana.Extensions
{
    // ReSharper disable once InconsistentNaming
    internal static class ITooltipExtensions
    {
        public static T WithTooltip<T>(this T obj, string value) where T : ITooltip
        {
            obj.TooltipValue = value;
            return obj;
        }
    }
}