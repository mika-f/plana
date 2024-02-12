// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Abstractions;

public interface IPlanaRandom
{
    int GetInt32();

    int GetInt32(int min, int max);

    void Shuffle<T>(Span<T> array);
}