// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Extensions;
using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameFields_ExternalReference()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-fields");
        await container.RunAsync();

        var implementation = await container.GetSourceByPathAsync("Plana.Composition.RenameSymbols/RenameSymbolsPlugin.cs");
        var reference = await container.GetSourceByPathAsync("Plana.Composition.RenameSymbols.Tests/RenameSymbolsPluginTest.cs");

        // IsEnableClassNameRenaming -> _0x935f5b12
        var declaration = await implementation.GetFirstSyntax<FieldDeclarationSyntax>(w => w.HasModifier(SyntaxKind.InternalKeyword));
        var identifier = declaration.Declaration.Variables[0].Identifier.ToIdentifier();
        Assert.True(declaration.Declaration.Variables[0].Identifier.ToHaveHexadecimalLikeString());

        var m = await reference.GetFirstSyntax<MemberAccessExpressionSyntax>((w, sm) =>
        {
            var parent = w.Parent?.Parent?.Parent;
            if (parent is not InvocationExpressionSyntax invocation)
                return false;

            var ms = sm.GetSymbolInfo(invocation.Expression).Symbol;
            if (ms is not IMethodSymbol method)
                return false;

            if (method.Name != nameof(Assert.False))
                return false;

            return true;
        });

        Assert.Equal(identifier, m.Name.Identifier.ToFullString());
    }

    [Fact]
    public async Task RenameFields_InternalReference()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-fields");
        await container.RunAsync();

        var implementation = await container.GetSourceByTypeAsync(typeof(MeaningEqualitySymbolComparator));

        // SymbolDisplayFormat -> _0xcb375677
        var declaration = await implementation.GetFirstSyntax<FieldDeclarationSyntax>(w => w.HasModifiers(SyntaxKind.PrivateKeyword, SyntaxKind.StaticKeyword, SyntaxKind.ReadOnlyKeyword));
        var identifier = declaration.Declaration.Variables[0].Identifier.ToIdentifier();
        Assert.True(declaration.Declaration.Variables[0].Identifier.ToHaveHexadecimalLikeString());

        var reference = await implementation.GetFirstSyntax<IdentifierNameSyntax>((w, sm) =>
        {
            var parent = w.Parent?.Parent?.Parent;
            if (parent is not InvocationExpressionSyntax invocation)
                return false;

            var ms = sm.GetSymbolInfo(invocation.Expression).Symbol;
            if (ms is not IMethodSymbol method)
                return false;

            if (method.Name != nameof(ISymbol.ToDisplayString))
                return false;

            return true;
        });

        Assert.Equal(identifier, reference.Identifier.ToFullString());
    }
}