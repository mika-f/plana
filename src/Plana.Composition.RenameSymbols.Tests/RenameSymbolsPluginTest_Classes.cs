// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;
using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameClasses_Attribute()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var implementation = await container.GetSourceByTypeAsync(typeof(PlanaPluginAttribute));
        var reference = await container.GetSourceByTypeAsync(typeof(RenameSymbolsPlugin));

        // PlanaPluginAttribute -> _0x35add3ac
        const string identifier = "_0x35add3ac";

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

    [Fact]
    public async Task RenameClasses_Class()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var implementation = await container.GetSourceByPathAsync("Plana.Testing/PlanaContainer{T}.cs");
        var reference = await container.GetSourceByPathAsync("Plana.Composition.RenameSymbols.Tests/RenameSymbolsPluginTest_Classes.cs");
        var extends = await container.GetSourceByPathAsync("Plana.Testing/PlanaContainer.cs");

        // PlanaContainer<T> -> _0xdb120989
        const string identifier = "_0xdb120989";

        var a = await implementation.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, a.Identifier.ToString());

        var b = await reference.GetFirstSyntax<ObjectCreationExpressionSyntax>();
        Assert.Equal(identifier, ((b.Type as GenericNameSyntax)?.Identifier).ToString());

        var c = await extends.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, ((c.BaseList?.Types[0].Type as GenericNameSyntax)?.Identifier).ToString());
    }

    [Fact]
    public async Task RenameClasses_Constructor()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var reference = await container.GetSourceByPathAsync("Plana.Composition.Extensions/PlanaPluginOption.cs");

        // PlanaPluginOption -> _0xb93c4da5
        const string identifier = "_0xb93c4da5";

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

        // PlanaContainer -> _0xdb120989
        const string identifier1 = "_0xdb120989";

        var implementation = await container.GetSourceByPathAsync("Plana.Testing/PlanaContainer{T}.cs");
        var a = await implementation.GetFirstSyntax<ClassDeclarationSyntax>();

        Assert.Equal(identifier1, a.Identifier.ToString());

        // RenameSymbolsPlugin -> _0xe2407d3d
        const string identifier2 = "_0xe2407d3d";

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
    public async Task RenameClasses_Interface()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var declaration = await container.GetSourceByTypeAsync(typeof(IPlanaSecureRandom));
        var implementation = await container.GetSourceByTypeAsync(typeof(PlanaSecureRandom));
        var reference = await container.GetSourceByTypeAsync(typeof(CSharpSymbolsWalker));

        // IPlanaSecureRandom -> _0x32fad750
        const string identifier = "_0x32fad750";

        var a = await declaration.GetFirstSyntax<InterfaceDeclarationSyntax>();
        Assert.Equal(identifier, a.Identifier.ToString());

        var b = await implementation.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, b.BaseList?.Types[0].Type.ToString());

        var c = await reference.GetFirstSyntax<ClassDeclarationSyntax>();
        Assert.Equal(identifier, c.ParameterList?.Parameters[2].Type?.ToString());
    }

    [Fact]
    public async Task RenameClasses_Record()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-classes");
        await container.RunAsync();

        var declaration = await container.GetSourceByTypeAsync(typeof(AnnotationComment));
        var reference = await container.GetSourceByTypeAsync(typeof(CSharpSyntaxNodeExtensions));

        // AnnotationComment -> _0xd9d2dd2a
        const string identifier = "_0xd9d2dd2a";

        var a = await declaration.GetFirstSyntax<RecordDeclarationSyntax>();
        Assert.Equal(identifier, a.Identifier.ToString());


        var b = await reference.GetFirstSyntax<MemberAccessExpressionSyntax>(w =>
        {
            if (w.Parent is not ArgumentSyntax arg)
                return false;

            if (arg.Parent is not ArgumentListSyntax args)
                return false;

            return args.Arguments.Count == 1;
        });

        Assert.Equal(identifier, b.Expression.ToString());
    }
}