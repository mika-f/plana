// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class Heading3 : Control
    {
        private readonly Label _label;

        public string Label
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Heading3() : base(StyledComponents.Create("39cbb5ba539624f48a3264911213e425", "4fb1ad4fee4c8174db981c61b8290023"))
        {
            _label = QuerySelector<Label>();
        }

        public new class UxmlFactory : UxmlFactory<Heading3, UxmlTraits> { }

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

                ((Heading3)ve).Label = _label.GetValueFromBag(bag, cc);
            }
        }
    }
}