// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameNamespacesWithNesting()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-namespaces");
        await container.RunAsync();

        var root = await container.GetSourceByPathAsync("Plana/PlanaRandom.cs"); // Plana

        var a = await root.GetSyntax<FileScopedNamespaceDeclarationSyntax>();
        Assert.Equal("_0xa79a150d", a.Name.ToFullString()); // Plana -> _0xc3feae14

    }
}