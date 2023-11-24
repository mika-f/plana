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
using NatsunekoLaboratory.UdonObfuscator.Models;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using Button = NatsunekoLaboratory.UdonObfuscator.Components.Button;

namespace NatsunekoLaboratory.UdonObfuscator
{
#if USTYLED
    using UStyled;
    using UStyled.Configurations;
    using UStyled.Configurations.Presets;
#endif

    public class UdonObfuscatorEditor : EditorWindow, INotifyPropertyChanged
    {
        private const string UxmlGuid = "512581237e5c880478d0c1a9d8a40ef5";
        private const string UssGuid = "b84460955893c6149baab61b8cf213a1";
        private const string Plugins = "aa64d32311d16b64384036813c06488a";
#if USTYLED
        private static readonly UStyledCompiler UStyled;
#endif
        private Button _obfuscateButton;
        private Button _scanButton;
        private ToggleGroup _obfuscateLevelField;
        private FileField _workspaceField;
        private ToggleGroup _isWriteInPlaceField;
        private DirectoryField _outputDirField;
        private Checkbox _isDryRunField;
        private DirectoryField _pluginsField;

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

            _obfuscateLevelField = rootVisualElement.GetElementByName<ToggleGroup>("obfuscate-level");
            _workspaceField = rootVisualElement.GetElementByName<FileField>("workspace");
            _isWriteInPlaceField = rootVisualElement.GetElementByName<ToggleGroup>("is-write-in-place");
            _outputDirField = rootVisualElement.GetElementByName<DirectoryField>("output-dir");
            _pluginsField = rootVisualElement.GetElementByName<DirectoryField>("plugins");
            _isDryRunField = rootVisualElement.GetElementByName<Checkbox>("is-dry-run");
            _scanButton = rootVisualElement.GetElementByName<Button>("scan");
            _obfuscateButton = rootVisualElement.GetElementByName<Button>("obfuscate");

            // bindings
            _obfuscateLevelField.Binding(this, () => IsProjectLevelObfuscate);
            _workspaceField.Binding(this, () => Workspace);
            _isWriteInPlaceField.Binding(this, () => IsWriteInPlace);
            _outputDirField.Binding(this, () => OutputDir);
            _pluginsField.Binding(this, () => PluginsDir);
            _isDryRunField.Binding(this, () => IsDryRun);

            // handlers
            _scanButton.AddClickEventHandler(OnClickScanPlugins);

            OnGUICreated();
        }

        private void OnGUICreated()
        {
            PluginsDir = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Plugins));
        }

        private async void OnClickScanPlugins()
        {
            var dir = PluginsDir.FullName;
            var workspace = Workspace ?? GetProjectScopeWorkspace();
            var obfuscator = new ObfuscateCommand(workspace, PluginsDir, IsDryRun);
            var o = await obfuscator.ExtractPropertiesAsync();
            var sections = obfuscator.ChunkByPlugins(o);
        }

        private FileInfo GetProjectScopeWorkspace()
        {
            var segments = Application.dataPath.Split('/');
            var project = segments[segments.Length - 2];
            var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", $"{project}.sln"));

            return new FileInfo(path);
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

        #region ObfuscateLevel

        private bool _isProjectLevelObfuscate;

        public bool IsProjectLevelObfuscate
        {
            get => _isProjectLevelObfuscate;
            set => SetField(ref _isProjectLevelObfuscate, value);
        }

        #endregion

        #region Workspace

        private FileInfo _workspace;

        public FileInfo Workspace
        {
            get => _workspace;
            set => SetField(ref _workspace, value);
        }

        #endregion

        #region IsWriteInPlace

        private bool _isWriteInPlace;

        public bool IsWriteInPlace
        {
            get => _isWriteInPlace;
            set => SetField(ref _isWriteInPlace, value);
        }

        #endregion

        #region OutputDir

        private DirectoryInfo _outputDir;

        public DirectoryInfo OutputDir
        {
            get => _outputDir;
            set => SetField(ref _outputDir, value);
        }

        #endregion

        #region PluginsDir

        private DirectoryInfo _pluginsDir;

        public DirectoryInfo PluginsDir
        {
            get => _pluginsDir;
            set
            {
                if (SetField(ref _pluginsDir, value))
                    _scanButton.Disabled = _pluginsDir == null;
            }
        }

        #endregion

        #region IsDryRun

        private bool _isDryRun;

        public bool IsDryRun
        {
            get => _isDryRun;
            set => SetField(ref _isDryRun, value);
        }

        #endregion

        #endregion
    }
}