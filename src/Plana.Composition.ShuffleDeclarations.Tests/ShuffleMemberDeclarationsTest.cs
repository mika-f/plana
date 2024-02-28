// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Testing;

namespace Plana.Composition.ShuffleDeclarations.Tests;

public class ShuffleMemberDeclarationsTest
{
    [Fact]
    public async Task CanInstantiate()
    {
        var container = new PlanaContainer<ShuffleMemberDeclarations>();
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Shuffle Declarations", instance.Name);
        Assert.Empty(instance.Options);
    }

    [Fact]
    public async Task ShuffleWithRandom()
    {
        var container = new PlanaContainer<ShuffleMemberDeclarations>();
        await container.RunAsync("../../../../Plana/Plana.csproj");

        var source = await container.GetSourceByPathAsync("PlanaRandom.cs");

        await source.HasDiffs();

        await source.ToMatchInlineSnapshot(@"
// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------
using Plana.Composition.Abstractions;

namespace Plana;
public class PlanaRandom : IPlanaSecureRandom
{
    public string GetString(char[] chars, int length)
    {
        var str = Enumerable.Repeat(chars, length).Select(s => s[_random.Next(s.Length)]).ToArray();
        return new string (str);
    }

    public PlanaRandom(int seed)
    {
        _random = new Random(seed);
    }

    public string GetAlphaNumericalString(int length)
    {
        return GetString(""abcedf0123456789"".ToCharArray(), length);
    }

    private readonly Random _random;
    private readonly List<string> _items = [];
    public void Shuffle<T>(Span<T> array)
    {
        _random.Shuffle(array);
    }

    public string GetGlobalUniqueAlphaNumericalString(int length)
    {
        var identifier = GetAlphaNumericalString(length);
        while (_items.Contains(identifier))
            identifier = GetAlphaNumericalString(length);
        _items.Add(identifier);
        return identifier;
    }

    public int GetInt32(int min, int max)
    {
        return _random.Next(min, max);
    }

    public int GetInt32()
    {
        return _random.Next();
    }

    public PlanaRandom()
    {
        _random = new Random();
    }
}
".Trim());
    }

    [Fact]
    public async Task RemoveRegionAndEndregionPreprocessors()
    {
        var container = new PlanaContainer<ShuffleMemberDeclarations>();
        await container.RunAsync("../../../../Plana.Composition.RenameSymbols/Plana.Composition.RenameSymbols.csproj");

        var source = await container.GetSourceByPathAsync("CSharpSymbolsWalker.cs");

        await source.HasDiffs();

        Assert.False(await source.ContainsAsync("#region"));
        Assert.False(await source.ContainsAsync("#endregion"));
    }

    [Fact]
    public async Task RemovePragmaDisableAndRestorePreprocessors()
    {
        var container = new PlanaContainer<ShuffleMemberDeclarations>();
        await container.RunAsync("../../../../Plana.Testing/Plana.Testing.csproj");

        var source = await container.GetSourceByPathAsync("PlanaContainer{T}.cs");

        await source.HasDiffs();

        Assert.False(await source.ContainsAsync("#pragma warning disable"));
        Assert.False(await source.ContainsAsync("#pragma warning restore"));
    }
}