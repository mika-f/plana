// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Workspace;

public class CSharpDocument(Document document) : IDocument
{
    public Guid Id => document.Id.Id;

    public string Name => document.Name;

    public string Path => document.FilePath!;

    public SemanticModel SemanticModel { get; internal set; } = null!;

    public CSharpSyntaxTree SyntaxTree { get; internal set; } = null!;

    public async Task WriteSyntaxTreeAsync(SyntaxTree tree, CancellationToken ct)
    {
        var root = await tree.GetRootAsync(ct);
        document = document.WithSyntaxRoot(root);

        SemanticModel = await document.GetSemanticModelAsync(ct) ?? throw new InvalidOperationException();
        SyntaxTree = (CSharpSyntaxTree)(await document.GetSyntaxTreeAsync(ct) ?? throw new InvalidOperationException());
    }
}