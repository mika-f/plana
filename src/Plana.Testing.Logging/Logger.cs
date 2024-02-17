// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Logging.Abstractions;

namespace Plana.Testing.Logging;

public class Logger : ILogger
{
    public void LogDebugInternal(string message) { }

    public void LogDebug(string message) { }

    public void LogInfo(string message) { }

    public void LogWarning(string message) { }

    public void LogError(string message) { }

    public void LogFatal(string message) { }
}