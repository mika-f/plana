// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;

using UdonObfuscator.CLI.Bindings;
using UdonObfuscator.CLI.Commands.Abstractions;
using UdonObfuscator.CLI.Extensions;
using UdonObfuscator.Composition.Abstractions.Algorithm;
using UdonObfuscator.Hosting.Abstractions;
using UdonObfuscator.Logging.Abstractions;
using UdonObfuscator.Workspace.Abstractions;

namespace UdonObfuscator.CLI.Commands;

public class ObfuscateCommand : ISubCommand
{
    private readonly Option<bool> _dryRun = new("--dry-run", () => false, "dry-run obfuscate");
    private readonly Option<DirectoryInfo> _output = new("--output", "path to directory write to");
    private readonly Option<FileInfo> _workspace = new Option<FileInfo>("--workspace", "path to workspace .csproj or .sln").ExistingOnly();
    private readonly Option<bool> _write = new("--write", () => false, "write obfuscated source code in place");

    public ObfuscateCommand()
    {
        Command.TreatUnmatchedTokensAsErrors = false;
        Command.AddOptions(_workspace, _dryRun, _write, _output);
        Command.SetHandlerEx(OnHandleCommand, new LoggerBinder(), new WorkspaceBinder(_workspace), new HostingContainerBinder());
    }

    public Command Command { get; } = new("obfuscate", "obfuscate workspace");

    private async Task OnHandleCommand(InvocationContext context, ILogger logger, IWorkspace workspace, IHostingContainer container, CancellationToken ct)
    {
        try
        {
            await container.ResolveAsync(ct);

            var algorithms = await ParseExtraArguments(context, container, logger);
            var obfuscator = new Obfuscator(workspace, algorithms, logger);
            var ret = await obfuscator.ObfuscateAsync(ct);
            var isDryRun = context.ParseResult.GetValueForOption(_dryRun);
            if (isDryRun)
            {
                foreach (var (path, content) in ret)
                {
                    ct.ThrowIfCancellationRequested();

                    Console.WriteLine(path);
                    Console.WriteLine(content);
                    Console.WriteLine();
                }

                return;
            }

            var write = context.ParseResult.GetValueForOption(_write);
            if (write)
            {
                foreach (var (path, content) in ret)
                    await File.WriteAllTextAsync(path, content, ct);

                return;
            }

            var output = context.ParseResult.GetValueForOption(_output);
            if (output != null)
            {
                var root = Path.GetDirectoryName(workspace.Path)!;

                foreach (var (path, content) in ret)
                {
                    var rel = Path.GetRelativePath(root, path);
                    var to = Path.Combine(output.FullName, rel);
                    var dir = Path.GetDirectoryName(to)!;

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    await File.WriteAllTextAsync(to, content, ct);
                }

                return;
            }

            throw new InvalidOperationException("no output provider specified");
        }
        catch (Exception e)
        {
            logger?.LogDebug($"an error occurred: {e.Message}");
        }
    }

    private async Task<List<IObfuscatorAlgorithm>> ParseExtraArguments(InvocationContext context, IHostingContainer container, ILogger logger)
    {
        var command = new Command("obfuscate");
        command.AddOptions(_workspace, _dryRun, _write, _output, GlobalCommandLineOptions.LogLevel, GlobalCommandLineOptions.Plugins);

        var enablers = new Dictionary<Option<bool>, IObfuscatorAlgorithm>();
        var options = new Dictionary<IObfuscatorAlgorithm, Dictionary<IObfuscatorAlgorithmOption, Option>>();

        foreach (var (obfuscator, attr) in container.Items)
        {
            var enabler = new Option<bool>($"--{attr.Id}", () => false, $"use {attr.Id}");
            enablers.Add(enabler, obfuscator);

            var dict = new Dictionary<IObfuscatorAlgorithmOption, Option>();

            foreach (var opt in obfuscator.Options)
            {
                var option = opt.ToOption();
                if (option == null)
                    continue;

                dict.Add(opt, option);
                command.AddOptions(option);
            }

            command.AddOptions(enabler);
            options.Add(obfuscator, dict);
        }

        var args = string.Join(" ", context.ParseResult.Tokens);
        var ret = command.Parse(args);
        if (ret.UnmatchedTokens.Count > 0)
        {
            foreach (var token in ret.UnmatchedTokens)
                logger?.LogError($"unrecognized command or argument {token}");

            await command.InvokeAsync("--help");

            throw new InvalidOperationException();
        }

        var items = new List<IObfuscatorAlgorithm>();
        foreach (var (option, instance) in enablers)
        {
            var enabled = ret.GetValueForOption(option);
            if (enabled)
            {
                var dict = options[instance];

                instance.BindParameters(new ObfuscatorParameterBinder(ret, dict));
                items.Add(instance);
            }
        }

        return items;
    }
}