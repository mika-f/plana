﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text;

using Plana.CLI.Bindings;
using Plana.CLI.Commands.Abstractions;
using Plana.CLI.Exceptions;
using Plana.CLI.Extensions;
using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Exceptions;
using Plana.Composition.Extensions;
using Plana.Hosting.Abstractions;
using Plana.Logging.Abstractions;
using Plana.Workspace;
using Plana.Workspace.Abstractions;

namespace Plana.CLI.Commands;

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
        Command.SetHandlerEx(OnHandleCommand, new LoggerBinder(), new HostingContainerBinder());
    }

    public Command Command { get; } = new("obfuscate", "obfuscate workspace");

    private async Task OnHandleCommand(InvocationContext context, ILogger logger, IHostingContainer container, CancellationToken ct)
    {
        var path = context.ParseResult.GetValueForOption(_workspace)!;
        IWorkspace workspace = path.Extension == ".sln" ? await SolutionWorkspace.CreateWorkspaceAsync(path, logger, ct) : await ProjectWorkspace.CreateWorkspaceAsync(path, logger, ct);

        try
        {
            await container.ResolveAsync(ct);

            var algorithms = await ParseExtraArguments(context, container, logger);
            var obfuscator = new Obfuscator(workspace, algorithms, logger);
            var ret = await obfuscator.ObfuscateAsync(ct);
            var isDryRun = context.ParseResult.GetValueForOption(_dryRun);
            if (isDryRun)
            {
                foreach (var document in ret)
                {
                    ct.ThrowIfCancellationRequested();

                    var source = await document.SyntaxTree.GetRootAsync(ct);

                    Console.WriteLine(document.Path);
                    Console.WriteLine(source.ToNormalizedFullString());
                    Console.WriteLine();
                }

                return;
            }

            var write = context.ParseResult.GetValueForOption(_write);
            if (write)
            {
                foreach (var document in ret)
                {
                    ct.ThrowIfCancellationRequested();

                    var source = await document.SyntaxTree.GetRootAsync(ct);
                    logger.LogInfo($"write file in-place: {document.Path}");
                    await File.WriteAllTextAsync(document.Path, source.ToNormalizedFullString(), ct);
                }

                return;
            }

            var output = context.ParseResult.GetValueForOption(_output);
            if (output != null)
            {
                var root = Path.GetDirectoryName(workspace.Path)!;

                foreach (var document in ret)
                {
                    ct.ThrowIfCancellationRequested();

                    var rel = Path.GetRelativePath(root, document.Path);
                    var to = Path.Combine(output.FullName, rel);
                    var dir = Path.GetDirectoryName(to)!;

                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);

                    logger.LogInfo($"write file: {to}");

                    var source = await document.SyntaxTree.GetRootAsync(ct);
                    await File.WriteAllTextAsync(to, source.ToNormalizedFullString(), ct);
                }

                return;
            }

            throw new InvalidOperationException("no output provider specified");
        }
        catch (ControlledGlobalExitException)
        {
            // ignored
        }
        catch (Exception e)
        {
            logger.LogDebug($"an error occurred: {e.Message}");

            throw;
        }
    }

    private async Task<List<IPlanaPlugin>> ParseExtraArguments(InvocationContext context, IHostingContainer container, ILogger logger)
    {
        var command = new Command("obfuscate");
        command.AddOptions(_workspace, _dryRun, _write, _output, GlobalCommandLineOptions.LogLevel, GlobalCommandLineOptions.Plugins);

        var enablers = new Dictionary<Option<bool>, IPlanaPlugin>();
        var options = new Dictionary<IPlanaPlugin, Dictionary<IPlanaPluginOption, Option>>();
        var allPluginOptions = new List<IPlanaPluginOption>();

        foreach (var (plugin, attr) in container.Items)
        {
            var enabler = new Option<bool>($"--{attr.Id}", () => false, $"enable {plugin.Name} plugin");
            allPluginOptions.Add(new PlanaPluginOption(attr.Id, plugin.Name, $"enable {plugin.Name} plugin", false));

            try
            {
                var temporary = new List<Option> { enabler };

                var dict = new Dictionary<IPlanaPluginOption, Option>();

                foreach (var opt in plugin.Options)
                {
                    var option = opt.ToOptions();
                    if (option == null)
                        continue;

                    dict.Add(opt, option);
                    temporary.Add(option);
                    allPluginOptions.Add(opt);
                }

                options.Add(plugin, dict);

                // success
                enablers.Add(enabler, plugin);
                command.AddOptions(temporary.ToArray());
            }
            catch (InvalidFormatException e)
            {
                // remove
                enablers.Remove(enabler);

                logger.LogError($"invalid name '{e.Message}' in {attr.GetType().Assembly.FullName}");
            }
        }

        if (context.ParseResult.GetValueForOption(GlobalCommandLineOptions.RetrieveArgs))
        {
            foreach (var o in allPluginOptions)
                if (o.Description == $"enable {o.FriendlyName} plugin")
                    Console.WriteLine($"Id={o.Name}, Name={o.FriendlyName}, Type={o.ValueType.FullName}, Required={false}, Description=SEPARATOR");
                else
                    Console.WriteLine($"Id={o.Name}, Name={o.FriendlyName}, Type={o.ValueType.FullName}, Required={false}, Description={o.Description}");

            throw new ControlledGlobalExitException();
        }

        IEnumerable<string> AsArgs(IReadOnlyList<Token> tokens)
        {
            foreach (var token in tokens)
                switch (token.Type)
                {
                    case TokenType.Argument:
                        if (token.Value.Contains(' '))
                        {
                            var sb = new StringBuilder();
                            foreach (var c in token.Value)
                                switch (c)
                                {
                                    case '"':
                                        sb.Append('\\');
                                        sb.Append(c);
                                        continue;

                                    case '\\':
                                        sb.Append(@"\\");
                                        continue;

                                    default:
                                        sb.Append(c);
                                        break;
                                }

                            yield return $"\"{sb}\"";
                            break;
                        }

                        yield return token.Value;
                        break;

                    case TokenType.Command:
                    case TokenType.Option:
                    case TokenType.DoubleDash:
                    case TokenType.Unparsed:
                    case TokenType.Directive:
                        yield return token.Value;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        var args = string.Join(" ", AsArgs(context.ParseResult.Tokens));
        var ret = command.Parse(args);
        if (ret.UnmatchedTokens.Count > 0)
        {
            foreach (var token in ret.UnmatchedTokens)
                logger?.LogFatal($"unrecognized command or argument {token}");

            await command.InvokeAsync("--help");

            throw new InvalidOperationException();
        }

        var items = new List<IPlanaPlugin>();
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