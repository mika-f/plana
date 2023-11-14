// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace NatsunekoLaboratory.UdonObfuscator.Components.Abstractions
{
    internal class StyledComponents
    {
        public string Uxml { get; private set; }

        public string Uss { get; private set; }

        public static StyledComponents Create(string uxml, string uss)
        {
            return Create(uxml, uxml, uss, uss);
        }

        public static StyledComponents Create(string devUxml, string prodUxml, string devUss, string prodUss)
        {
#if USTYLED
            return new StyledComponents
            {
                Uxml = devUxml,
                Uss = devUss
            };
#else
            return new StyledComponents
            {
                Uxml = prodUxml,
                Uss = prodUss
            };
#endif
        }
    }
}