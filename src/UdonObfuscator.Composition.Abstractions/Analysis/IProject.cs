// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace UdonObfuscator.Composition.Abstractions.Analysis;

public interface IProject
{
    Guid Id { get; }

    string Name { get; }
}