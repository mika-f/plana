// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Reflection;

using UdonObfuscator.Composition.Abstractions;
using UdonObfuscator.Composition.Abstractions.Attributes;
using UdonObfuscator.Hosting.Abstractions;
using UdonObfuscator.Logging.Abstractions;

namespace UdonObfuscator.Hosting;

public class HostingContainer(DirectoryInfo root, ILogger? logger) : IHostingContainer
{
    private readonly List<(IObfuscatorAlgorithm, ObfuscatorAlgorithmAttribute)> _items = new();

    public Task ResolveAsync(CancellationToken ct)
    {
        logger?.LogDebug($"resolving plugins in: {root}");

        var libraries = root.GetFiles("*.dll");
        if (libraries.Length == 0)
        {
            logger?.LogDebug("no plugins found, successfully resolving task");
            return Task.CompletedTask;
        }

        logger?.LogDebug("one or more plugins found, resolving......");

        foreach (var library in libraries)
        {
            ct.ThrowIfCancellationRequested();

            logger?.LogDebug($"plugin found: {library.Name}");

            try
            {
                var context = new PluginLoadContext(library.FullName);
                var assembly = context.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(library.FullName)));
                var items = GetImplementations(assembly).ToList();
                _items.AddRange(items);

                logger?.LogInfo($"plugin loaded: {library.Name}");
            }
            catch (NotSupportedException e)
            {
                logger?.LogError($"failed to load plugin: {e.Message}");
            }
            catch
            {
                logger?.LogError($"failed to load plugin: {library.Name}");
            }
        }

        logger?.LogDebug($"successfully loaded {_items.Count} instance(s)");

        return Task.CompletedTask;
    }

    private IEnumerable<(IObfuscatorAlgorithm Instance, ObfuscatorAlgorithmAttribute Attribute)> GetImplementations(Assembly assembly)
    {
        var count = 0;

        foreach (var t in assembly.GetExportedTypes())
            if (typeof(IObfuscatorAlgorithm).IsAssignableFrom(t))
            {
                var attr = t.GetCustomAttribute<ObfuscatorAlgorithmAttribute>();
                if (attr == null)
                    continue;

                if (Activator.CreateInstance(t) is not IObfuscatorAlgorithm instance)
                    continue;

                logger?.LogDebug($"activated: {t.FullName}");

                count++;

                yield return (instance, attr);
            }

        if (count == 0)
            throw new NotSupportedException($"can't find any type which implements {nameof(IObfuscatorAlgorithm)} in {assembly.GetName().Name} from {assembly.Location}");
    }
}