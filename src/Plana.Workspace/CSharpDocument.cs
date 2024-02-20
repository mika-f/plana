// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------


using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions.Analysis;

namespace Plana.Workspace;

public class CSharpDocument : IDocument
{
    private readonly PlanaWorkspaceContext _context;
    private readonly Document _document;

    public DocumentId RawId => _document.Id;

    private CSharpDocument(PlanaWorkspaceContext context, Document document, CSharpSyntaxTree tree)
    {
        _context = context;
        _document = document;
        SyntaxTree = tree;
    }

    public Guid Id => _document.Id.Id;

    public string Name => _document.Name;

    public string Path => _document.FilePath!;

    // mutable
    public SemanticModel SemanticModel { get; private set; } = null!;

    // mutable
    public CSharpSyntaxTree SyntaxTree { get; private set; }

    public CSharpSyntaxTree OriginalSyntaxTree { get; private init; } = null!;

    public async Task ApplyChangesAsync(SyntaxNode node, CancellationToken ct)
    {
        var document = await _context.WriteChangesToDocumentAsync(RawId, node, ct);

        SyntaxTree = ((CSharpSyntaxTree?)await document.GetSyntaxTreeAsync(ct))!;
        SemanticModel = (await document.GetSemanticModelAsync(ct))!;
    }

    internal static async Task<IDocument> CreateDocumentAsync(PlanaWorkspaceContext context, Document document, CancellationToken ct)
    {
        var tree = (CSharpSyntaxTree?)await document.GetSyntaxTreeAsync(ct);
        var sm = await document.GetSemanticModelAsync(ct);

        if (context.TryRegisterOriginalDocument(document.Id, tree!, out var original))
            return new CSharpDocument(context, document, tree!) { SemanticModel = sm!, OriginalSyntaxTree = original };

        return new CSharpDocument(context, document, tree!) { SemanticModel = sm!, OriginalSyntaxTree = tree! };
    }
}