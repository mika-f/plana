// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.IO;

using UnityEngine;

namespace NatsunekoLaboratory.UdonObfuscator.Extensions
{
    internal static class FileSystemInfoExtensions
    {
        public static string ToRelativePath(this FileSystemInfo obj)
        {
            var root = new Uri(Application.dataPath);
            var path = new Uri(obj.FullName);
            var rel = root.MakeRelativeUri(path);

            return rel.ToString();
        }
    }
}