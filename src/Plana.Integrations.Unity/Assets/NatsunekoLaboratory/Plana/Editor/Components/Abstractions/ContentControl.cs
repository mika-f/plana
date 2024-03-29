﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using NatsunekoLaboratory.Plana.Extensions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.Plana.Components.Abstractions
{
    internal class ContentControl : Control
    {
        public override VisualElement contentContainer { get; }

        public VisualElement Container => contentContainer;

        protected ContentControl(StyledComponents sc) : base(sc)
        {
            contentContainer = this.QuerySelector<VisualElement>("[name='container']");
        }
    }
}