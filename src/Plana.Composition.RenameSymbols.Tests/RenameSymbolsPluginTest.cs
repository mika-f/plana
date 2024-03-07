// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Plana.Testing;

namespace Plana.Composition.RenameSymbols.Tests;

public partial class RenameSymbolsPluginTest
{
    [Fact]
    public async Task CanInstantiateWithDefaults()
    {
        var container = new PlanaContainer<RenameSymbolsPlugin>();
        var instance = await container.InstantiateWithBind();

        Assert.NotNull(instance);
        Assert.Equal("Rename Symbols", instance.Name);
        Assert.Equal(7, instance.Options.Count);

        Assert.False(instance.IsEnableClassNameRenaming);
        Assert.False(instance.IsEnableFieldsRenaming);
        Assert.False(instance.IsEnableMethodsRenaming);
        Assert.False(instance.IsEnableNamespaceRenaming);
        Assert.False(instance.IsEnablePropertiesRenaming);
        Assert.False(instance.IsEnableVariablesRenaming);
        Assert.False(instance.KeepOriginalNameInInspector);
    }
}