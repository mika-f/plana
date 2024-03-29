﻿// ------------------------------------------------------------------------------------------
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

using NatsunekoLaboratory.Plana.Components;
using NatsunekoLaboratory.Plana.Components.Abstractions;
using NatsunekoLaboratory.Plana.Extensions;
using NatsunekoLaboratory.Plana.Models;
using NatsunekoLaboratory.Plana.Models.Abstractions;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

using Button = NatsunekoLaboratory.Plana.Components.Button;
using Object = UnityEngine.Object;

namespace NatsunekoLaboratory.Plana
{
#if USTYLED
    using UStyled;
    using UStyled.Configurations;
    using UStyled.Configurations.Presets;
#endif

    public class PlanaEditor : EditorWindow, INotifyPropertyChanged, IStyledComponents
    {
        private const string Plugins = "aa64d32311d16b64384036813c06488a";

#if USTYLED
        private static readonly UStyledCompiler UStyled;
#endif

        private readonly StyledComponents _sc = StyledComponents.Create(
            "512581237e5c880478d0c1a9d8a40ef5",
            "b84460955893c6149baab61b8cf213a1",
            "fb937f4bb333fa14c909bf77ac1d5a67",
            "c93f385d6149dfa48bacbfa0f60f923f"
        );

        private ObservableDictionary<string, object> _extras;
        private Checkbox _isDryRunField;
        private ToggleGroup _isWriteInPlaceField;
        private ItemsControl _items;
        private Button _loadPreferencesButton;
        private Button _obfuscateButton;
        private ToggleGroup _obfuscateLevelField;
        private DirectoryField _outputDirField;
        private DirectoryField _pluginsField;
        private Button _savePreferencesButton;
        private Button _scanButton;
        private SerializedObject _so;
        private FileField _workspaceField;

#if USTYLED
        static PlanaEditor()
        {
            UStyled = new UStyledCompilerBuilder()
                      .UsePreprocessor(UStyledPreprocessor.SerializedValue)
                      .UsePresets(new StylifyPreset())
                      .Build();
        }
#endif

        public event PropertyChangedEventHandler PropertyChanged;

        public string UxmlGuid => _sc.Uxml;

        public string AdditionalUssGuid => _sc.Uss;

        [MenuItem("Window/Natsuneko Laboratory/Plana")]
        public static void ShowWindow()
        {
            var window = GetWindow<PlanaEditor>("Plana");
            window.Show();
        }

#if USTYLED
        [MenuItem("Window/Natsuneko Laboratory/Debug/Export Compiled Assets")]
        public static void Export()
        {
            var types = typeof(PlanaEditor)
                        .Assembly
                        .GetTypes()
                        .Where(w => typeof(IStyledComponents).IsAssignableFrom(w))
                        .Where(w => w.GetConstructors().Any())
                        .Select(Activator.CreateInstance)
                        .Cast<IStyledComponents>()
                        .ToList();

            foreach (var type in types)
            {
                var path = AssetDatabase.GUIDToAssetPath(type.UxmlGuid);
                var asset = LoadAssetByGuid<VisualTreeAsset>(type.UxmlGuid);
                var (uxml, uss) = UStyled.CompileAsString(asset);

                if (!string.IsNullOrEmpty(type.AdditionalUssGuid))
                {
                    var str = AssetDatabase.GUIDToAssetPath(type.AdditionalUssGuid);
                    using (var sr = new StreamReader(str))
                        uss += sr.ReadToEnd();
                }

                var dir = Path.GetDirectoryName(path) ?? throw new ArgumentException();
                var filename = Path.GetFileNameWithoutExtension(path);
                var generatedUxml = Path.Combine(dir, $"{filename}.g.uxml");
                var generatedUss = Path.Combine(dir, $"{filename}.g.uss");

                File.WriteAllText(generatedUxml, uxml);
                File.WriteAllText(generatedUss, uss);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
#endif

        private static T LoadAssetByGuid<T>(string guid) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid));
        }

        // ReSharper disable once InconsistentNaming
        public void CreateGUI()
        {
            _so = new SerializedObject(this);
            _so.Update();

            var xaml = LoadAssetByGuid<VisualTreeAsset>(_sc.Uxml);

#if USTYLED
            var (uxml, uss) = UStyled.CompileAsAsset(xaml);
#else
            var uxml = LoadAssetByGuid<VisualTreeAsset>(_sc.Uxml);
            var uss = LoadAssetByGuid<StyleSheet>(_sc.Uss);
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
            _savePreferencesButton = rootVisualElement.GetElementByName<Button>("save-preferences");
            _loadPreferencesButton = rootVisualElement.GetElementByName<Button>("load-preferences");
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
            _savePreferencesButton.AddClickEventHandler(OnClickSavePreferences);
            _loadPreferencesButton.AddClickEventHandler(OnClickLoadPreferences);

            OnGUICreated();
        }

