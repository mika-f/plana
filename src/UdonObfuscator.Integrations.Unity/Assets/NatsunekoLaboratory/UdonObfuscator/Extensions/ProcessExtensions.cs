// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace NatsunekoLaboratory.UdonObfuscator.Extensions
{
    internal static class ProcessExtensions
    {
        public static Task<int> RunAsync(this Process obj, StringBuilder stdout = null, StringBuilder stderr = null)
        {
            var exit = new TaskCompletionSource<int>();
            var waitForStdoutCompleted = new TaskCompletionSource<object>();
            var waitForStderrCompleted = new TaskCompletionSource<object>();

            void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
            {
                if (e.Data != null)
                    stdout.AppendLine(e.Data);
                else
                    waitForStdoutCompleted.TrySetResult(null);
            }

            obj.StartInfo.RedirectStandardOutput = true;
            obj.StartInfo.RedirectStandardError = true;
            obj.OutputDataReceived += OnOutputDataReceived;
            obj.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data != null)
                    stderr?.AppendLine(e.Data);
                else
                    waitForStderrCompleted.TrySetResult(null);
            };

            obj.Exited += async (sender, args) =>
            {
                await waitForStdoutCompleted.Task;
                await waitForStderrCompleted.Task;

                if (obj.ExitCode == 0)
                    exit.TrySetResult(obj.ExitCode);
                else
                    exit.TrySetException(new Exception());
            };

            var ret = obj.Start();
            if (!ret)
                throw new InvalidOperationException();

            obj.BeginOutputReadLine();
            obj.BeginErrorReadLine();

            return exit.Task;
        }
    }
}