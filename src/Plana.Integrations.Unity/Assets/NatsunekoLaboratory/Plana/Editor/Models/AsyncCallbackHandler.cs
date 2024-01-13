// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;

using NatsunekoLaboratory.Plana.Models.Abstractions;

namespace NatsunekoLaboratory.Plana.Models
{
    internal class AsyncCallbackHandler : IAsyncCallbackHandler
    {
        private readonly TaskCompletionSource<bool> _task = new TaskCompletionSource<bool>();

        public void Next()
        {
            _task.SetResult(true);
        }

        public void Abort()
        {
            _task.SetException(new Exception("task aborted"));
        }

        public async Task WaitForCompleted()
        {
            await _task.Task;
        }
    }
}