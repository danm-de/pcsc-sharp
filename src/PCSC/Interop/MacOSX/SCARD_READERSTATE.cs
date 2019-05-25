using System;
using System.Runtime.InteropServices;

namespace PCSC.Interop.MacOSX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SCARD_READERSTATE
    {
        internal IntPtr pszReader;
        internal IntPtr pvUserData;
        internal int dwCurrentState;
        internal int dwEventState;
        internal int cbAtr;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = PCSCliteMacOsX.MAX_ATR_SIZE)]
        internal byte[] rgbAtr;
    }
}
