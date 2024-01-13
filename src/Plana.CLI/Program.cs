// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

using Plana.CLI.Commands;
using Plana.CLI.Commands.Abstractions;
using Plana.CLI.Extensions;

var app = new RootCommand("Plana: An obfuscator for VRChat")
{
    TreatUnmatchedTokensAsErrors = false
};

var commands = new List<ISubCommand> { new ObfuscateCommand() };
var globals = new GlobalCommandLineOptions();
globals.BindTo(app);
app.AddCommands(commands.Select(w => w.Command).ToArray());

return await app.InvokeAsync(args);