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
    public async Task RenameClasses_Constructor()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var reference = await container.GetSourceByPathAsync("Plana.Composition.Extensions/PlanaPluginOption.cs");

        // PlanaPluginOption -> _0x4e115ed4
        const string identifier = "_0x4e115ed4";

        var @class = await reference.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, @class.Identifier.ToFullString());

        var constructor = await reference.GetFirstSyntax<ConstructorDeclarationSyntax>();
        Assert.Equal(identifier, constructor.Identifier.ToFullString());
    }
}