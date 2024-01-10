// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;
using NatsunekoLaboratory.UdonObfuscator.Extensions;
using NatsunekoLaboratory.UdonObfuscator.Models;
using NatsunekoLaboratory.UdonObfuscator.Models.Abstractions;

using UnityEditor;

using UnityEngine.UIElements;

using NativeButton = UnityEngine.UIElements.Button;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class Button : Control, IClickable
    {
        private readonly NativeButton _button;
        private readonly List<Action<IAsyncCallbackHandler>> _listeners;

        public string Text
        {
            get => _button.text;
            set => _button.text = value;
        }

        public bool Disabled
        {
            get => !_button.enabledSelf;
            set => _button.SetEnabled(!value);
        }

        public Button() : base(StyledComponents.Create("bdd8ef457ffffa841a24a66347750c70", "87729b99b94bdb54aac2fd66189f38d4", "808e1ba23cecd8f428bad55ef4a8f500", "023ec3d472daecf4386e1fd60b41db61"))
        {
            _listeners = new List<Action<IAsyncCallbackHandler>>();
            _button = this.QuerySelector<NativeButton>();
            _button.clicked += OnButtonClicked;
        }

        public void AddClickEventHandler(Action<IAsyncCallbackHandler> listener)
        {
            _listeners.Add(listener);
        }

        private async void OnButtonClicked()
        {
            try
            {
                EditorUtility.DisplayProgressBar("Executing......", $"execute command{(_listeners.Count > 1 ? "s" : "")}......", 0f);

                foreach (var listener in _listeners)
                {
                    var handler = new AsyncCallbackHandler();
                    listener.Invoke(handler);

                    await handler.WaitForCompleted();
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
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