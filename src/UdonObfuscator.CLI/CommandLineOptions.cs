// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

using UdonObfuscator.Logging;

namespace UdonObfuscator.CLI;

internal class CommandLineOptions
{
    private readonly List<Option> _options;

    public Option<FileInfo> Workspace { get; } = new Option<FileInfo>("--workspace", "path to workspace .csproj or .sln").ExistingOnly();

    public Option<bool> Write { get; } = new ("--write", () => false, "write obfuscated source code in place");

    public Option<bool> DryRun { get; } = new("--dry-run", () => false, "dry-run obfuscate");

    public Option<LogLevel> LogLevel { get; } = new("--log-level", () => Logging.LogLevel.Normal, "log detail level");

    public CommandLineOptions()
    {
        _options = [Workspace, Write, DryRun, LogLevel];
    }

    public void AddOption<T>(string name, string description)
    {
        _options.Add(new Option<T>(name, description));
    }

    public void AddOption<T>(string name, string description, Func<T> getDefaultValue)
    {
        _options.Add(new Option<T>(name, getDefaultValue, description));
    }

    public void Bind(Command command)
    {
        foreach (var option in _options)
            command.AddOption(option);
    }
}