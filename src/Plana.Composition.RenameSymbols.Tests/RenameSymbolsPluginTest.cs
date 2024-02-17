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
    public async Task CanInstantiateWithDefaults()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>();
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Rename Symbols", instance.Name);
        Assert.Equal(8, instance.Options.Count);

        Assert.False(instance.IsEnableClassNameRenaming);
        Assert.False(instance.IsEnableFieldsRenaming);
        Assert.False(instance.IsEnableMethodsRenaming);
        Assert.False(instance.IsEnableNamespaceRenaming);
        Assert.False(instance.IsEnablePropertiesRenaming);
        Assert.False(instance.IsEnableVariablesRenaming);
        Assert.False(instance.KeepOriginalNameInInspector);
        Assert.False(instance.KeepOriginalNameWithSendCustomEvent);
    }

    [Fact]
    public async Task RenameNamespaces()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-namespaces");
        await container.RunAsync();

        var root = await container.GetSourceByPathAsync("Plana/PlanaRandom.cs"); // Plana (Plana.dll)
        var nest = await container.GetSourceByPathAsync("Plana.Composition.Abstractions/IPlanaSecureRandom.cs"); // Plana.Composition.Abstractions (Plana.Composition.Abstractions.dll)

        // Plana -> _0xb73384b5
        var rootDecl = await root.GetSyntax<FileScopedNamespaceDeclarationSyntax>();
        Assert.Equal("_0xb73384b5", rootDecl.Name.ToFullString());

        // Plana.Composition.Abstractions -> _0xb73384b5._0x23636295._0x895054f0
        var abstractionsDecl = await nest.GetSyntax<FileScopedNamespaceDeclarationSyntax>();
        Assert.Equal("_0xb73384b5._0x23636295._0x895054f0", abstractionsDecl.Name.ToFullString());

        var usingToAbstractions = await root.GetSyntax<UsingDirectiveSyntax>();
        Assert.Equal("_0xb73384b5._0x23636295._0x895054f0", usingToAbstractions.Name!.ToFullString());
    }

    [Fact]
    public async Task RenameProperties()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-properties");
        await container.RunAsync();

        var abstraction = await container.GetSourceByPathAsync("Plana.Composition.Abstractions/IPlanaPluginRunContext.cs");
        var implementation = await container.GetSourceByPathAsync("Plana/PlanaPluginRunContext.cs");
        var reference1 = await container.GetSourceByPathAsync("Plana.Composition.DisableConsoleOutput/DisableConsoleOutputPlugin.cs");
        var reference2 = await container.GetSourceByPathAsync("Plana/Obfuscator.cs");

        // Solution -> _0xc7b29ba1
        const string identifier = "_0xc7b29ba1";

        var solutionAbstractDecl = await abstraction.GetFirstSyntax<PropertyDeclarationSyntax>();
        Assert.Equal(identifier, solutionAbstractDecl.Identifier.ToString());

        var solutionImplDecl = await implementation.GetFirstSyntax<PropertyDeclarationSyntax>();
        Assert.Equal(identifier, solutionImplDecl.Identifier.ToString());

        /*
        var solutionRef1 = await reference1.GetSyntax<MemberAccessExpressionSyntax>();

        var solutionRef2 = await reference2.GetSyntax<MemberAccessExpressionSyntax>();
        */
    }
}