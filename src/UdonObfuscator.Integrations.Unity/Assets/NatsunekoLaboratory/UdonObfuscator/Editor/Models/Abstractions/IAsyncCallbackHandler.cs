// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace NatsunekoLaboratory.UdonObfuscator.Models.Abstractions
{
    internal interface IAsyncCallbackHandler
    {
        void Next();

        void Abort();
    }
}