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
    internal class FileField : Control, IValueChangeNotifiable<FileInfo>
    {
        private readonly ObjectField _field;
        private readonly List<Action<ChangeEvent<FileInfo>>> _listeners;
        private Type _t;
        private FileInfo _value;

        public string Label
        {
            get => _field.label;
            set => _field.label = value;
        }

        public string Filter { get; set; }

        public Type Type
        {
            get => _t;
            set
            {
                if (_t == value)
                    return;

                _field.objectType = value;
                _t = value;
            }
        }

        public FileField() : base(StyledComponents.Create("0ba11867e8d00b84882b0bade54f787a", "37dbb67b6aa66684a99564da5d186b54"))
        {
            _listeners = new List<Action<ChangeEvent<FileInfo>>>();

            _field = this.QuerySelector<ObjectField>();
            _field.RegisterValueChangedCallback(OnFormValueChanged);
        }

        public FileInfo Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                var obj = value == null ? null : AssetDatabase.LoadAssetAtPath(value.ToRelativePath(), Type);
                _field.SetValueWithoutNotify(obj);
                _value = value;
            }
        }

        public void AddValueChangedEventListener(Action<ChangeEvent<FileInfo>> listener)
        {
            _listeners.Add(listener);
        }

        private void OnFormValueChanged(ChangeEvent<Object> e)
        {
            var obj = e.newValue;
            var path = obj == null ? null : AssetDatabase.GetAssetPath(obj);

            try
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    var attr = File.GetAttributes(path);
                    if (attr.IsDirectory())
                        path = null;
                }

                var newValue = path == null ? null : new FileInfo(path);
                var oldValue = _value;

                _field.SetValueWithoutNotify(path == null ? null : obj);
                _value = newValue;

                foreach (var listener in _listeners)
                    listener.Invoke(ChangeEvent<FileInfo>.GetPooled(oldValue, newValue));
            }
            catch
            {
                // ignored
            }
        }

        public new class UxmlFactory : UxmlFactory<FileField, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _filter = new UxmlStringAttributeDescription { name = "filter" };
            private readonly UxmlStringAttributeDescription _label = new UxmlStringAttributeDescription { name = "label", defaultValue = "" };
            private readonly UxmlStringAttributeDescription _type = new UxmlStringAttributeDescription { name = "type", defaultValue = "" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);

                ((FileField)ve).Label = _label.GetValueFromBag(bag, cc);
                ((FileField)ve).Filter = _filter.GetValueFromBag(bag, cc);

                var t = _type.GetValueFromBag(bag, cc);
                ((FileField)ve).Type = Type.GetType(t) ?? typeof(DefaultAsset);
            }
        }
    }
}