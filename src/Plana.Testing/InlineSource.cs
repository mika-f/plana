// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Extensions;

using Xunit;

namespace Plana.Testing;

public class InlineSource(IDocument? document) : ITestableObject<string>
{
    public Task ToMatchInlineSnapshot(string snapshot)
    {
        if (document == null)
            Assert.Fail("output and/or input is null");

        var s = SyntaxFactory.ParseCompilationUnit(snapshot);

        Assert.Equal(s.ToNormalizedFullString(), document.SyntaxTree.ToNormalizedFullString());

        return Task.CompletedTask;
    }

    public Task HasDiffs()
    {
        if (document == null)
            Assert.Fail("output and/or input is null");

        Assert.NotEqual(document.OriginalSyntaxTree.ToNormalizedFullString(), document.SyntaxTree.ToNormalizedFullString());

        return Task.CompletedTask;
    }

    public Task NoDiffs()
    {
        if (document == null)
            Assert.Fail("output and/or input is null");

        Assert.Equal(document.OriginalSyntaxTree.ToNormalizedFullString(), document.SyntaxTree.ToNormalizedFullString());

        return Task.CompletedTask;
    }

    public async Task<T> GetSyntax<T>() where T : CSharpSyntaxNode
    {
        return await GetSyntax<T>(_ => true);
    }

    public async Task<T> GetSyntax<T>(Func<T, bool> predicate) where T : CSharpSyntaxNode
    {
        var ret = (await GetSyntaxOf<T>()).SingleOrDefault(predicate);
        Assert.NotNull(ret);

        return ret;
    }

    public async Task<T> GetFirstSyntax<T>() where T : CSharpSyntaxNode
    {
        return await GetFirstSyntax<T>(_ => true);
    }


    public async Task<T> GetFirstSyntax<T>(Func<T, bool> predicate) where T : CSharpSyntaxNode
    {
        var ret = (await GetSyntaxOf<T>()).FirstOrDefault(predicate);
        Assert.NotNull(ret);

        return ret;
    }

    private Task<List<T>> GetSyntaxOf<T>() where T : CSharpSyntaxNode
    {
        if (document == null)
            Assert.Fail("output and/or input is null");

        var ret = document.SyntaxTree.GetCompilationUnitRoot().DescendantNodes().OfType<T>().ToList();
        Assert.NotNull(ret);

        return Task.FromResult(ret);
    }
}