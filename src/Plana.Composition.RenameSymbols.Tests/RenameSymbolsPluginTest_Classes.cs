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

        // PlanaPluginOption -> _0xcb375677
        const string identifier = "_0xcb375677";

        var @class = await reference.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, @class.Identifier.ToFullString());

        var constructor = await reference.GetFirstSyntax<ConstructorDeclarationSyntax>();
        Assert.Equal(identifier, constructor.Identifier.ToFullString());
    }

    [Fact]
    public async Task RenameClasses_HasTypeParameters()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var reference = await container.GetSourceByPathAsync("Plana.Composition.RenameSymbols.Tests/RenameSymbolsPluginTest_Classes.cs");

        // PlanaContainer -> _0x403629f8
        const string identifier1 = "_0x403629f8";

        var implementation = await container.GetSourceByPathAsync("Plana.Testing/PlanaContainer{T}.cs");
        var a = await implementation.GetFirstSyntax<ClassDeclarationSyntax>();

        Assert.Equal(identifier1, a.Identifier.ToString());

        // RenameSymbolsPlugin -> _0x409e98f6
        const string identifier2 = "_0x409e98f6";

        var parameter = await container.GetSourceByTypeAsync(typeof(RenameSymbolsPlugin));
        var b = await parameter.GetFirstSyntax<ClassDeclarationSyntax>();

        Assert.Equal(identifier2, b.Identifier.ToString());

        var invocation = await reference.GetFirstSyntax<ObjectCreationExpressionSyntax>(w => w.Type.IsKind(SyntaxKind.GenericName));
        var generics = invocation.Type as GenericNameSyntax;

        Assert.NotNull(generics);
        Assert.Equal(identifier1, generics.Identifier.ToString());
        Assert.Equal(identifier2, generics.TypeArgumentList.Arguments[0].ToString());
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