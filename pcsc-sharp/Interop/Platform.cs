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
            var platform = Environment.OSVersion.Platform;

            if (
                platform == PlatformID.Win32S ||
                platform == PlatformID.Win32Windows ||
                platform == PlatformID.Win32NT ||
                platform == PlatformID.WinCE
            ) {
                IsWindows = true;
                Lib = new WinSCardAPI();
                return;
            }

            IsWindows = false;
            Lib = new PCSCliteAPI();
        }
    }
}
