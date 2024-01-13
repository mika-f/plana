// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

using NatsunekoLaboratory.Plana.Components.Abstractions;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.Plana.Components
{
    internal class ItemsControl : ContentControl
    {
        public IList<VisualElement> Items => new ItemsControlImpl(Container);

        public ItemsControl() : base(StyledComponents.Create("fa8f32e3c48207645a253b1faef8b409", "447d72a2c34860e40a43b53506195997", "9578bf91344544c4693d1700f74c7887", "78e4c3853c876944eab481eff0bae1dd")) { }

        public new class UxmlFactory : UxmlFactory<ItemsControl, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }

        private class ItemsControlImpl : IList<VisualElement>
        {
            private readonly VisualElement _ve;

            public ItemsControlImpl(VisualElement ve)
            {
                _ve = ve;
            }

            public IEnumerator<VisualElement> GetEnumerator()
            {
                return _ve.Children().GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public void Add(VisualElement item)
            {
                _ve.Add(item);
            }

            public void Clear()
            {
                _ve.Clear();
            }

            public bool Contains(VisualElement item)
            {
                return _ve.Contains(item);
            }

            public void CopyTo(VisualElement[] array, int arrayIndex)
            {
                throw new NotImplementedException();
            }

            public bool Remove(VisualElement item)
            {
                if (_ve.Contains(item))
                {
                    _ve.Remove(item);
                    return true;
                }

                return false;
            }

            public int Count => _ve.childCount;

            public bool IsReadOnly => false;

            public int IndexOf(VisualElement item)
            {
                return _ve.IndexOf(item);
            }

            public void Insert(int index, VisualElement item)
            {
                _ve.Insert(index, item);
            }

            public void RemoveAt(int index)
            {
                _ve.RemoveAt(index);
            }

            public VisualElement this[int index]
            {
                get => _ve[index];
                set => throw new NotSupportedException();
            }
        }
    }
}