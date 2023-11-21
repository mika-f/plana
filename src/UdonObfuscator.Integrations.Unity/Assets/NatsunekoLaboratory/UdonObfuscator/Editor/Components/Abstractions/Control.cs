// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

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
            var css = LoadAssetByGuid<StyleSheet>(sc.Uss);
            var (uxml, uss) = UStyled.CompileAsAsset(xaml);
#else
            var uxml = LoadAssetByGuid<VisualTreeAsset>(sc.Uxml);
            var uss = LoadAssetByGuid<StyleSheet>(sc.Uss);
#endif

            if (css != null)
                styleSheets.Add(css);

            styleSheets.Add(uss);
            hierarchy.Add(uxml.CloneTree());
        }

        protected static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}