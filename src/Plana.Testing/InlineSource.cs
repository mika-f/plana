// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Xunit;

namespace Plana.Testing;

public class InlineSource(string path, string? output, string? input) : ITestableObject<string>
{
    public Task HasDiffs()
    {
        Assert.NotEqual(output, input);

        return Task.CompletedTask;
    }

    public Task NoDiffs()
    {
        Assert.Equal(output, input);

        return Task.CompletedTask;
    }

    public Task ToMatchInlineSnapshot(string snapshot)
    {
        Assert.Equal(snapshot, output);

        return Task.CompletedTask;
    }
}