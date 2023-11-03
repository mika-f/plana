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
        logger?.LogInfo($"obfuscate workspace with {algorithms.Count} algorithm(s), this may take a few minutes......");

        await workspace.ActivateWorkspaceAsync(ct);

        var projects = await workspace.GetProjectsAsync(ct);

        foreach (var algorithm in algorithms)
            await algorithm.ObfuscateAsync(projects, ct);

        return new Dictionary<string, string>();
    }
}