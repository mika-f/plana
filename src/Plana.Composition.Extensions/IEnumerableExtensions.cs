// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Extensions;

// ReSharper disable once InconsistentNaming
public static class IEnumerableExtensions
{
    public static bool None<T>(this IEnumerable<T> obj, Func<T, bool> predicate)
    {
        return !obj.Any(predicate);
    }

    public static bool None<T>(this IEnumerable<T> obj)
    {
        return !obj.Any();
    }
}