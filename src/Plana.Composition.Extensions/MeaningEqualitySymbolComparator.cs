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

    public bool Equals(ISymbol x, ISymbol y)
    {
        if (x.GetType() == y.GetType())
            switch (x)
            {
                case INamespaceSymbol:
                    return x.ToDisplayString(SymbolDisplayFormat) == y.ToDisplayString(SymbolDisplayFormat);

                case IPropertySymbol:
                    return x.ToDisplayString(SymbolDisplayFormat) == y.ToDisplayString(SymbolDisplayFormat);
            }

        return SymbolEqualityComparer.Default.Equals(x, y);
    }

    public int GetHashCode(ISymbol obj)
    {
        switch (obj)
        {
            case INamespaceSymbol:
                return obj.ToDisplayString(SymbolDisplayFormat).GetHashCode();

            case IPropertySymbol:
                return obj.ToDisplayString(SymbolDisplayFormat).GetHashCode();
        }

        return SymbolEqualityComparer.Default.GetHashCode(obj);
    }
}