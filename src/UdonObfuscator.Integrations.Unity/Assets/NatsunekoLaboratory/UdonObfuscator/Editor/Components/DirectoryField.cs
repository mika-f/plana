// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;

using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;

using UnityEditor;
using UnityEditor.UIElements;

using UnityEngine;
using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components
{
    internal class DirectoryField : Control
    {
        private readonly ObjectField _field;

        public DirectoryInfo Path { get; set; }

        public string Label
        {
            get => _field.label;
            set => _field.label = value;
        }

        public DirectoryField() : base(StyledComponents.Create("0ba11867e8d00b84882b0bade54f787a", "37dbb67b6aa66684a99564da5d186b54"))
        {
            _field = QuerySelector<ObjectField>();
            _field.objectType = typeof(DefaultAsset);
            _field.RegisterValueChangedCallback(OnValueChanged);
        }

        private void OnValueChanged(ChangeEvent<Object> e)
        {
            var obj = e.newValue as DefaultAsset;
            if (obj == null)
                _field.SetValueWithoutNotify(null);

            var path = AssetDatabase.GetAssetPath(obj);

            try
            {
                var attr = File.GetAttributes(path);
                if (!attr.HasFlag(FileAttributes.Directory))
                    _field.SetValueWithoutNotify(null);
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