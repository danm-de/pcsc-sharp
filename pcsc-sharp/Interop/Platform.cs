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
        private static readonly bool _iswindows;
        private static readonly ISCardAPI _lib;
        
        /// <summary>
        /// Returns <c>true</c> if the operation system runs on Windows. <c>false</c> otherwise.
        /// </summary>
        internal static bool IsWindows {
            get { return _iswindows; }
        }

        /// <summary>
        /// Platform smart card library.
        /// </summary>
        internal static ISCardAPI Lib {
            get { return _lib; }
        }

        static Platform() {
            var platform = Environment.OSVersion.Platform.ToString();

            if (platform.Contains("Win32") || platform.Contains("Win64")) {
                _iswindows = true;
                _lib = new WinSCardAPI {
                    TextEncoding = new UnicodeEncoding()
                };
                return;
            }

            _iswindows = false;
            _lib = new PCSCliteAPI {
                TextEncoding = new UTF8Encoding()
            };
        }
    }
}
