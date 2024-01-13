// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace Plana.Logging.Extensions;

public static class LogLevelExtensions
{
    public static bool IsDebugEnabled(this LogLevel logLevel)
    {
        return logLevel >= LogLevel.Verbose;
    }

    public static bool IsInfoEnabled(this LogLevel logLevel)
    {
        return logLevel >= LogLevel.Normal;
    }

    public static bool IsWarnEnabled(this LogLevel logLevel)
    {
        return logLevel >= LogLevel.Warn;
    }

    public static bool IsErrorEnabled(this LogLevel logLevel)
    {
        return logLevel >= LogLevel.Error;
    }

    public static bool IsFatalEnabled(this LogLevel logLevel)
    {
        return logLevel >= LogLevel.Silent;
    }
}