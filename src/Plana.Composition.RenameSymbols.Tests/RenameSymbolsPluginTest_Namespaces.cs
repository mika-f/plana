// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameNamespacesInProjectWorkspace()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-namespaces");
        await container.RunAsync("../../../../Plana.CLI/Plana.CLI.csproj");

        var source = await container.GetSourceByPathAsync("Commands/ObfuscateCommand.cs");

        // Plana.CLI.Commands
        var @namespace = await source.GetFirstSyntax<FileScopedNamespaceDeclarationSyntax>();
        var parts = @namespace.Name.ToIdentifier().Split(".");

        foreach (var part in parts)
            Assert.True(part.ToHaveHexadecimalLikeString());

        var usings = (await source.GetSyntaxList<UsingDirectiveSyntax>()).Select(w => w.ToNormalizedTrimmedFullString()).ToList();

        // Plana -> Plana
        Assert.Contains("using Plana;", usings);

        // Plana.Composition.Abstractions -> Plana.Composition.Abstractions
        Assert.Contains("using Plana.Composition.Abstractions;", usings);

        // Plana.CLI.xxx -> parts[0].parts[1].unknown
        Assert.True(usings.Where(w => w.StartsWith($"{parts[0]}.{parts[1]}")).All(w => w.Split(".")[2].ToHaveHexadecimalLikeString()));
    }

    [Fact]
    public async Task RenameNamespacesInSolutionWorkspace()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-namespaces");
        await container.RunAsync();

        var root = await container.GetSourceByPathAsync("Plana/PlanaRandom.cs"); // Plana (Plana.dll)
        var nest = await container.GetSourceByPathAsync("Plana.Composition.Abstractions/IPlanaSecureRandom.cs"); // Plana.Composition.Abstractions (Plana.Composition.Abstractions.dll)

        // Plana -> _0xb73384b5
        var rootDecl = await root.GetSyntax<FileScopedNamespaceDeclarationSyntax>();
        var a = rootDecl.Name.ToIdentifier();
        Assert.True(a.ToHaveHexadecimalLikeString());

        // Plana.Composition.Abstractions -> _0xb73384b5._0x23636295._0x895054f0
        var abstractionsDecl = await nest.GetSyntax<FileScopedNamespaceDeclarationSyntax>();
        var parts1 = abstractionsDecl.Name.ToIdentifier().Split(".");

        Assert.Equal(a, parts1[0]);

        var b = parts1[1];
        Assert.True(b.ToHaveHexadecimalLikeString());
        Assert.True(parts1[2].ToHaveHexadecimalLikeString());

        var usingToAbstractions = await root.GetFirstSyntax<UsingDirectiveSyntax>();
        var parts2 = usingToAbstractions.Name!.ToIdentifier().Split(".");

        Assert.Equal(a, parts2[0]);
        Assert.Equal(b, parts2[1]);
        Assert.True(parts2[2].ToHaveHexadecimalLikeString());
    }
}