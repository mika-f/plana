// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using Plana.Composition.Abstractions.Algorithm;
using Plana.Logging.Abstractions;
using Plana.Workspace.Abstractions;

namespace Plana;

public class Obfuscator(IWorkspace workspace, List<IObfuscatorAlgorithm> algorithms, ILogger? logger)
{
    public async Task<Dictionary<string, string>> ObfuscateAsync(CancellationToken ct)
    {
        logger?.LogInfo($"obfuscate workspace with {algorithms.Count} algorithm(s), this may take a few minutes......");

        await workspace.ActivateWorkspaceAsync(ct);

        var projects = await workspace.GetProjectsAsync(ct);

        try
        {
            foreach (var algorithm in algorithms)
            {
                logger?.LogInfo($"applying {algorithm.Name}......");

                await algorithm.ObfuscateAsync(projects, ct);
            }

            logger?.LogInfo("all algorithms are applied");
        }
        catch (Exception e)
        {
            logger?.LogError("an error occurred, rollback all algorithms");
            logger?.LogDebug(e.Message);
        }

        var dict = new Dictionary<string, string>();

        foreach (var document in projects.SelectMany(w => w.Documents))
        {
            var node = await document.SyntaxTree.GetRootAsync(ct);
            var source = node.NormalizeWhitespace().ToFullString();

            dict.Add(document.Path, source);
        }

        return dict;
    }
}