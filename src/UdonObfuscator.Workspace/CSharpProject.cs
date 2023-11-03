// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using UdonObfuscator.Composition.Abstractions.Analysis;
using UdonObfuscator.Logging.Abstractions;

namespace UdonObfuscator.Workspace;

public class CSharpProject(Project project, ILogger? logger) : IProject
{
    private readonly List<IDocument> _documents = new();

    public Guid Id => project.Id.Id;

    public string Name => project.Name;

    public IReadOnlyCollection<IDocument> Documents => _documents.AsReadOnly();

    public async Task InflateDocumentsAsync(CancellationToken ct)
    {
        foreach (var document in project.Documents)
        {
            ct.ThrowIfCancellationRequested();

            var sm = await document.GetSemanticModelAsync(ct);
            var tree = await document.GetSyntaxTreeAsync(ct);

            if (tree is CSharpSyntaxTree cs && sm != null)
            {
                var instance = new CSharpDocument(document) { SemanticModel = sm, SyntaxTree = cs };
                _documents.Add(instance);
            }
            else
            {
                if (sm == null)
                    logger?.LogWarning("failed to get SemanticModel for semantic analysis");
                if (tree is not CSharpSyntaxTree)
                    logger?.LogWarning("failed to cast to CSharpSyntaxTree, languages other than C# are not currently supported");
            }
        }
    }
}