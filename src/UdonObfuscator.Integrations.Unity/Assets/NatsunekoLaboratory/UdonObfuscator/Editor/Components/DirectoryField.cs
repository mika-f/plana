// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;
using NatsunekoLaboratory.UdonObfuscator.Extensions;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine.UIElements;

using Object = UnityEngine.Object;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class DirectoryField : Control, IValueChangeNotifiable<DirectoryInfo>
    {
        private readonly ObjectField _field;
        private readonly List<Action<ChangeEvent<DirectoryInfo>>> _listeners;
        private DirectoryInfo _value;

        public string Label
        {
            get => _field.label;
            set => _field.label = value;
        }

        public DirectoryField() : base(StyledComponents.Create("0ba11867e8d00b84882b0bade54f787a", "37dbb67b6aa66684a99564da5d186b54"))
        {
            _listeners = new List<Action<ChangeEvent<DirectoryInfo>>>();

            _field = this.QuerySelector<ObjectField>();
            _field.objectType = typeof(DefaultAsset);
            _field.RegisterValueChangedCallback(OnFormValueChanged);
        }

        public DirectoryInfo Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                var obj = value == null ? null : AssetDatabase.LoadAssetAtPath<DefaultAsset>(value.ToRelativePath());
                _field.SetValueWithoutNotify(obj);
                _value = value;
            }
        }

        public void AddValueChangedEventListener(Action<ChangeEvent<DirectoryInfo>> listener)
        {
            _listeners.Add(listener);
        }

        private void OnFormValueChanged(ChangeEvent<Object> e)
        {
            var obj = e.newValue as DefaultAsset;
            var path = obj == null ? null : AssetDatabase.GetAssetPath(obj);

            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var attr = File.GetAttributes(path);
                    if (!attr.HasFlag(FileAttributes.Directory))
                        path = null;
                }

                var newValue = path == null ? null : new DirectoryInfo(path);
                var oldValue = _value;

                _field.SetValueWithoutNotify(path == null ? null : obj);
                _value = newValue;

                foreach (var listener in _listeners)
                    listener.Invoke(ChangeEvent<DirectoryInfo>.GetPooled(oldValue, newValue));
            }
            catch
            {
                // ignored
            }
        }


        public new class UxmlFactory : UxmlFactory<DirectoryField, UxmlTraits> { }

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

                ((DirectoryField)ve).Label = _label.GetValueFromBag(bag, cc);
            }
        }
    }
}