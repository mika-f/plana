// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Security.Cryptography;

using Plana.Composition.Abstractions;

namespace Plana;

public class PlanaSecureRandom : IPlanaSecureRandom
{
    public int GetInt32()
    {
        return RandomNumberGenerator.GetInt32(int.MaxValue);
    }

    public int GetInt32(int min, int max)
    {
        return RandomNumberGenerator.GetInt32(min, max);
    }

    public void Shuffle<T>(Span<T> array)
    {
        RandomNumberGenerator.Shuffle(array);
    }
}