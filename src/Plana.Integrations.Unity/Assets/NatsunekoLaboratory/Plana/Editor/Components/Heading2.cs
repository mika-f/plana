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
    internal class Heading2 : Control
    {
        private readonly Label _label;

        public string Label
        {
            get => _label.text;
            set => _label.text = value;
        }

        public Heading2() : base(StyledComponents.Create("dc02c2476ce7ab247b7b5129ecbf8ff6", "c0ddd4bb8913dbb43b06844834b56b2e", "26454c007fdbcdd4d90a50668bbab4c4", "230b3917dd7cdf64d9a4562c2d8d6e9d"))
        {
            _label = this.QuerySelector<Label>();
        }

        public new class UxmlFactory : UxmlFactory<Heading2, UxmlTraits> { }

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

                ((Heading2)ve).Label = _label.GetValueFromBag(bag, cc);
            }
        }
    }
}