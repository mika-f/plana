// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Analysis;
using Plana.Composition.Abstractions.Enum;
using Plana.Logging.Abstractions;
using Plana.Workspace.Abstractions;

namespace Plana;

public class Obfuscator(IWorkspace workspace, List<IPlanaPlugin> instances, ILogger? logger)
{
    public async Task<IReadOnlyCollection<IDocument>> ObfuscateAsync(CancellationToken ct)
    {
        logger?.LogInfo($"obfuscate workspace with {instances.Count} instances(s), this may take a few minutes......");

        return await RunAsync(RunKind.Obfuscate, new PlanaRandom(), new PlanaSecureRandom(), ct);
    }

    public async Task<IReadOnlyCollection<IDocument>> RunAsync(RunKind kind, IPlanaRandom r, IPlanaSecureRandom sr, CancellationToken ct)
    {
        try
        {
            foreach (var instance in instances)
            {
                var solution = await workspace.ToSolutionAsync(ct);
                var context = new PlanaPluginRunContext(solution, kind, r, sr, ct);

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

        var projects = await workspace.GetProjectsAsync(ct);
        return projects.SelectMany(w => w.Documents).ToList();
    }
}