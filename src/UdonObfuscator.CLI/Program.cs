// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Invocation;

using UdonObfuscator;
using UdonObfuscator.CLI;
using UdonObfuscator.Logging;

var app = new RootCommand("UdonObfuscator: An obfuscator for VRChat");
var options = new CommandLineOptions();

options.Bind(app);
app.SetHandler(Handler);

return await app.InvokeAsync(args);

async Task Handler(InvocationContext context)
{
    var workspace = context.ParseResult.GetValueForOption(options.Workspace) ?? throw new InvalidOperationException();
    var level = context.ParseResult.GetValueForOption(options.LogLevel);
    var logger = new Logger(level);

    var obfuscator = new Obfuscator(logger);
    var ret = await obfuscator.ObfuscateAsync(workspace);

    var isDryRun = context.ParseResult.GetValueForOption(options.DryRun);
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

    var write = context.ParseResult.GetValueForOption(options.Write);
    foreach (var (path, content) in ret)
    {
        var to = write ? path : Path.Combine(Path.GetDirectoryName(path)!, $"{Path.GetFileNameWithoutExtension(path)}.g.cs");
        await File.WriteAllTextAsync(to, content);
    }
}