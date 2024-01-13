// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.CommandLine;

namespace Plana.CLI.Commands.Abstractions;

internal interface ISubCommand
{
    Command Command { get; }
}