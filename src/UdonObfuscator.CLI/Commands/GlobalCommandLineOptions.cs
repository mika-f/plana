// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

using UdonObfuscator.CLI.Commands.Abstractions;
using UdonObfuscator.Logging;

namespace UdonObfuscator.CLI.Commands;

internal class GlobalCommandLineOptions : ICommandLineOptions
{
    private readonly List<Option> _options;

    // public Option<FileInfo> Workspace { get; } = new Option<FileInfo>("--workspace", "path to workspace .csproj or .sln").ExistingOnly();


    public static Option<LogLevel> LogLevel { get; } = new("--log-level", () => Logging.LogLevel.Normal, "log detail level");

    public static Option<DirectoryInfo> Plugins { get; } = new Option<DirectoryInfo>("--plugins", () => new DirectoryInfo("./"), "path to plugins directory loaded from").ExistingOnly();

    public GlobalCommandLineOptions()
    {
        _options = [LogLevel, Plugins];
    }

    public void AddOption<T>(string name, string description)
    {
        _options.Add(new Option<T>(name, description));
    }

    public void AddOption<T>(string name, string description, Func<T> getDefaultValue)
    {
        _options.Add(new Option<T>(name, getDefaultValue, description));
    }

    public void BindTo(Command command)
    {
        foreach (var option in _options)
            command.AddGlobalOption(option);
    }
}