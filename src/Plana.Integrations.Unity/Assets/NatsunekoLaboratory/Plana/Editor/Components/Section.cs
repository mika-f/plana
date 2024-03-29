﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;

using NatsunekoLaboratory.Plana.Components.Abstractions;
using NatsunekoLaboratory.Plana.Extensions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.Plana.Components
{
    internal class Section : ContentControl
    {
        private readonly Heading2 _title;

        public string Title
        {
            get => _title.Label;
            set => _title.Label = value;
        }

        public Section() : base(StyledComponents.Create("0ad4cedc831a12241a0bb35110ec49ec", "1c2c6e9b9bf40564a93826a9c2ab16b7", "2db915862cbd0ac4cbfa60a4d0d86852", "96a5852ed6280c6429cbd16ad39e32e2"))
        {
            _title = this.GetElementByName<Heading2>("title");
        }

        protected Section(StyledComponents sc) : base(sc)
        {
            _title = this.GetElementByName<Heading2>("title");
        }

        public new class UxmlFactory : UxmlFactory<Section, UxmlTraits> { }

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

                ((Section)ve).Title = _title.GetValueFromBag(bag, cc);
            }
        }
    }
}