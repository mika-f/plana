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
    internal class Heading1 : Control
    {
        private readonly Label _label;

        public string Label
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Heading1() : base(StyledComponents.Create("46a2346e020f9f14c9c42386cce8defc", "67a1b812bae7e1e46be014d9a7a3cb2e", "0a8f463a1852af742bc2c7335fe1a763", "f8b6c7cdea0e10f4e93ef0d0158978d3"))
        {
            _label = this.QuerySelector<Label>();
        }

        public new class UxmlFactory : UxmlFactory<Heading1, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((Heading1)ve).Label = _label.GetValueFromBag(bag, cc);
            }
        }
    }
}