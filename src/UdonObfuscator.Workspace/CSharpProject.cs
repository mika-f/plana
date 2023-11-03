// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using UdonObfuscator.Composition.Abstractions.Analysis;

namespace UdonObfuscator.Workspace;

public class CSharpProject(Project project) : IProject
{
    public Guid Id => project.Id.Id;

    public string Name => project.Name;
}