// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics;

using UdonObfuscator.Logging.Extensions;

namespace UdonObfuscator.Logging;

public class Logger(LogLevel level)
{
    [Conditional("DEBUG")]
    public void LogDebugInternal(string message)
    {
        Debug.WriteLine(message);
    }

    public void LogDebug(string message)
    {
        if (level.IsDebugEnabled())
            Debug.WriteLine(message);
    }

    public void LogInfo(string message)
    {
        if (level.IsInfoEnabled())
            Console.WriteLine(message);
    }

    public void LogWarn(string message)
    {
        if (level.IsWarnEnabled())
            Console.WriteLine(message);
    }

    public void LogError(string message)
    {
        if (level.IsErrorEnabled())
            Console.WriteLine(message);
    }

    public void LogFatal(string message)
    {
        if (level.IsFatalEnabled())
            Console.WriteLine(message);
    }
}