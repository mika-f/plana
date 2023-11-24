// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.IO;

namespace NatsunekoLaboratory.UdonObfuscator.Extensions
{
    internal static class FileAttributesExtensions
    {
        public static bool IsFile(this FileAttributes obj)
        {
            return !obj.IsDirectory();
        }

        public static bool IsDirectory(this FileAttributes obj)
        {
            return obj.HasFlag(FileAttributes.Directory);
        }
    }
}