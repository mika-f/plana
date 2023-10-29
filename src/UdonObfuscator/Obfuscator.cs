﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UdonObfuscator.Logging.Abstractions;

namespace UdonObfuscator;

public class Obfuscator(ILogger? logger)
{
    public async Task<Dictionary<string, string>> ObfuscateAsync(FileInfo workspace)
    {
        return new Dictionary<string, string>();
    }
}