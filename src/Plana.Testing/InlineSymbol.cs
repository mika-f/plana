// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

namespace Plana.Testing;

public class InlineSymbol : ITestableObject<ISymbol>
{
    public Task ToMatchInlineSnapshot(ISymbol snapshot)
    {
        throw new NotImplementedException();
    }
}