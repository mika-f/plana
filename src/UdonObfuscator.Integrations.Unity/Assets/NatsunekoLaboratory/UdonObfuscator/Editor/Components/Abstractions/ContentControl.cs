// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components.Abstractions
{
    internal class ContentControl : Control
    {
        protected ContentControl(StyledComponents sc) : base(sc)
        {
            contentContainer = QuerySelector<VisualElement>("[name='container']");
        }

        public override VisualElement contentContainer { get; }
    }
}