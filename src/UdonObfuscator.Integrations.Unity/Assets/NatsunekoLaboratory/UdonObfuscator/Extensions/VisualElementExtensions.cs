// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Extensions
{
    public static class VisualElementExtensions
    {
        public static T GetElementByName<T>(this VisualElement ve, string name) where T : VisualElement
        {
            return GetElementsByName<T>(ve, name).First();
        }

        public static List<T> GetElementsByName<T>(this VisualElement ve, string name) where T : VisualElement
        {
            return ve.Query<T>().Name(name).Build().ToList();
        }

        public static T QuerySelector<T>(this VisualElement ve, string selector = null) where T : VisualElement
        {
            return QuerySelectorAll<T>(ve, selector).FirstOrDefault();
        }

        public static List<T> QuerySelectorAll<T>(this VisualElement ve, string selector = null) where T : VisualElement
        {
            var s = new StringBuilder();
            var o = new StringBuilder();
            var q = ve.Query<T>();

            if (!string.IsNullOrWhiteSpace(selector))
            {
                var sr = new StringReader(selector);
                int c;

                while ((c = sr.Read()) != -1)
                    switch (c)
                    {
                        case ' ':
                        case '>':
                        case '~':
                        case '+':
                        case ':':
                            o.Append((char)c);
                            continue;

                        default:
                            if (o.Length == 0) s.Append((char)c);

                            break;
                    }

                if (s.Length > 0)
                    q = Append(q, s.ToString());
            }

            return q.Build().ToList();
        }

        private static UQueryBuilder<T> Append<T>(UQueryBuilder<T> query, string selector) where T : VisualElement
        {
            if (selector.StartsWith("[") && selector.EndsWith("]"))
            {
                var s = selector.Substring(1, selector.Length - 2).Split('=');
                var key = s[0];
                var value = s[1].Substring(1, s[1].Length - 2);
                var last = key.Last();
                var @operator = "=";

                switch (last)
                {
                    case '*':
                        key = key.Substring(0, key.Length - 1);
                        @operator = "*";
                        break;

                    case '^':
                        key = key.Substring(0, key.Length - 1);
                        @operator = "^";
                        break;

                    case '$':
                        key = key.Substring(0, key.Length - 1);
                        @operator = "$";
                        break;
                }

                switch (key)
                {
                    case "name":
                        return query.Name(value);
                }
            }

            return query;
        }
    }
}