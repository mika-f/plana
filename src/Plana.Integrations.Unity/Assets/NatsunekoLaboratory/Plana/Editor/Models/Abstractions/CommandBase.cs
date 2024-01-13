// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

using NatsunekoLaboratory.Plana.Extensions;

using UnityEditor;

namespace NatsunekoLaboratory.Plana.Models.Abstractions
{
    internal class CommandBase
    {
        private const string Guid = "300a45c7c5134854a88ad90d52d3a119";
        private static readonly string Path;

        static CommandBase()
        {
            Path = AssetDatabase.GUIDToAssetPath(Guid);
        }

        public async Task<bool> Check()
        {
            try
            {
                await CreateProcess("dotnet", "--version").RunAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<(int ExitCode, string o)> RunAsync(string command, string arguments)
        {
            var process = CreateProcess("dotnet", $"{Path} {command} {arguments}");
            var o = new StringBuilder();

            // NOTE: StringBuilder is not thread-safe instance. But the output order of stdout and stderr is not important (as long as the order is maintained between stdout and stderr, it is fine)
            var code = await process.RunAsync(o, o);

            return (code, o.ToString());
        }

        protected static Process CreateProcess(string name, string arguments = null)
        {
            var si = new ProcessStartInfo
            {
                FileName = name,
                Arguments = arguments ?? "",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            return new Process { StartInfo = si, EnableRaisingEvents = true };
        }
    }
}