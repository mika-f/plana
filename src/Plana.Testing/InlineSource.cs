// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Xunit;

namespace Plana.Testing;

public class InlineSource(string path, string? output, string? input) : ITestableObject<string>
{
    private CSharpSyntaxNode? _i;
    private CSharpSyntaxNode? _o;

    public string Source => output;

    public Task ToMatchInlineSnapshot(string snapshot)
    {
        if (output == null)
            Assert.Fail("output and/or input is null");

        _o = SyntaxFactory.ParseCompilationUnit(output);
        var s = SyntaxFactory.ParseCompilationUnit(snapshot);

        Assert.Equal(s.ToNormalizedFullString(), _o.ToNormalizedFullString());

        return Task.CompletedTask;
    }

    public Task HasDiffs()
    {
        if (output == null || input == null)
            Assert.Fail("output and/or input is null");

        Assert.NotEqual(document.OriginalSyntaxTree.ToNormalizedFullString(), document.SyntaxTree.ToNormalizedFullString());

        return Task.CompletedTask;
    }

    public Task NoDiffs()
    {
        if (output == null || input == null)
            Assert.Fail("output and/or input is null");

        _o = SyntaxFactory.ParseCompilationUnit(output);
        _i = SyntaxFactory.ParseCompilationUnit(input);

        Assert.Equal(_i.ToNormalizedFullString(), _o.ToNormalizedFullString());

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

    public async Task<T> GetSyntax<T>(Func<T, SemanticModel, bool> predicate) where T : CSharpSyntaxNode
    {
        var ret = (await GetSyntaxOf<T>()).SingleOrDefault(w => predicate(w, document!.SemanticModel));
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

    public async Task<T> GetFirstSyntax<T>(Func<T, SemanticModel, bool> predicate) where T : CSharpSyntaxNode
    {
        var ret = (await GetSyntaxOf<T>()).FirstOrDefault(w => predicate(w, document!.SemanticModel));
        Assert.NotNull(ret);

        return ret;
    }

    private Task<List<T>> GetSyntaxOf<T>() where T : CSharpSyntaxNode
    {
        if (output == null)
            Assert.Fail("output and/or input is null");

        _o ??= SyntaxFactory.ParseCompilationUnit(output);

        var ret = _o.DescendantNodes().OfType<T>().ToList();
        Assert.NotNull(ret);

        return Task.FromResult(ret);
    }
}