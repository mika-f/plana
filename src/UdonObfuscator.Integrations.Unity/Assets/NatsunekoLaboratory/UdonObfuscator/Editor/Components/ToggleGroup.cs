// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;
using NatsunekoLaboratory.UdonObfuscator.Extensions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class ToggleGroup : ContentControl, IValueChangeNotifiable<bool>
    {
        public enum ToggleMode
        {
            EnabledWhenTrue,

            EnabledWhenFalse
        }

        private readonly Checkbox _toggle;

        public string Label
        {
            get => _toggle.Label;
            set => _toggle.Label = value;
        }

        public bool Value
        {
            get => _toggle.Value;
            set => _toggle.Value = value;
        }

        public ToggleMode Mode { get; set; }

        public ToggleGroup() : base(StyledComponents.Create("65842bc485392d54cace7e2bb32e3a3e", "d3ef72825c3a976439f450a2d327004f"))
        {
            _toggle = this.QuerySelector<Checkbox>();
            _toggle.AddValueChangedEventListener(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<bool> e)
        {
            contentContainer.SetEnabled(Mode == ToggleMode.EnabledWhenTrue ? e.newValue : !e.newValue);
        }

        public new class UxmlFactory : UxmlFactory<ToggleGroup, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label" };
            private readonly UxmlEnumAttributeDescription<ToggleMode> _mode = new UxmlEnumAttributeDescription<ToggleMode> { name = "mode", defaultValue = ToggleMode.EnabledWhenTrue };
            private readonly UxmlBoolAttributeDescription _value = new UxmlBoolAttributeDescription { name = "value" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((ToggleGroup)ve).Label = _label.GetValueFromBag(bag, cc);
                ((ToggleGroup)ve).Value = _value.GetValueFromBag(bag, cc);
                ((ToggleGroup)ve).Mode = _mode.GetValueFromBag(bag, cc);
            }
        }

        public void AddValueChangedEventListener(Action<ChangeEvent<bool>> listener)
        {
            _toggle.AddValueChangedEventListener(listener);
        }
    }
}