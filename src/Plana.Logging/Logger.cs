// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics;

using Kokuban;

using Plana.Logging.Abstractions;
using Plana.Logging.Extensions;

namespace Plana.Logging;

public class Logger(LogLevel level) : ILogger
{
    public void LogDebugInternal(string message)
    {
        LogDebugInternalC(message);
    }

    public void LogDebug(string message)
    {
        if (level.IsDebugEnabled())
            Console.WriteLine(Chalk.BrightWhite + $"[{GetDateTime()}] [DEBUG] {message}");
    }

    public void LogInfo(string message)
    {
        if (level.IsInfoEnabled())
            Console.WriteLine(Chalk.BrightCyan + $"[{GetDateTime()}] [INFO]  {message}");
    }

    public void LogWarning(string message)
    {
        if (level.IsWarnEnabled())
            Console.WriteLine(Chalk.BrightYellow + $"[{GetDateTime()}] [WARN]  {message}");
    }

    public void LogError(string message)
    {
        if (level.IsErrorEnabled())
            Console.WriteLine(Chalk.BrightRed + $"[{GetDateTime()}] [ERROR] {message}");
    }

    public void LogFatal(string message)
    {
        if (level.IsFatalEnabled())
            Console.WriteLine(Chalk.BgBrightRed.BrightWhite + $"[{GetDateTime()}] [FATAL] {message}");
    }

    private string GetDateTime()
    {
        return DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss");
    }

    [Conditional("DEBUG")]
    private void LogDebugInternalC(string message)
    {
        Debug.WriteLine(message);
    }
}