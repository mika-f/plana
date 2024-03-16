// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Plana.Composition.Extensions;

public class MeaningEqualitySymbolComparator : IEqualityComparer<ISymbol>
{
    private static readonly SymbolDisplayFormat SymbolDisplayFormat = new(
        /* globalNamespaceStyle: */
        SymbolDisplayGlobalNamespaceStyle.OmittedAsContaining,
        /* typeQualificationStyle: */
        SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        /* genericsOptions: */
        SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeVariance,
        /* memberOptions: */
        SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeRef | SymbolDisplayMemberOptions.IncludeExplicitInterface,
        /* delegateStyle: */
        default,
        /* extensionMethodStyle: */
        default,
        /* parameterOptions: */
        SymbolDisplayParameterOptions.IncludeOptionalBrackets | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeExtensionThis | SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName,
        /* propertyStyle: */
        SymbolDisplayPropertyStyle.ShowReadWriteDescriptor,
        /* localOptions: */
        SymbolDisplayLocalOptions.IncludeType,
        /* kindOptions: */
        SymbolDisplayKindOptions.IncludeMemberKeyword,
        /* miscellaneousOptions: */
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier
    );

    public static MeaningEqualitySymbolComparator Default => new();

    public bool Equals(ISymbol? x, ISymbol? y)
    {
        if (x == null || y == null)
            return false;

        if (SymbolEqualityComparer.Default.Equals(x, y))
            return true;

        if (x.GetType() == y.GetType())
            switch (x)
            {
                case INamespaceSymbol:
                case INamedTypeSymbol:
                case IPropertySymbol:
                case IMethodSymbol:
                case IFieldSymbol:
                case IParameterSymbol:
                    return x.ToDisplayString(SymbolDisplayFormat) == y.ToDisplayString(SymbolDisplayFormat);
            }

        return false;
    }

    public int GetHashCode(ISymbol obj)
    {
        switch (obj)
        {
            case INamespaceSymbol:
            case INamedTypeSymbol:
            case IPropertySymbol:
            case IMethodSymbol:
            case IFieldSymbol:
                return obj.ToDisplayString(SymbolDisplayFormat).GetHashCode();

            case IParameterSymbol parameter:
            {
                if (parameter.ContainingSymbol is IMethodSymbol m)
                {
                    var a = (m.IsExtensionMethod ? m.ReducedFrom ?? m.OriginalDefinition : m).ToDisplayString(SymbolDisplayFormat);
                    var b = obj.ToDisplayString(SymbolDisplayFormat);

                    return HashCode.Combine(a, b);
                }

                break;
            }
        }

        return SymbolEqualityComparer.Default.GetHashCode(obj);
    }
}