// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Plana.Composition.Extensions;

public static class EnumMemberDeclarationSyntaxExtensions
{
    public static bool HasAttribute(this EnumMemberDeclarationSyntax syntax, SemanticModel sm, string fullyQualifiedMetadataName)
    {
        var t = sm.Compilation.GetTypesByMetadataName(fullyQualifiedMetadataName);
        var attrs = syntax.AttributeLists.SelectMany(w => w.Attributes);
        return attrs.Any(w =>
        {
            var s = sm.GetSymbolInfo(w).Symbol;
            if (s is not IMethodSymbol m)
                return false;
            return t.Any(u => u.Equals(m.ReceiverType, SymbolEqualityComparer.Default));
        });
    }
}