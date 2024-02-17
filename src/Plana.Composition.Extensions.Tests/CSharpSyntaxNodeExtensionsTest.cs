// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

namespace Plana.Composition.Extensions.Tests;

public class CSharpSyntaxNodeExtensionsTest
{
    [Fact]
    public void HasAnnotationCommentReturnTrueWhenSyntaxHasAnnotationComment()
    {
        var source = SyntaxFactory.ParseCompilationUnit("/* plana:disable */ namespace TestNamespace {}");
        var @namespace = source.Members[0];

        Assert.True(@namespace.HasAnnotationComment());
    }

    [Fact]
    public void HasAnnotationCommentReturnFalseWhenSyntaxHasNotAnnotationComment()
    {
        var source = SyntaxFactory.ParseCompilationUnit("namespace TestNamespace {}");
        var @namespace = source.Members[0];

        Assert.False(@namespace.HasAnnotationComment());
    }
}