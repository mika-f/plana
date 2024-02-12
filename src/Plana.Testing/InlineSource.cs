// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using Xunit;

namespace Plana.Testing;

public class InlineSource(string path, string source) : ITestableObject<string>
{
    public Task ToMatchInlineSnapshot(string snapshot)
    {
        Assert.Equal(snapshot, source);

        return Task.CompletedTask;
    }

    public string Source => source;
}