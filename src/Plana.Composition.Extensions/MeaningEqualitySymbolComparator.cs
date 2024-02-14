// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Plana.Composition.Extensions;

public class MeaningEqualitySymbolComparator : IEqualityComparer<ISymbol>
{
    public static MeaningEqualitySymbolComparator Default => new();

    public bool Equals(ISymbol x, ISymbol y)
    {
        if (x.GetType() == y.GetType())
            switch (x)
            {
                case INamespaceSymbol:
                    return x.ToDisplayString() == y.ToDisplayString();
            }

        return SymbolEqualityComparer.Default.Equals(x, y);
    }

    public int GetHashCode(ISymbol obj)
    {
        switch (obj)
        {
            case INamespaceSymbol:
                return obj.ToDisplayString().GetHashCode();
        }

        return SymbolEqualityComparer.Default.GetHashCode(obj);
    }
}