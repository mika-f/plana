// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using NatsunekoLaboratory.Plana.Components.Abstractions;
using NatsunekoLaboratory.Plana.Extensions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.Plana.Components
{
    internal class Paragraph : Control
    {
        private readonly Label _label;

        public string Text
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Paragraph() : base(StyledComponents.Create("4d93c728735b2a84586976f7c776c7df", "053e1d291c84ef54b940f16f9a8ad722", "ff63739756d70244e99aca784e33eecb", "7b84de402cd0a924b981b471caebb551"))
        {
            _label = this.QuerySelector<Label>();
        }

        public new class UxmlFactory : UxmlFactory<Paragraph, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription { name = "text", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((Paragraph)ve).Text = _text.GetValueFromBag(bag, cc);
            }
        }
    }
}