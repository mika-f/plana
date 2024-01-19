// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Plana.Composition.Abstractions.Analysis;

public interface ISourceMap
{
    void Register(ISymbol from, ISymbol to);

    void Update(ISymbol from, ISymbol to);

    ISymbol Get(ISymbol from);

    ISymbol ReverseGet(ISymbol to);
}