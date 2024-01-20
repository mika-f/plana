// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.CLI.Exceptions;

public class ControlledGlobalExitException : Exception
{
    public ControlledGlobalExitException() : base("Controlled global exit operation initiated.") { }

    public ControlledGlobalExitException(string message) : base(message) { }

    public ControlledGlobalExitException(string message, Exception innerException) : base(message, innerException) { }
}