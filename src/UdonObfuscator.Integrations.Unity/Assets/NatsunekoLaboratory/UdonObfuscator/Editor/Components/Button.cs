// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;
using NatsunekoLaboratory.UdonObfuscator.Extensions;

using UnityEngine.UIElements;

using NativeButton = UnityEngine.UIElements.Button;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class Button : Control, IClickable
    {
        private readonly NativeButton _button;
        private readonly List<Action> _listeners;

        public string Text
        {
            get => _button.text;
            set => _button.text = value;
        }

        public Button() : base(StyledComponents.Create("bdd8ef457ffffa841a24a66347750c70", "87729b99b94bdb54aac2fd66189f38d4"))
        {
            _listeners = new List<Action>();
            _button = this.QuerySelector<NativeButton>();
            _button.clicked += OnButtonClicked;
        }

        public void AddClickEventHandler(Action listener)
        {
            _listeners.Add(listener);
        }

        private void OnButtonClicked()
        {
            foreach (var listener in _listeners)
                listener.Invoke();
        }

        public new class UxmlFactory : UxmlFactory<Button, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _text = new UxmlStringAttributeDescription { name = "text" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((Button)ve).Text = _text.GetValueFromBag(bag, cc);
            }
        }
    }
}