        private void OnGUICreated()
        {
            PluginsDir = new DirectoryInfo(AssetDatabase.GUIDToAssetPath(Plugins));

            // initial state
            UpdateObfuscateButton();

            // 1st scan
            OnClickLoadPreferences(null);
        }

        private async void OnClickScanPlugins(IAsyncCallbackHandler handler)
        {
            OnClickLoadPreferences(handler);
        }

        private async void OnClickObfuscate(IAsyncCallbackHandler handler)
        {
            try
            {
                var workspace = GetProjectScopeWorkspace(Workspace);
                var obfuscator = new ObfuscateCommand(workspace, PluginsDir, IsDryRun, IsWriteInPlace, OutputDir, _extras.ToDictionary());
                await obfuscator.ObfuscateAsync();

                AssetDatabase.Refresh();
                handler.Next();
            }
            catch
            {
                handler.Abort();
            }
        }

        private void OnClickSavePreferences(IAsyncCallbackHandler handler)
        {
            try
            {
                var v = _extras.ToDictionary().Where(w => (bool)w.Value).Select(w => w.Key);
                EditorUserSettings.SetConfigValue("PlanaPreferences", string.Join(" ", v));

                handler.Next();
            }
            catch
            {
                handler.Abort();
            }
        }

        private async void OnClickLoadPreferences([CanBeNull] IAsyncCallbackHandler handler)
        {
            try
            {
                var workspace = GetProjectScopeWorkspace(Workspace);
                var obfuscator = new ObfuscateCommand(workspace, PluginsDir, IsDryRun);
                var o = await obfuscator.ExtractPropertiesAsync();
                var v = EditorUserSettings.GetConfigValue("PlanaPreferences");
                var plugins = obfuscator.ChunkByPlugins(o);
                InflatePluginsSection(plugins, v.Split(' ').ToDictionary(w => w, w => true));

                handler?.Next();
            }
            catch
            {
                handler?.Abort();
            }
        }

        private void InflatePluginsSection(Dictionary<string, List<(string Id, string FriendlyName, string Description, Type Type)>> plugins, Dictionary<string, bool> defaults)
        {
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
                var group = new ToggleGroup { Text = $"Enable {ToTitleCase(ti, enabler.FriendlyName)}", Value = defaults.GetValueOrDefault(enabler.Id, false) }.Binding(enabler.Id, _extras, new ObjectToBooleanConverter());

                group.ReflectToState();
                section.Container.Add(group);

                var collection = new ObservableCollection<VisualElement>();
                group.Container.Add(new ItemsControl().Binding(collection));

                _extras.Add(enabler.Id, defaults.GetValueOrDefault(enabler.Id, false));

                foreach (var item in items.Skip(1))
                    switch (item.Type)
                    {
                        case Type s when s == typeof(bool):
                            var b = defaults.GetValueOrDefault(item.Id, false);

                            collection.Add(new Checkbox { Text = item.FriendlyName, Value = b }.Binding(item.Id, _extras, new ObjectToBooleanConverter()).WithTooltip(item.Description));
                            _extras.Add(item.Id, b);
                            break;

                        default:
                            Debug.LogWarning($"unsupported type found: {item.Type.FullName}");
                            break;
                    }

                Items.Add(section);
            }
        }

        private string ToTitleCase(TextInfo ti, string text)
        {
            return ti.ToTitleCase(text).Replace("-", " ");
        }

        private FileInfo GetProjectScopeWorkspace(FileInfo fi)
        {
            if (fi == null)
            {
                if (IsProjectLevelObfuscate)
                {
                    var segments = Application.dataPath.Split('/');
                    var project = segments[segments.Length - 2];
                    var path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", $"{project}.sln"));
                    return new FileInfo(path);
                }

                return GetCSharpProjectWorkspace("Assembly-CSharp");
            }


            var filename = Path.GetFileNameWithoutExtension(fi.FullName);
            return GetCSharpProjectWorkspace(filename);
        }

        private FileInfo GetCSharpProjectWorkspace(string project)
        {
            var path = Application.dataPath;
            return new FileInfo(Path.GetFullPath(Path.Combine(path, "..", $"{project}.csproj")));
        }

        private void UpdateObfuscateButton()
        {
            if (_isWriteInPlace)
            {
                _obfuscateButton.Disabled = false; // always enabled when is write in-place
                return;
            }

            _obfuscateButton.Disabled = _outputDir == null;
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
            set
            {
                if (SetField(ref _isWriteInPlace, value))
                    UpdateObfuscateButton();
            }
        }

        #endregion

        #region OutputDir

        private DirectoryInfo _outputDir;

        public DirectoryInfo OutputDir
        {
            get => _outputDir;
            set
            {
                if (SetField(ref _outputDir, value))
                    UpdateObfuscateButton();
            }
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