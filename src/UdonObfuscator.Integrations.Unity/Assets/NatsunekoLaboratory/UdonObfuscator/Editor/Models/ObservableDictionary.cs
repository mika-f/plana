// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NatsunekoLaboratory.UdonObfuscator.Models
{
    // poor impl
    internal class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged
    {
        private readonly ObservableCollection<KeyValuePair<TKey, TValue>> _internal = new ObservableCollection<KeyValuePair<TKey, TValue>>();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            if (_internal.Any(w => w.Key.Equals(item.Key)))
                throw new ArgumentException();

            _internal.Add(item);
        }

        public void Clear()
        {
            _internal.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _internal.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var existing = _internal.FirstOrDefault(w => w.Key.Equals(item.Key));
            if (existing.Equals(default(KeyValuePair<TKey, TValue>)))
                return false;

            if (existing.Value.Equals(item))
            {
                _internal.Remove(existing);
                return true;
            }

            return false;
        }

        public int Count => _internal.Count;

        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key)
        {
            return _internal.Any(w => w.Key.Equals(key));
        }

        public bool Remove(TKey key)
        {
            var existing = _internal.FirstOrDefault(w => w.Key.Equals(key));
            if (existing.Equals(default(KeyValuePair<TKey, TValue>)))
                return false;

            _internal.Remove(existing);
            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (ContainsKey(key))
            {
                value = _internal.First(w => w.Key.Equals(key)).Value;
                return true;
            }

            value = default;
            return false;
        }

        public TValue this[TKey key]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        public ICollection<TKey> Keys => _internal.Select(w => w.Key).ToList();

        public ICollection<TValue> Values => _internal.Select(w => w.Value).ToList();

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _internal.CollectionChanged += value;
            remove => _internal.CollectionChanged -= value;
        }
    }
}