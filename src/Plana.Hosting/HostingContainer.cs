// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Reflection;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Hosting.Abstractions;
using Plana.Logging.Abstractions;

namespace Plana.Hosting;

public class HostingContainer(DirectoryInfo root, ILogger? logger) : IHostingContainer
{
    private readonly List<(IPlanaPlugin, PlanaPluginAttribute)> _items = new();

    public IReadOnlyCollection<(IPlanaPlugin, PlanaPluginAttribute)> Items => _items.AsReadOnly();

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
            catch (Exception e)
            {
                logger?.LogError($"failed to load plugin: Name={library.Name}, Reason={e.Message}");
            }
        }

        logger?.LogDebug($"successfully loaded {_items.Count} instance(s)");

        var duplications = _items.GroupBy(w => w.Item2.Id).Where(w => w.Count() > 1).ToList();
        if (duplications.Count > 0)
        {
            foreach (var duplication in duplications)
                logger?.LogFatal($"Id={duplication.Key} is duplicated, the id must be unique");

            throw new InvalidOperationException();
        }

        return Task.CompletedTask;
    }

    private IEnumerable<(IPlanaPlugin Instance, PlanaPluginAttribute Attribute)> GetImplementations(Assembly assembly)
    {
        var count = 0;

        foreach (var t in assembly.GetExportedTypes())
            if (typeof(IPlanaPlugin).IsAssignableFrom(t))
            {
                var attr = t.GetCustomAttribute<PlanaPluginAttribute>();
                if (attr == null)
                    continue;

                if (Activator.CreateInstance(t) is not IPlanaPlugin instance)
                    continue;

                logger?.LogDebug($"activated: Type={t.FullName}, Id={attr.Id}");

                count++;

                yield return (instance, attr);
            }

        if (count == 0)
            throw new NotSupportedException($"can't find any type which implements {nameof(IPlanaPlugin)} in {assembly.GetName().Name} from {assembly.Location}");
    }
}