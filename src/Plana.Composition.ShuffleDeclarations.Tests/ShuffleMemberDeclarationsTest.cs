// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics;

using Plana.Testing;

namespace Plana.Composition.ShuffleDeclarations.Tests;

public class ShuffleMemberDeclarationsTest
{
    [Fact]
    public async Task ShuffleWithRandom()
    {
        var container = new PlanaContainer<ShuffleMemberDeclarations>();
        await container.RunAsync("../../../../Plana/Plana.csproj");

        var source = await container.GetSourceByPathAsync("PlanaRandom.cs");

        Debug.WriteLine(source.Source);

        await source.ToMatchInlineSnapshot(@"
// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------
using Plana.Composition.Abstractions;

namespace Plana;
public class PlanaRandom : IPlanaSecureRandom
{
    public int GetInt32()
    {
        return _random.Next();
    }

    public PlanaRandom(int seed)
    {
        _random = new Random(seed);
    }

    public int GetInt32(int min, int max)
    {
        return _random.Next(min, max);
    }

    public PlanaRandom()
    {
        _random = new Random();
    }

    public void Shuffle<T>(Span<T> array)
    {
        _random.Shuffle(array);
    }

    private readonly Random _random;
}".Trim());
    }
}