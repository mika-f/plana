// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using NatsunekoLaboratory.UdonObfuscator.Components;
using NatsunekoLaboratory.UdonObfuscator.Components.Abstractions;
using NatsunekoLaboratory.UdonObfuscator.Models;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Extensions
{
    // ReSharper disable once InconsistentNaming
    internal static class IValueChangeNotifiableExtensions
    {
        public enum BindingMode
        {
            TwoWay,
            OneWay,
            OneTime,
            OneWaoToSource
        }

        private static readonly Dictionary<Type, Dictionary<string, MemberInfo>> Members = new Dictionary<Type, Dictionary<string, MemberInfo>>();

        public static TElement Binding<TElement, TValue>(this TElement field, INotifyPropertyChanged obj, Expression<Func<TValue>> reference, BindingMode mode = BindingMode.TwoWay) where TElement : IValueChangeNotifiable<TValue>
        {
            var variable = (reference.Body as MemberExpression)?.Member.Name;
            var onRaiseByThis = false;

            if (string.IsNullOrWhiteSpace(variable))
                return field;

            var t = obj.GetType();
            var member = GetMemberInfo(t, variable);

            field.AddValueChangedEventListener(w =>
            {
                if (mode == BindingMode.OneWaoToSource || mode == BindingMode.TwoWay)
                {
                    field.Value = w.newValue;
                    SetReflectedValue(obj, member, w.newValue);
                }

                onRaiseByThis = true;
            });

            obj.PropertyChanged += (sender, e) =>
            {
                if (mode == BindingMode.OneWay || mode == BindingMode.TwoWay)
                    if (variable == e.PropertyName && !onRaiseByThis)
                        field.Value = GetReflectedValue<TValue>(obj, GetCachedMemberInfo(t, variable));

                onRaiseByThis = false;
            };

            if (mode == BindingMode.OneTime)
                field.Value = GetReflectedValue<TValue>(obj, GetCachedMemberInfo(t, variable));

            return field;
        }

        public static TElement Binding<TElement>(this TElement obj, ObservableCollection<VisualElement> collection, BindingMode mode = BindingMode.TwoWay) where TElement : ItemsControl
        {
            collection.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems[0] is VisualElement ve)
                            obj.Items.Insert(e.NewStartingIndex, ve);
                        break;
                    }

                    case NotifyCollectionChangedAction.Move:
                    {
                        var ve = obj.Items[e.OldStartingIndex];
                        obj.Items.Insert(e.NewStartingIndex, ve);
                        obj.Items.RemoveAt(e.OldStartingIndex);
                        break;
                    }

                    case NotifyCollectionChangedAction.Remove:
                        if (e.OldStartingIndex >= 0)
                            obj.Items.RemoveAt(e.OldStartingIndex);
                        break;

                    case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.NewItems[0] is VisualElement ve)
                            obj.Items[e.NewStartingIndex] = ve;
                        break;
                    }

                    case NotifyCollectionChangedAction.Reset:
                        obj.Items.Clear();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            };

            return obj;
        }

        public static TElement Binding<TElement, TFieldValue, TKey, TStoreValue>(this TElement obj, TKey key, ObservableDictionary<TKey, TStoreValue> collection, IValueConverter<TStoreValue, TFieldValue> converter, BindingMode mode = BindingMode.TwoWay)
            where TElement : IValueChangeNotifiable<TFieldValue>
        {
            var onRaiseByThis = false;

            obj.AddValueChangedEventListener(e =>
            {
                if (mode == BindingMode.OneWaoToSource || mode == BindingMode.TwoWay)
                {
                    obj.Value = e.newValue;
                    collection[key] = converter.ConvertBack(e.newValue, typeof(TStoreValue));
                }

                onRaiseByThis = true;
            });

            collection.CollectionChanged += (sender, e) =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                    {
                        if (e.NewItems[0] is KeyValuePair<TKey, TStoreValue> item && key.Equals(item.Key))
                            obj.Value = converter.Convert(item.Value, typeof(TFieldValue));
                        break;
                    }

                    case NotifyCollectionChangedAction.Replace:
                    {
                        if (e.NewItems[0] is KeyValuePair<TKey, TStoreValue> item && key.Equals(item.Key) && !onRaiseByThis)
                            obj.Value = converter.Convert(item.Value, typeof(TFieldValue));
                        break;
                    }

                    case NotifyCollectionChangedAction.Move:
                    case NotifyCollectionChangedAction.Remove:
                    case NotifyCollectionChangedAction.Reset:
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                onRaiseByThis = false;
            };


            return obj;
        }

        private static MemberInfo GetCachedMemberInfo(Type t, string name)
        {
            return Members[t][name];
        }

        private static MemberInfo GetMemberInfo(Type t, string variable)
        {
            if (Members.ContainsKey(t) && Members[t].ContainsKey(variable)) return Members[t][variable];

            var member = t.GetMember(variable, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(w => w is PropertyInfo || w is FieldInfo);
            if (member is PropertyInfo || member is FieldInfo)
            {
                if (Members.TryGetValue(t, out var m))
                {
                    m.Add(variable, member);
                    return member;
                }

                Members.Add(t, new Dictionary<string, MemberInfo>());
                Members[t].Add(variable, member);
                return member;
            }

            throw new InvalidOperationException("unreachable");
        }

        private static T GetReflectedValue<T>(object obj, MemberInfo member)
        {
            if (member is PropertyInfo property)
                return (T)property.GetMethod.Invoke(obj, new object[] { });
            if (member is FieldInfo field)
                return (T)field.GetValue(obj);

            return default;
        }

        private static void SetReflectedValue<T>(object obj, MemberInfo member, T newValue)
        {
            if (member is PropertyInfo property)
                property.SetMethod.Invoke(obj, new object[] { newValue });
            else if (member is FieldInfo field)
                field.SetValue(obj, newValue);
        }
    }
}