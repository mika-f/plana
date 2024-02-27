// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;
using Plana.Testing;
using Plana.Workspace;

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
        var reference = await container.GetSourceByPathAsync("Plana.Composition.DisableConsoleOutput/DisableConsoleOutputPlugin.cs");

        // Solution -> _0xc7b29ba1
        const string identifier = "_0xc7b29ba1";

        var solutionAbstractDecl = await abstraction.GetFirstSyntax<PropertyDeclarationSyntax>();
        Assert.Equal(identifier, solutionAbstractDecl.Identifier.ToString());

        var solutionImplDecl = await implementation.GetFirstSyntax<PropertyDeclarationSyntax>();
        Assert.Equal(identifier, solutionImplDecl.Identifier.ToString());

        bool IsAccessToContextSolution(MemberAccessExpressionSyntax syntax, SemanticModel sm)
        {
            if (syntax.Expression is not IdentifierNameSyntax receiver)
                return false;

            // context is local parameter and IPlanaPluginRunContext
            var context = sm.GetSymbolInfo(receiver).Symbol;
            if (context is not IParameterSymbol s1)
                return false;

            var a = s1.Type.Equals(typeof(IPlanaPluginRunContext).ToSymbol(sm), SymbolEqualityComparer.Default);

            // accessed property is IPlanaSolution
            var ccc = sm.GetSymbolInfo(syntax.Name);
            if (ccc.Symbol is not IPropertySymbol s2)
                return false;

            var b = s2.Type.Equals(typeof(ISolution).ToSymbol(sm), SymbolEqualityComparer.Default);

            return a && b;
        }

        var solutionRef = await reference.GetFirstSyntax<MemberAccessExpressionSyntax>(IsAccessToContextSolution);
        Assert.Equal(identifier, solutionRef.Name.ToString());
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
}