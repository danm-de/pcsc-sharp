using System;
using PCSC.Interop.Linux;
using PCSC.Interop.MacOSX;
using PCSC.Interop.Windows;

namespace PCSC.Interop
{
    internal enum PlatformType
    {
        Windows,
        Linux,
        MacOSX
    }
    
    /// <summary>
    /// Platform selector (Windows or UNIX)
    /// </summary>
    public static class Platform
    {
        internal static PlatformType Type { get; }

        
        /// <summary>
        /// Returns <c>true</c> if the operation system runs on Windows. <c>false</c> otherwise.
        /// </summary>
        public static bool IsWindows => Type == PlatformType.Windows;
        
        /// <summary>
        /// Platform smart card library.
        /// </summary>
        internal static ISCardApi Lib { get; }

        static Platform() {
            var platform = Environment.OSVersion.Platform;

            switch (platform) {
                case PlatformID.Unix:
                    if (Unix.Native.GetUnameSysName() == Unix.Native.OS_NAME_OSX) {
                        // Mono identifies MacOSX as Unix
                        goto case PlatformID.MacOSX;
                    }
                    
                    Type = PlatformType.Linux;
                    Lib = new PCSCliteLinux();
                    break;
                case PlatformID.MacOSX:
                    Type = PlatformType.MacOSX;
                    Lib = new PCSCliteMacOsX();
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    Type = PlatformType.Windows;
                    Lib = new WinSCardAPI();
                    break;
                default:
                    throw new NotSupportedException("Sorry, your OS platform is not supported.");
            }
        }
    }
}
