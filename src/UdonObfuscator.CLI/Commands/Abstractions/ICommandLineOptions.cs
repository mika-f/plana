// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

namespace UdonObfuscator.CLI.Commands.Abstractions;

internal interface ICommandLineOptions
{
    void AddOption<T>(string name, string description);

    void AddOption<T>(string name, string description, Func<T> getDefaultValue);

    void BindTo(Command command);
}