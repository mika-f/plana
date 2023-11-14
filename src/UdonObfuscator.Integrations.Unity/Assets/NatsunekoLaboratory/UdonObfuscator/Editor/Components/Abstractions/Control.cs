// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator.Components.Abstractions
{
#if USTYLED
    using UStyled;
    using UStyled.Configurations;
    using UStyled.Configurations.Presets;
#endif

    internal class Control : VisualElement
    {
#if USTYLED
        private static readonly UStyledCompiler UStyled;
#endif

        static Control()
        {
#if USTYLED
            UStyled = new UStyledCompilerBuilder()
                      .UsePreprocessor(UStyledPreprocessor.SerializedValue)
                      .UsePresets(new StylifyPreset())
                      .Build();
#endif
        }

        protected Control(StyledComponents sc)
        {
#if USTYLED
            var xaml = LoadAssetByGuid<VisualTreeAsset>(sc.Uxml);
            var (uxml, uss) = UStyled.CompileAsAsset(xaml);
#else
            var uxml = LoadAssetByGuid<VisualTreeAsset>(sc.Uxml);
            var uss = LoadAssetByGuid<StyleSheet>(sc.Uss);
#endif

            styleSheets.Add(uss);
            hierarchy.Add(uxml.CloneTree());
        }

        protected static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        #region Web Compats

        protected T QuerySelector<T>(string selector = null) where T : VisualElement
        {
            return QuerySelectorAll<T>(selector).FirstOrDefault();
        }

        protected List<T> QuerySelectorAll<T>(string selector = null) where T : VisualElement
        {
            var s = new StringBuilder();
            var o = new StringBuilder();
            var q = this.Query<T>();

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
                            if (o.Length == 0)
                            {
                                s.Append((char)c);
                            }

                            break;
                    }

                if (s.Length > 0)
                    q = Append(q, s.ToString());
            }

            return q.Build().ToList();
        }

        private UQueryBuilder<T> Append<T>(UQueryBuilder<T> query, string selector) where T : VisualElement
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

        #endregion
    }
}