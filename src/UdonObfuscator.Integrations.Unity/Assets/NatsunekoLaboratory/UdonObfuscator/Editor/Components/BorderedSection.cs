// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class BorderedSection : Section
    {
        public BorderedSection() : base(StyledComponents.Create("2e36091e25bfde9499683ee2ba42f40e", "94fa0b39c08264844a241073e34c3f2a", "8a23a29802c92a04882c55ff1ea7cf01", "cb82dee2f5d109944997637a811681b6")) { }

        public new class UxmlFactory : UxmlFactory<BorderedSection, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _title = new UxmlStringAttributeDescription { name = "title", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((BorderedSection)ve).Title = _title.GetValueFromBag(bag, cc);
            }
        }
    }
}