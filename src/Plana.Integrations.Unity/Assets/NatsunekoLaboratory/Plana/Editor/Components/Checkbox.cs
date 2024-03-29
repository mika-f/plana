﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using NatsunekoLaboratory.Plana.Components.Abstractions;
using NatsunekoLaboratory.Plana.Extensions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.Plana.Components
{
    internal class Checkbox : Control, ITooltip, IValueChangeNotifiable<bool>
    {
        private readonly List<Action<ChangeEvent<bool>>> _listeners;
        private readonly Toggle _toggle;

        public string Label
        {
            get => _toggle.label;
            set => _toggle.label = value;
        }

        public string Text
        {
            get => _toggle.text;
            set => _toggle.text = value;
        }


        public Checkbox() : base(StyledComponents.Create("84651a7aeb61342438ed3151794dd07c", "9ad74141476795e4293466c737e8f6aa", "f7767e6d222c9e4489cdbc01fac94ae6", "0047e4fbe8c9cdd4788ff73418ab3b76"))
        {
            _listeners = new List<Action<ChangeEvent<bool>>>();
            _toggle = this.QuerySelector<Toggle>();
            _toggle.RegisterValueChangedCallback(OnValueChanged);
        }

        public string TooltipValue
        {
            get => _toggle.tooltip;
            set => _toggle.tooltip = value;
        }

        public bool Value
        {
            get => _toggle.value;
            set => _toggle.value = value;
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
            private readonly UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription { name = "text", defaultValue = "" };
            private readonly UxmlBoolAttributeDescription _value = new UxmlBoolAttributeDescription { name = "value", defaultValue = false };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((Checkbox)ve).Label = _label.GetValueFromBag(bag, cc);
                ((Checkbox)ve).Text = _text.GetValueFromBag(bag, cc);
                ((Checkbox)ve).Value = _value.GetValueFromBag(bag, cc);
            }
        }
    }
}