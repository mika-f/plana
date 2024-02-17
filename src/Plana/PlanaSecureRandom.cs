// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Security.Cryptography;

using Plana.Composition.Abstractions;

namespace Plana;

public class PlanaSecureRandom : IPlanaSecureRandom
{
    private readonly List<string> _items = [];

    public int GetInt32()
    {
        return RandomNumberGenerator.GetInt32(int.MaxValue);
    }

    public int GetInt32(int min, int max)
    {
        return RandomNumberGenerator.GetInt32(min, max);
    }

    public string GetString(char[] chars, int length)
    {
        return RandomNumberGenerator.GetString(chars, length);
    }

    public string GetAlphaNumericalString(int length)
    {
        var hex = "abcedf0123456789".ToCharArray();
        return GetString(hex, length);
    }

    public string GetGlobalUniqueAlphaNumericalString(int length)
    {
        var identifier = GetAlphaNumericalString(length);

        while (_items.Contains(identifier))
            identifier = GetAlphaNumericalString(length);

        _items.Add(identifier);

        return identifier;
    }

    public void Shuffle<T>(Span<T> array)
    {
        RandomNumberGenerator.Shuffle(array);
    }
}