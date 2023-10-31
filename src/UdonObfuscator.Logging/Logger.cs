// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics;

using Kokuban;

using UdonObfuscator.Logging.Abstractions;
using UdonObfuscator.Logging.Extensions;

namespace UdonObfuscator.Logging;

public class Logger(LogLevel level) : ILogger
{
    public void LogDebugInternal(string message)
    {
        LogDebugInternalC(message);
    }

    public void LogDebug(string message)
    {
        if (level.IsDebugEnabled())
            Console.WriteLine(Chalk.BrightWhite + $"[DEBUG] {message}");
    }

    public void LogInfo(string message)
    {
        if (level.IsInfoEnabled())
            Console.WriteLine(Chalk.BrightCyan + $"[INFO]  {message}");
    }

    public void LogWarning(string message)
    {
        if (level.IsWarnEnabled())
            Console.WriteLine(Chalk.BrightYellow + $"[WARN]  {message}");
    }

    public void LogError(string message)
    {
        if (level.IsErrorEnabled())
            Console.WriteLine(Chalk.BrightRed + $"[ERROR] {message}");
    }

    public void LogFatal(string message)
    {
        if (level.IsFatalEnabled())
            Console.WriteLine(Chalk.BgBrightRed.BrightWhite + $"[FATAL] {message}");
    }

    [Conditional("DEBUG")]
    private void LogDebugInternalC(string message)
    {
        Debug.WriteLine(message);
    }
}