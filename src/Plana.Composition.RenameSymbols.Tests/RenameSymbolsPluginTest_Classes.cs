// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;
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

        // PlanaPluginOption -> _0xb35682f5
        const string identifier = "_0xb35682f5";

        var @class = await reference.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, @class.Identifier.ToFullString());

        var constructor = await reference.GetFirstSyntax<ConstructorDeclarationSyntax>();
        Assert.Equal(identifier, constructor.Identifier.ToFullString());
    }

    [Fact]
    public async Task RenameClasses_Attribute()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var implementation = await container.GetSourceByTypeAsync(typeof(PlanaPluginAttribute));
        var reference = await container.GetSourceByTypeAsync(typeof(RenameSymbolsPlugin));

        // PlanaPluginAttribute -> _0xb73384b5
        const string identifier = "_0xb73384b5";

        var declaration = await implementation.GetFirstSyntax<ClassDeclarationSyntax>((@class, sm) =>
        {
            var symbol = sm.GetDeclaredSymbol(@class);
            return symbol?.BaseType != null && symbol.BaseType.Equals(typeof(Attribute).ToSymbol(sm), SymbolEqualityComparer.Default);
        });

        Assert.Equal($"{identifier}Attribute", declaration.Identifier.ToFullString());

        var attribute = await reference.GetFirstSyntax<AttributeSyntax>((w, sm) =>
        {
            if (w.Parent is not AttributeListSyntax attributes)
                return false;

            if (attributes.Target?.Identifier.IsKind(SyntaxKind.AssemblyKeyword) == true)
                return false;

            return true;
        });
        Assert.Equal(identifier, attribute.Name.ToFullString());
    }
}