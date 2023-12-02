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

    internal class Control : VisualElement, IStyledComponents
    {
#if USTYLED
        private static readonly UStyledCompiler UStyled;
#endif
        private readonly StyledComponents _sc;

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
            _sc = sc;

#if USTYLED
            var xaml = LoadAssetByGuid<VisualTreeAsset>(_sc.Uxml);
            var (uxml, uss) = UStyled.CompileAsAsset(xaml);
#else
            var uxml = LoadAssetByGuid<VisualTreeAsset>(sc.Uxml);
            var uss = LoadAssetByGuid<StyleSheet>(sc.Uss);
#endif

            var css = LoadAssetByGuid<StyleSheet>(_sc.Uss);
            if (css != null)
                styleSheets.Add(css);

            styleSheets.Add(uss);
            hierarchy.Add(uxml.CloneTree());
        }

        public string UxmlGuid => _sc.Uxml;

        public string AdditionalUssGuid => _sc.Uss;

        protected static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}