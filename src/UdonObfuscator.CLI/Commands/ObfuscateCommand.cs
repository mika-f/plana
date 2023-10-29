// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;

using UdonObfuscator.CLI.Bindings;
using UdonObfuscator.CLI.Commands.Abstractions;
using UdonObfuscator.CLI.Extensions;
using UdonObfuscator.HostInfrastructure;
using UdonObfuscator.Logging.Abstractions;

namespace UdonObfuscator.CLI.Commands;

public class ObfuscateCommand : ISubCommand
{
    private readonly Option<bool> _dryRun = new("--dry-run", () => false, "dry-run obfuscate");
    private readonly Option<FileInfo> _workspace = new Option<FileInfo>("--workspace", "path to workspace .csproj or .sln").ExistingOnly();
    private readonly Option<bool> _write = new("--write", () => false, "write obfuscated source code in place");

    public ObfuscateCommand()
    {
        Command.AddOptions(_workspace, _dryRun, _write);
        Command.SetHandlerEx(OnHandleCommand, new LoggerBinder(), new PluginBinder());
    }

    public Command Command { get; } = new("obfuscate", "obfuscate workspace");

    private async Task OnHandleCommand(InvocationContext context, ILogger logger, PluginResolver resolver, CancellationToken ct)
    {
        var workspace = context.ParseResult.GetValueForOption(_workspace) ?? throw new InvalidOperationException();
        var obfuscator = new Obfuscator(logger);

        // load plugins for obfuscating
        await resolver.ResolveAsync();

        var ret = await obfuscator.ObfuscateAsync(workspace);
        var isDryRun = context.ParseResult.GetValueForOption(_dryRun);
        if (isDryRun)
        {
            foreach (var (path, content) in ret)
            {
                Console.WriteLine(path);
                Console.WriteLine(content);
                Console.WriteLine();
            }

            return;
        }

        var write = context.ParseResult.GetValueForOption(_write);
        foreach (var (path, content) in ret)
        {
            var to = write ? path : Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}.g.cs");
            await File.WriteAllTextAsync(to, content, ct);
        }
    }
}