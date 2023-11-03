// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using UdonObfuscator.Composition.Abstractions.Algorithm;
using UdonObfuscator.Logging.Abstractions;
using UdonObfuscator.Workspace.Abstractions;

namespace UdonObfuscator;

public class Obfuscator(IWorkspace workspace, List<IObfuscatorAlgorithm> algorithms, ILogger? logger)
{
    public async Task<Dictionary<string, string>> ObfuscateAsync(CancellationToken ct)
    {
        await workspace.ActivateWorkspaceAsync(ct);

        return new Dictionary<string, string>();
    }
}