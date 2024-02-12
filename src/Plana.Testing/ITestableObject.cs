// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Testing;

public interface ITestableObject<in T>
{
    Task ToMatchInlineSnapshot(T snapshot);
}