// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Logging.Abstractions;

public interface ILogger
{
    void LogDebugInternal(string message);

    void LogDebug(string message);

    void LogInfo(string message);

    void LogWarning(string message);

    void LogError(string message);

    void LogFatal(string message);
}