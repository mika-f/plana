// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Microsoft.CodeAnalysis;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Enum;
using Plana.Logging.Abstractions;
using Plana.Workspace.Abstractions;

namespace Plana;

public class Obfuscator(IWorkspace workspace, List<IPlanaPlugin> instances, ILogger? logger)
{
    public async Task<Dictionary<string, string>> ObfuscateAsync(CancellationToken ct)
    {
        logger?.LogInfo($"obfuscate workspace with {instances.Count} instances(s), this may take a few minutes......");

        await workspace.ActivateWorkspaceAsync(ct);

        var projects = await workspace.GetProjectsAsync(ct);
        var solution = new PlanaSolution(projects);
        var context = new PlanaPluginRunContext(solution, RunKind.Obfuscate, ct);

        try
        {
            foreach (var instance in instances)
            {
                logger?.LogInfo($"applying {instance.Name}......");

                await instance.RunAsync(context);
            }

            logger?.LogInfo("all instances are applied");
        }
        catch (Exception e)
        {
            logger?.LogError("an error occurred, rollback all instances");
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