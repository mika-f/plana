﻿// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UdonObfuscator.Hosting.Abstractions;
using UdonObfuscator.Logging.Abstractions;
using UdonObfuscator.Workspace.Abstractions;

namespace UdonObfuscator;

public class Obfuscator(IWorkspace workspace, IHostingContainer container, ILogger? logger)
{
    public async Task<Dictionary<string, string>> ObfuscateAsync()
    {
        await container.ResolveAsync();

        return new Dictionary<string, string>();
    }
}