// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace NatsunekoLaboratory.UdonObfuscator.Extensions
{
    internal static class ProcessExtensions
    {
        public static Task<int> RunAsync(this Process obj)
        {
            var exit = new TaskCompletionSource<int>();

            obj.StartInfo.RedirectStandardOutput = true;
            obj.Exited += (sender, args) => exit.TrySetResult(obj.ExitCode);

            var ret = obj.Start();
            if (ret)
                return exit.Task;

            throw new InvalidOperationException();
        }
    }
}