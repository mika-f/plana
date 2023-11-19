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
    internal class Checkbox : Control, IValueChangedEvent<bool>
    {
        private readonly List<Action<ChangeEvent<bool>>> _listeners;
        private readonly Toggle _toggle;

        public string Label
        {
            get => _toggle.label;
            set => _toggle.label = value;
        }

        public bool Value
        {
            get => _toggle.value;
            set => _toggle.value = value;
        }


        public Checkbox() : base(StyledComponents.Create("84651a7aeb61342438ed3151794dd07c", "9ad74141476795e4293466c737e8f6aa"))
        {
            _listeners = new List<Action<ChangeEvent<bool>>>();
            _toggle = this.QuerySelector<Toggle>();
            _toggle.RegisterValueChangedCallback(OnValueChanged);
        }

        public void AddValueChangedEventListener(Action<ChangeEvent<bool>> listener)
        {
            _listeners.Add(listener);
        }

        private void OnValueChanged(ChangeEvent<bool> e)
        {
            foreach (var listener in _listeners)
                listener.Invoke(e);
        }

        public new class UxmlFactory : UxmlFactory<Checkbox, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label", defaultValue = "" };
            private readonly UxmlBoolAttributeDescription _value = new UxmlBoolAttributeDescription { name = "value", defaultValue = false };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((Checkbox)ve).Label = _label.GetValueFromBag(bag, cc);
                ((Checkbox)ve).Value = _value.GetValueFromBag(bag, cc);
            }
        }
    }
}