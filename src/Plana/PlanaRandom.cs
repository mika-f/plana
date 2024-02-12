// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;

namespace Plana;

public class PlanaRandom : IPlanaSecureRandom
{
    private readonly Random _random;

    public PlanaRandom(int seed)
    {
        _random = new Random(seed);
    }

    public PlanaRandom()
    {
        _random = new Random();
    }

    public int GetInt32()
    {
        return _random.Next();
    }

    public int GetInt32(int min, int max)
    {
        return _random.Next(min, max);
    }

    public void Shuffle<T>(Span<T> array)
    {
        _random.Shuffle(array);
    }
}