﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions.Analysis;
using Plana.Logging.Abstractions;

namespace Plana.Workspace;

public class CSharpProject(Project project, ILogger? logger) : IProject
{
    private readonly List<IDocument> _documents = new();

    public Guid Id => project.Id.Id;

    public string Name => project.Name;

    public IReadOnlyCollection<IDocument> Documents => _documents.AsReadOnly();

    public async Task InflateDocumentsAsync(CancellationToken ct)
    {
        var path = new Uri(Path.GetDirectoryName(project.FilePath)!);

        foreach (var document in project.Documents)
        {
            ct.ThrowIfCancellationRequested();

            if (document.FilePath != null && path.IsBaseOf(new Uri(document.FilePath)))
            {
                var sm = await document.GetSemanticModelAsync(ct);
                var tree = await document.GetSyntaxTreeAsync(ct);

                if (tree is CSharpSyntaxTree cs && sm != null)
                {
                    var instance = new CSharpDocument(document, cs) { SemanticModel = sm };
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
            else
            {
                logger?.LogWarning($"the file {document.FilePath} will ignored because located outside of project");
            }
        }
    }
}