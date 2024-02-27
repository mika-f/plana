// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Plana.Composition.Extensions;

public static class TypeExtensions
{
    public static INamedTypeSymbol? ToSymbol(this Type t, SemanticModel sm)
    {
        return sm.Compilation.GetTypeByMetadataName(t.FullName ?? throw new InvalidOperationException());
    }
}