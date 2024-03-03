// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;
using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameProperties()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-properties");
        await container.RunAsync();

        var abstraction = await container.GetSourceByPathAsync("Plana.Composition.Abstractions/IPlanaPluginRunContext.cs");
        var implementation = await container.GetSourceByPathAsync("Plana/PlanaPluginRunContext.cs");
        var reference = await container.GetSourceByPathAsync("Plana.Composition.DisableConsoleOutput/DisableConsoleOutputPlugin.cs");

        // Solution -> _0x4265cf21
        const string identifier = "_0x4265cf21";

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
}