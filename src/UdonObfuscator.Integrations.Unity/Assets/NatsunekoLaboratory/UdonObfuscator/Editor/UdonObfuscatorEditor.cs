// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

using JetBrains.Annotations;

using NatsunekoLaboratory.UdonObfuscator.Components;
using NatsunekoLaboratory.UdonObfuscator.Extensions;
using NatsunekoLaboratory.UdonObfuscator.Models;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using Button = NatsunekoLaboratory.UdonObfuscator.Components.Button;
using Object = UnityEngine.Object;

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
        private ItemsControl _items;
        private ObservableDictionary<string, object> _extras;
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
            _items = rootVisualElement.GetElementByName<ItemsControl>("items");
            _extras = new ObservableDictionary<string, object>();

            // bindings
            _obfuscateLevelField.Binding(this, () => IsProjectLevelObfuscate);
            _workspaceField.Binding(this, () => Workspace);
            _isWriteInPlaceField.Binding(this, () => IsWriteInPlace);
            _outputDirField.Binding(this, () => OutputDir);
            _pluginsField.Binding(this, () => PluginsDir);
            _isDryRunField.Binding(this, () => IsDryRun);
            _items.Binding(Items);

            // handlers
            _scanButton.AddClickEventHandler(OnClickScanPlugins);
            _obfuscateButton.AddClickEventHandler(OnClickObfuscate);

            OnGUICreated();
        }

        private void OnGUICreated()
        {
            PluginsDir = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Plugins));
        }

        private async void OnClickScanPlugins()
        {
            var workspace = Workspace ?? GetProjectScopeWorkspace();
            var obfuscator = new ObfuscateCommand(workspace, PluginsDir, IsDryRun);
            var o = await obfuscator.ExtractPropertiesAsync();
            var plugins = obfuscator.ChunkByPlugins(o);

            Items.Clear();
            _extras.Clear();

            var culture = new CultureInfo("en-us", false);
            var ti = culture.TextInfo;

            foreach (var plugin in plugins)
            {
                var t = plugin.Key;
                var items = plugin.Value;
                var section = new BorderedSection { Title = ToTitleCase(ti, t) };
                var enabler = items[0];
                var group = new ToggleGroup { Text = $"Enable {ToTitleCase(ti, enabler.Arg)}", Value = false }.Binding(enabler.Arg, _extras, new ObjectToBooleanConverter());

                group.ReflectToState();
                section.Container.Add(group);

                var collection = new ObservableCollection<VisualElement>();
                group.Container.Add(new ItemsControl().Binding(collection));

                _extras.Add(enabler.Arg, false);

                foreach (var item in items.Skip(1))
                {
                    var arg = item.Arg;
                    var type = item.Type;

                    switch (type)
                    {
                        case Type s when s == typeof(bool):
                            collection.Add(new Checkbox { Text = ToTitleCase(ti, arg) }.Binding(arg, _extras, new ObjectToBooleanConverter()));
                            _extras.Add(arg, false);
                            break;

                        default:
                            Debug.LogWarning($"unsupported type found: {type.FullName}");
                            break;
                    }
                }

                Items.Add(section);
            }
        }

        private async void OnClickObfuscate()
        {
            var workspace = Workspace ?? GetProjectScopeWorkspace();
            var obfuscator = new ObfuscateCommand(workspace, PluginsDir, IsDryRun, IsWriteInPlace, OutputDir, _extras.ToDictionary());
        }

        private string ToTitleCase(TextInfo ti, string text)
        {
            return ti.ToTitleCase(text).Substring("--".Length).Replace("-", " ");
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

        public ObservableCollection<VisualElement> Items { get; } = new ObservableCollection<VisualElement>();

        #endregion
    }
}