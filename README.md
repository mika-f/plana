# Plana

Plana is a powerful general purpose obfuscator for C#, but optimized for Unity and UdonSharp, containing a variety of features which provide protection for your source code from the hackers.

## Key Features

- cli and unity integration
- support C#, Unity C#, and UdonSharp
- pluggable

## Official Plugins

- Plana.Composition.ControlFlowFlattening
- Plana.Composition.DeadCodeInjection
- Plana.Composition.DisableConsoleOutput
- Plana.Composition.NumbersToExpressions
- Plana.Composition.RenameSymbols
- Plana.Composition.ShuffleDeclarations
- Plana.Composition.SourceMaps
- Plana.Composition.SplitStrings
- Plana.Composition.StringEncryption

## How to use

You can use Plana from an Unity GUI or Command-Line Tool.

### Command Line

```bash
# assembly level obfuscate
$ udon-obfuscator-cli obfuscate --workspace ./YourUdonSharpProject.csproj --plugins ./plugins/ --rename-symbols --control-flow-flattening --source-maps

# project level obfuscate
$ udon-obfuscator-cli obfuscate --workspace ./YourUnityProject.sln --plugins ./plugins/ --rename-symbols --control-flow-flattening --source-maps
```

### Unity Integration

See https://docs.natsuneko.com/udon-obfuscator

## Develop Plugin

You can add obfuscate algorithm with plugin.

1. Create a C# project with .NET 8.
2. Add a new reference of `Plana.Composition.Abstractions` to project
3. Create a new C# source with the following template:

```csharp
using Plana.Composition.Abstractions;
using Plana.Composition.Abstractions.Attributes;

namespace Plana.Composition.YourFirstPlugin;

[ObfuscatorAlgorithm("your-first-plugin")]
public class YourFirstPlugin : IObfuscatorAlgorithm
{
    private static readonly ObfuscatorAlgorithmOption<bool> FlagOption = new("--flag", "this is flag option", () => true);

    private bool _isFlagged;

    public IReadOnlyCollection<IObfuscatorAlgorithmOption> Options => new List<IObfuscatorAlgorithmOption> { FlagOption }.AsReadOnly();

    public void BindParameters(IObfuscatorParameterBinder binder)
    {
        _isFlagged = binder.GetValue(FlagOption);
    }
}
```

## License

MIT by [@6jz](https://twitter.com/6jz)
