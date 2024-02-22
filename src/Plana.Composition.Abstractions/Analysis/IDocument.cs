// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Plana.Composition.Abstractions.Analysis;

public interface IDocument
{
    Guid Id { get; }

    string Name { get; }

    string Path { get; }

    SemanticModel SemanticModel { get; }

    CSharpSyntaxTree SyntaxTree { get; }

    CSharpSyntaxTree OriginalSyntaxTree { get; }

    Task ApplyChangesAsync(SyntaxNode node, CancellationToken ct);

    Task ApplyChangesWithoutNotificationAsync(SyntaxNode node, CancellationToken ct);
}