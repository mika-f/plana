// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using NatsunekoLaboratory.UdonObfuscator.Components;
using NatsunekoLaboratory.UdonObfuscator.Extensions;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace NatsunekoLaboratory.UdonObfuscator
{
#if USTYLED
    using UStyled;
    using UStyled.Configurations;
    using UStyled.Configurations.Presets;
#endif

    public class UdonObfuscatorEditor : EditorWindow
    public class UdonObfuscatorEditor : EditorWindow, INotifyPropertyChanged
    {
        private const string UxmlGuid = "512581237e5c880478d0c1a9d8a40ef5";
        private const string UssGuid = "b84460955893c6149baab61b8cf213a1";
        private const string Plugins = "aa64d32311d16b64384036813c06488a";
#if USTYLED
        private static readonly UStyledCompiler UStyled;
#endif

        private SerializedObject _so;

#if USTYLED
        static UdonObfuscatorEditor()
        {
            UStyled = new UStyledCompilerBuilder()
                      .UsePreprocessor(UStyledPreprocessor.SerializedValue)
                      .UsePresets(new StylifyPreset())
                      .Build();
        }
#endif

        public event PropertyChangedEventHandler PropertyChanged;

        [MenuItem("Window/Natsuneko Laboratory/Udon Obfuscator")]
        public static void ShowWindow()
        {
            var window = GetWindow<UdonObfuscatorEditor>("Udon Obfuscator");
            window.Show();
        }

        private static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        // ReSharper disable once InconsistentNaming
        public void CreateGUI()
        {
            _so = new SerializedObject(this);
            _so.Update();

            var xaml = LoadAssetByGuid<VisualTreeAsset>(UxmlGuid);

#if USTYLED
            var (uxml, uss) = UStyled.CompileAsAsset(xaml);
#else
            var uxml = LoadAssetByGuid<VisualTreeAsset>(UxmlGuid);
            var uss = LoadAssetByGuid<StyleSheet>(UssGuid);
#endif
            rootVisualElement.styleSheets.Add(uss);

            var tree = uxml.CloneTree();
            rootVisualElement.Add(tree);

            rootVisualElement.GetElementByName<DirectoryField>("plugins").Binding(this, () => PluginsDir);

            OnGUICreated();
        }

        private void OnGUICreated()
        {
            PluginsDir = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Plugins));
        }

        }
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #region Properties

        #region PluginsDir

        private DirectoryInfo _pluginsDir;

        public DirectoryInfo PluginsDir
        {
            get => _pluginsDir;
            set
            {
                if (SetField(ref _pluginsDir, value))
                    _scan.Disabled = _pluginsDir == null;
            }
        }

        #endregion

        #endregion
    }
}