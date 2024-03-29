// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp.Syntax;

using Plana.Composition.Extensions;
using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task RenameVariables_ForEachStatement()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-variables");
        await container.RunAsync();

        var implementation = await container.GetSourceByTypeAsync(typeof(Obfuscator));

        // instance -> _0xc22a5edb
        var @foreach = await implementation.GetFirstSyntax<ForEachStatementSyntax>();
        Assert.True(@foreach.Identifier.ToHaveHexadecimalLikeString());
    }

    [Fact]
    public async Task RenameVariables_PrimaryConstructor()
    {
        // for test purpose
        _ = nameof(AnnotationComment.Annotation);

        var container = new PlanaContainer<RenameSymbolsPlugin>("rename-variables");
        await container.RunAsync();

        var implementation = await container.GetSourceByTypeAsync(typeof(AnnotationComment));

        // AnnotationComment.Annotation -> _0xf35395eb

        var declaration = await implementation.GetFirstSyntax<RecordDeclarationSyntax>();
        var constructor = declaration.ParameterList!.Parameters[0];
        var identifier = constructor.Identifier.ToIdentifier();
        Assert.True(identifier.ToHaveHexadecimalLikeString());

        var internalReference = await implementation.GetFirstSyntax<IdentifierNameSyntax>(w =>
        {
            var parent = w.Parent?.Parent;
            if (parent is not InterpolatedStringExpressionSyntax)
                return false;

            var decl = parent.Parent?.Parent;
            if (decl is not PropertyDeclarationSyntax p)
                return false;

            return p.Identifier.ToFullString().Trim() == nameof(AnnotationComment.Comment);
        });
        Assert.Equal(identifier, internalReference.Identifier.ToIdentifier());

        var externalReferenceImplementation = await container.GetSourceByPathAsync("Plana.Composition.RenameSymbols.Tests/RenameSymbolsPluginTest_Variables.cs");
        var externalReference = await externalReferenceImplementation.GetFirstSyntax<MemberAccessExpressionSyntax>(w =>
        {
            var unknown = w.Parent?.Parent?.Parent?.Parent;
            if (unknown is not AssignmentExpressionSyntax assignment)
                return false;

            return assignment.Left.ToString() == "_";
        });

        Assert.Equal(identifier, externalReference.Name.ToIdentifier());
    }
}