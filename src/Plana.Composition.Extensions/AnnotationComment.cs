// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Composition.Extensions;

public readonly record struct AnnotationComment(string Annotation)
{
    public string Comment => $"/* plana:{Annotation} */";

    public static AnnotationComment DefaultAnnotationComment => new("disable");
}