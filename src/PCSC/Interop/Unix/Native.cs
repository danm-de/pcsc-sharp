using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop.Unix
{
    internal static class Native
    {
        private const string C_LIB = "libc";
        public const string OS_NAME_OSX = "Darwin";

        public static string GetUnameSysName() {
            var utsNameBuffer = new byte[1000];

            if (uname(utsNameBuffer) != 0) return null;
            
            // Find the null terminator of the first string in struct utsname.
            var length = Array.IndexOf(utsNameBuffer, (byte) 0, 0, utsNameBuffer.Length);
            return Encoding.ASCII.GetString(utsNameBuffer, 0, length);

        }

        [DllImport(C_LIB, CharSet = CharSet.Ansi)]
        private static extern int uname(
            [Out] byte[] buffer);
    }
}
