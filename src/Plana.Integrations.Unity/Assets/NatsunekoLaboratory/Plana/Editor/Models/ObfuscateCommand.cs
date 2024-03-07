// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NatsunekoLaboratory.Plana.Models.Abstractions;

using UnityEngine;

namespace NatsunekoLaboratory.Plana.Models
{
    internal class ObfuscateCommand : CommandBase
    {
        private static readonly List<string> KnownProperties = new List<string>
        {
            "--workspace",
            "--dry-run",
            "--write",
            "--output",
            "--plugins",
            "--help"
        };

        private readonly Dictionary<string, object> _extras;
        private readonly bool _isDryRun;
        private readonly bool _isWriteInPlace;
        private readonly DirectoryInfo _outputDir;
        private readonly DirectoryInfo _plugins;
        private readonly FileSystemInfo _workspace;

        public ObfuscateCommand(FileSystemInfo workspace, DirectoryInfo plugins, bool isDryRun = false, bool isWriteInPlace = false, DirectoryInfo outputDir = null, Dictionary<string, object> extras = null)
        {
            _workspace = workspace;
            _plugins = plugins;
            _isDryRun = isDryRun;
            _isWriteInPlace = isWriteInPlace;
            _outputDir = outputDir;
            _extras = extras ?? new Dictionary<string, object>();
        }

        public async Task<Dictionary<string, (string FriendlyName, Type Type, string Descripton)>> ExtractPropertiesAsync()
        {
            var dict = new Dictionary<string, (string FriendlyName, Type Type, string Descripton)>();
            var arguments = BuildArgs();
            arguments.Add("--retrieve-args");

            var (code, stdout) = await RunAsync("obfuscate", string.Join(" ", arguments));

            using (var sr = new StringReader(stdout))
            {
                var line = "";
                while ((line = await sr.ReadLineAsync()) != null)
                {
                    var segments = line.Split(',').Select(w => w.Trim()).ToArray();
                    var id = segments[0];
                    var name = segments[1];
                    var type = segments[2];
                    var isRequired = segments[3];
                    var description = string.Join(" ", segments.Skip(4));


                    if (id.StartsWith("Id=") && name.StartsWith("Name=") && type.StartsWith("Type=") && description.StartsWith("Description="))
                    {
                        var i = id.Substring("Id=".Length);
                        var n = name.Substring("Name=".Length);
                        var t = Type.GetType(type.Substring("Type=".Length));
                        var d = description.Substring("Description=".Length);

                        if (KnownProperties.Contains(n))
                            continue;

                        if (t != null)
                            dict.Add(i, (n, t, d));
                    }
                }
            }

            return dict;
        }

        public Dictionary<string, List<(string Id, string FriendlyName, string Description, Type Type)>> ChunkByPlugins(Dictionary<string, (string FriendlyName, Type Type, string Description)> properties)
        {
            var dict = new Dictionary<string, List<(string Id, string FriendlyName, string Description, Type Type)>>();
            var cur = "";

            foreach (var property in properties)
            {
                var arg = property.Key;
                var type = property.Value.Type;
                var friendlyName = property.Value.FriendlyName;
                var description = property.Value.Description;

                if (description == "SEPARATOR")
                    cur = arg;

                if (string.IsNullOrWhiteSpace(cur))
                    continue; // unknown

                if (dict.ContainsKey(cur))
                    dict[cur].Add((arg, friendlyName, description, type));
                else
                    dict[cur] = new List<(string Id, string FriendlyName, string Description, Type Type)> { (arg, friendlyName, description, type) };
            }

            return dict;
        }

        public async Task ObfuscateAsync()
        {
            var args = BuildArgs();
            var (code, stdout) = await RunAsync("obfuscate", string.Join(" ", args));

            Action<string> logger = null;

            switch (code)
            {
                case 0:
                    logger = Debug.Log;
                    break;

                case 1:
                    logger = Debug.LogError;
                    break;

                default:
                    throw new ArgumentException();
            }

            using (var sr = new StringReader(stdout))
            {
                var line = "";
                while ((line = await sr.ReadLineAsync()) != null)
                    logger.Invoke(line);
            }
        }

        private List<string> BuildArgs()
        {
            var args = new List<string>
            {
                "--workspace", $"\"{_workspace.FullName}\"",
                "--plugins", $"\"{_plugins.FullName}\""
            };

            if (_isDryRun)
                args.Add("--dry-run");

            if (_isWriteInPlace)
                args.Add("--write");

            if (_outputDir != null)
                args.AddRange(new[] { "--output", $"\"{_outputDir.FullName}\"" });

            if (_extras.Any())
                foreach (var extra in _extras)
                {
                    var k = extra.Key;
                    var v = extra.Value;

                    if (v is bool b)
                    {
                        if (b)
                            args.Add($"--{k}");
                        continue;
                    }

                    if (v is FileSystemInfo f)
                    {
                        args.AddRange(new[] { $"--{k}", $"\"{f.FullName}\"" });
                        continue;
                    }

                    args.AddRange(new[] { $"--{k}", v.ToString() });
                }

            return args;
        }
    }
}