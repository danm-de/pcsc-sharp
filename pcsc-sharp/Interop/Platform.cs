using System;
using System.Text;
using PCSC.Interop.Unix;
using PCSC.Interop.Windows;

namespace PCSC.Interop
{
    /// <summary>
    /// Platform selector (Windows or UNIX)
    /// </summary>
    internal static class Platform
    {
        /// <summary>
        /// Returns <c>true</c> if the operation system runs on Windows. <c>false</c> otherwise.
        /// </summary>
        internal static bool IsWindows { get; }

        /// <summary>
        /// Platform smart card library.
        /// </summary>
        internal static ISCardAPI Lib { get; }

        static Platform() {
            var platform = Environment.OSVersion.Platform.ToString();

            if (platform.Contains("Win32") || platform.Contains("Win64")) {
                IsWindows = true;
                Lib = new WinSCardAPI {
                    TextEncoding = new UnicodeEncoding()
                };
                return;
            }

            IsWindows = false;
            Lib = new PCSCliteAPI {
                TextEncoding = new UTF8Encoding()
            };
        }
    }
}
