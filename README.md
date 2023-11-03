# Udon Obfuscator

Udon Obfuscator is a powerful obfuscator for UdonSharp, containing a variety of features which provide protection for your source code.

## Key Features

- cli and unity integration
- pluggable

## Official Plugins

- UdonObfuscator.Composition.ControlFlowFlattening
- UdonObfuscator.Composition.DeadCodeInjection
- UdonObfuscator.Composition.DisableConsoleOutput
- UdonObfuscator.Composition.NumbersToExpressions
- UdonObfuscator.Composition.RenameSymbols
- UdonObfuscator.Composition.ShuffleDeclarations
- UdonObfuscator.Composition.SourceMaps
- UdonObfuscator.Composition.SplitStrings
- UdonObfuscator.Composition.StringEncryption

## How to use

You can use Udon Obfuscator from an Unity GUI or Command-Line Tool.

### Command Line

```bash
# assembly level obfuscate
$ udon-obfuscator-cli obfuscate --workspace ./YourUdonSharpProject.csproj --plugins ./plugins/ --rename-symbols --control-flow-flattening --source-maps

# project level obfuscate
$ udon-obfuscator-cli obfuscate --workspace ./YourUnityProject.sln --plugins ./plugins/ --rename-symbols --control-flow-flattening --source-maps
```

### Unity Integration

Coming Soon...

## Develop Plugin

You can add obfuscate algorithm with plugin.

1. Create a C# project with .NET 8.
2. Add a new reference of `UdonObfuscator.Composition.Abstractions` to project
3. Create a new C# source with the following template:

```csharp
using UdonObfuscator.Composition.Abstractions;
using UdonObfuscator.Composition.Abstractions.Attributes;

namespace UdonObfuscator.Composition.YourFirstPlugin;

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
