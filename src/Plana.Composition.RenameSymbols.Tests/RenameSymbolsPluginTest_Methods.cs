// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Extensions;
using Plana.Testing;
using Plana.Workspace;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameMethods_ExtensionMethods()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-methods");
        await container.RunAsync();

        var originalDefinition = await container.GetSourceByPathAsync("Plana.CLI/Extensions/CommandExtensions.cs");
        var reference = await container.GetSourceByPathAsync("Plana.CLI/Commands/ObfuscateCommand.cs");

        // CommandExtensions.AddOptions -> _0x4e115ed4
        const string addOptionsIdentifier = "_0x4e115ed4";

        var def = await originalDefinition.GetFirstSyntax<MethodDeclarationSyntax>((w, sm) =>
        {
            if (w.HasNotModifier(SyntaxKind.StaticKeyword))
                return false;

            var param1 = sm.GetDeclaredSymbol(w.ParameterList.Parameters[0]);
            var param2 = sm.GetDeclaredSymbol(w.ParameterList.Parameters[1]);

            return param1?.Type.ToDisplayString() == "System.CommandLine.Command" && param2?.Type.ToDisplayString() == "System.CommandLine.Option[]";
        });
        Assert.Equal(addOptionsIdentifier, def.Identifier.ToString());

        var r = await reference.GetFirstSyntax<InvocationExpressionSyntax>((w, sm) =>
        {
            var expression = w.Expression;
            if (expression is not MemberAccessExpressionSyntax access)
                return false;

            var receiver = access.Expression;
            var si1 = sm.GetSymbolInfo(receiver);
            if (si1.Symbol is not IPropertySymbol p)
                return false;

            return p.Type.ToDisplayString() == "System.CommandLine.Command";
        });
        Assert.Equal($"Command.{addOptionsIdentifier}", r.Expression.ToFullString().Trim());
    }

    [Fact]
    public async Task RenameMethods_ExternalInterfaceMethods()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-methods");
        await container.RunAsync();

        var reference = await container.GetSourceByPathAsync("Plana.Composition.Extensions/MeaningEqualitySymbolComparator.cs");

        bool IsMethodsIsEqualsSignature(MethodDeclarationSyntax w, SemanticModel sm)
        {
            if (w.ParameterList.Parameters.Count != 2)
                return false;

            var x = w.ParameterList.Parameters[0].Type;
            var y = w.ParameterList.Parameters[1].Type;
            if (x == null || y == null)
                return false;

            var xSymbol = sm.GetSymbolInfo(x).Symbol;
            var ySymbol = sm.GetSymbolInfo(y).Symbol;
            if (xSymbol is not INamedTypeSymbol || ySymbol is not INamedTypeSymbol)
                return false;

            return xSymbol.Equals(typeof(ISymbol).ToSymbol(sm), SymbolEqualityComparer.Default) && ySymbol.Equals(typeof(ISymbol).ToSymbol(sm), SymbolEqualityComparer.Default);
        }

        bool IsMethodsIsGetHashCodeSignature(MethodDeclarationSyntax w, SemanticModel sm)
        {
            if (w.ParameterList.Parameters.Count != 1)
                return false;

            var identifier = w.ParameterList.Parameters[0].Type;
            if (identifier == null)
                return false;

            var si = sm.GetSymbolInfo(identifier);
            if (si.Symbol is not INamedTypeSymbol param)
                return false;

            return param.Equals(typeof(ISymbol).ToSymbol(sm), SymbolEqualityComparer.Default);
        }


        var equals = await reference.GetFirstSyntax<MethodDeclarationSyntax>(IsMethodsIsEqualsSignature);
        Assert.Equal(nameof(MeaningEqualitySymbolComparator.Equals), equals.Identifier.ToString());

        var getHashCode = await reference.GetFirstSyntax<MethodDeclarationSyntax>(IsMethodsIsGetHashCodeSignature);
        Assert.Equal(nameof(MeaningEqualitySymbolComparator.GetHashCode), getHashCode.Identifier.ToString());
    }

    [Fact]
    public async Task RenameMethods_InterfaceMethods()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-methods");
        await container.RunAsync();

        var rootAbstraction = await container.GetSourceByPathAsync("Plana.Composition.Abstractions/IPlanaPlugin.cs");
        var inheritAbstraction = await container.GetSourceByPathAsync("Plana.Composition.Extensions/IPlanaPlugin2.cs");
        var implementation = await container.GetSourceByPathAsync("Plana.Composition.RenameSymbols/RenameSymbolsPlugin.cs");
        var reference = await container.GetSourceByPathAsync("Plana/Obfuscator.cs");

        // IPlanaPlugin.RunAsync -> _0xa79a150d
        const string runAsyncIdentifier = "_0xa79a150d";

        bool IsMethodsHasRunAsyncSignature(MethodDeclarationSyntax w, SemanticModel sm)
        {
            var identifier = w.ParameterList.Parameters[0].Type;
            if (identifier == null)
                return false;

            var si = sm.GetSymbolInfo(identifier);
            if (si.Symbol is not INamedTypeSymbol param)
                return false;

            return param.Equals(typeof(IPlanaPluginRunContext).ToSymbol(sm), SymbolEqualityComparer.Default);
        }

        var rootAbstractionDecl = await rootAbstraction.GetFirstSyntax<MethodDeclarationSyntax>(IsMethodsHasRunAsyncSignature);
        Assert.Equal(runAsyncIdentifier, rootAbstractionDecl.Identifier.ToString());

        var inheritAbstractionDecl = await inheritAbstraction.GetFirstSyntax<MethodDeclarationSyntax>(IsMethodsHasRunAsyncSignature);
        Assert.Equal(runAsyncIdentifier, inheritAbstractionDecl.Identifier.ToString());

        var referenceDecl = await reference.GetFirstSyntax<InvocationExpressionSyntax>((w, sm) =>
        {
            var expression = w.Expression;
            if (expression is not MemberAccessExpressionSyntax access)
                return false;

            var receiver = access.Expression;
            var si1 = sm.GetSymbolInfo(receiver);
            if (si1.Symbol is not ILocalSymbol local1)
                return false;

            var param = w.ArgumentList.Arguments[0];
            var si2 = sm.GetSymbolInfo(param.Expression);
            if (si2.Symbol is not ILocalSymbol local2)
                return false;

            return local1.Type.Equals(typeof(IPlanaPlugin).ToSymbol(sm), SymbolEqualityComparer.Default) && local2.Type.Interfaces[0].Equals(typeof(IPlanaPluginRunContext).ToSymbol(sm), SymbolEqualityComparer.Default);
        });
        Assert.Equal(runAsyncIdentifier, ((MemberAccessExpressionSyntax)referenceDecl.Expression).Name.Identifier.ToString());

        // IPlanaPlugin2.ObfuscateAsync -> _0xba881b8e
        const string obfuscateAsyncIdentifier = "_0xba881b8e";

        var inheritAbstractionDecl2 = (await inheritAbstraction.GetSyntaxList<MethodDeclarationSyntax>(IsMethodsHasRunAsyncSignature))[1];
        Assert.Equal(obfuscateAsyncIdentifier, inheritAbstractionDecl2.Identifier.ToString());

        var implementationDecl = await implementation.GetFirstSyntax<MethodDeclarationSyntax>(IsMethodsHasRunAsyncSignature);
        Assert.Equal(obfuscateAsyncIdentifier, implementationDecl.Identifier.ToString());
    }

    [Fact]
    public async Task RenameMethods_GenericsInterfaceMethods()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-methods");
        await container.RunAsync();


        var @interface = await container.GetSourceByPathAsync("Plana.Testing/ITestableObject.cs");
        var implementation = await container.GetSourceByTypeAsync(typeof(InlineSource));

        // ITestableObject<T>.ToMatchInlineSnapshot(T) -> _0x204ecb6f
        const string identifier = "_0x204ecb6f";

        var decl = await @interface.GetFirstSyntax<MethodDeclarationSyntax>();
        Assert.Equal(identifier, decl.Identifier.ToString());

        var impl = await implementation.GetFirstSyntax<MethodDeclarationSyntax>();
        Assert.Equal(identifier, impl.Identifier.ToString());
    }

    [Fact]
    public async Task RenameMethods_GenericsMethods()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-methods");
        await container.RunAsync();

        var implementation = await container.GetSourceByTypeAsync(typeof(InlineSource));

        // InlineSource.GetSyntaxOf<T> -> _0x2b70476f
        const string identifier = "_0x2b70476f";

        var def = await implementation.GetFirstSyntax<MethodDeclarationSyntax>((w, sm) =>
        {
            if (w.ParameterList.Parameters.Count != 0)
                return false;

            if (w.ConstraintClauses.None())
                return false;

            var ret = w.ReturnType;
            var symbol = sm.GetSymbolInfo(ret).Symbol;
            if (symbol is not INamedTypeSymbol t)
                return false;

            if (t.TypeArguments.Length != 1)
                return false;

            if (t.TypeArguments[0] is not INamedTypeSymbol list)
                return false;

            return true;
        });

        Assert.Equal(identifier, def.Identifier.ToString());

        var reference = await implementation.GetFirstSyntax<InvocationExpressionSyntax>((w, sm) =>
        {
            if (w.Parent is not AwaitExpressionSyntax awaiter)
                return false;

            if (awaiter.Parent is ReturnStatementSyntax)
                return false;

            if (awaiter.Parent?.Parent is not MemberAccessExpressionSyntax inv)
                return false;

            if (w.Expression is MemberAccessExpressionSyntax)
                return false;

            return inv.Name.ToString() == "FirstOrDefault";
        });

        Assert.Equal($"{identifier}<T>", reference.Expression.ToString());
    }

    [Fact]
    public async Task RenameMethods_OriginalDefinitionMethods()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-methods");
        await container.RunAsync();

        var originalDefinition = await container.GetSourceByPathAsync("Plana.Workspace/SolutionWorkspace.cs");
        var reference = await container.GetSourceByPathAsync("Plana.CLI/Commands/ObfuscateCommand.cs");

        // SolutionWorkspace.CreateWorkspaceAsync -> _0xc686c7a5
        const string createWorkspaceAsyncIdentifier = "_0xc686c7a5";

        var def = await originalDefinition.GetFirstSyntax<MethodDeclarationSyntax>(w => w.HasModifier(SyntaxKind.StaticKeyword));
        Assert.Equal(createWorkspaceAsyncIdentifier, def.Identifier.ToString());

        var r = await reference.GetFirstSyntax<InvocationExpressionSyntax>((w, sm) =>
        {
            var expression = w.Expression;
            if (expression is not MemberAccessExpressionSyntax access)
                return false;

            var receiver = access.Expression;
            var si1 = sm.GetSymbolInfo(receiver);
            if (si1.Symbol is not ITypeSymbol t)
                return false;

            return t.Equals(typeof(SolutionWorkspace).ToSymbol(sm), SymbolEqualityComparer.Default);
        });
        Assert.Equal($"SolutionWorkspace.{createWorkspaceAsyncIdentifier}", r.Expression.ToFullString());
    }
}