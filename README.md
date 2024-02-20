# Plana

Plana is a powerful general purpose obfuscator for C#, but optimized for Unity and UdonSharp, containing a variety of features which provide protection for your source code from the hackers.

## Key Features

- cli and unity integration
- support C#, Unity C#, and UdonSharp
- pluggable

## Official Plugins

- (Planned) Plana.Composition.ControlFlowFlattening
- (Planned) Plana.Composition.DeadCodeInjection
- Plana.Composition.DisableConsoleOutput
- (Planned) Plana.Composition.NumbersToExpressions
- Plana.Composition.RenameSymbols
- Plana.Composition.ShuffleDeclarations
- (Planned) Plana.Composition.SourceMaps
- (Planned) Plana.Composition.SplitStrings
- (Planned) Plana.Composition.StringEncryption

## How to use

You can use Plana from any integrations.

### Command Line

```bash
# assembly level obfuscate
$ plana-cli obfuscate --workspace ./YourUdonSharpProject.csproj --plugins ./plugins/ --rename-symbols --control-flow-flattening --source-maps

# project level obfuscate
$ plana-cli obfuscate --workspace ./YourUnityProject.sln --plugins ./plugins/ --rename-symbols --control-flow-flattening --source-maps
```

Please visit https://docs.natsuneko.com/plana/integrations/cli for more information.

### Unity Integration

See https://docs.natsuneko.com/plana/integrations/unity

### Desktop Integration (for Windows and Linux)

See https://docs.natsuneko.com/plana/integrations/desktop

## Develop Plugin

You can add any behaviour to Plana, by plugin system.

1. create a C# project with .NET 8.
2. add a new reference of `Plana.Composition.Abstractions` to project
   1. also add `Plana.Composition.Extensions` for useful extension methods
3. create a new C# source with the following template:

```csharp
using System.Runtime.CompilerServices;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;
using Plana.Composition.Extensions;

[assembly: InternalsVisibleTo("Plana.Composition.YourFirstPlugin.Tests")]

namespace Plana.Composition.YourFirstPlugin;

[PlanaPlugin("your-first-plugin")]
public class YourFirstPlugin : IPlanaPlugin2
{
    private static readonly PlanaPluginOption Flag = new("flag", "Flag Option", "this is flag option", false);

    internal bool IsFlagged;

    public IReadOnlyCollection<IPlanaPluginOption> Options => new List<IPlanaPluginOption> { Flag }.AsReadOnly();

    public string Name => "Your First Plugin";

    public void BindParameters(IPlanaPluginParameterBinder binder)
    {
        IsFlagged = binder.GetValue(FlagOption);
    }

    public async Task ObfuscateAsync(IPlanaPluginRunContext context)
    {
        foreach (var document in context.Solution.Projects.SelectMany(w => w.Documents))
        {
            var oldNode = await document.SyntaxTree.GetRootAsync(context.CancellationToken);
            var newNode = // create a new node

            await document.ApplyChangesAsync(newNode, context.CancellationToken);
        }
    }
}
```

4. add the following lines to csproj:

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <!-- required -->
  <PropertyGroup>
    <EnableDynamicLoading>true</EnableDynamicLoading>
  </PropertyGroup>

  <!-- recommended for developing plugins -->
  <PropertyGroup>
    <BuildOnCI>$(CI)</BuildOnCI>
  </PropertyGroup>

  <Target Name="PostBuild" AfterTargets="PostBuildEvent">
    <Exec Command="pwsh -Command New-Item -Force -ItemType Directory '/path/to/plana/plugins/'" Condition="'$(BuildOnCI)' == ''" />
    <Exec Command="pwsh -Command Copy-Item '$(TargetDir)$(ProjectName).dll' '/path/to/plana/plugins/$(ProjectName).dll'" Condition="'$(BuildOnCI)' == ''" />
    <Exec Command="pwsh -Command Copy-Item '$(TargetDir)$(ProjectName).dll' '/path/to/plana/plugins/$(ProjectName).dll'" Condition="'$(Configuration)' == 'Release' And '$(BuildOnCI)' == ''" />
  </Target>

</Project>
```

## License

MIT by [@6jz](https://twitter.com/6jz)
