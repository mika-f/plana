// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;

namespace Plana;

public class PlanaRandom : IPlanaSecureRandom
{
    private readonly Random _random;
    private readonly List<string> _items = [];

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

    public string GetString(char[] chars, int length)
    {
        var str = Enumerable.Repeat(chars, length)
                            .Select(s => s[_random.Next(s.Length)])
                            .ToArray();

        return new string(str);
    }

    public string GetAlphaNumericalString(int length)
    {
        return GetString("abcedf0123456789".ToCharArray(), length);
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
        _random.Shuffle(array);
    }
}