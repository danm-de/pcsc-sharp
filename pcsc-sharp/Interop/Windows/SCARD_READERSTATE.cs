using System;
using System.Runtime.InteropServices;

namespace PCSC.Interop.Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SCARD_READERSTATE
    {
        internal IntPtr pszReader;
        internal IntPtr pvUserData;
        internal int dwCurrentState;
        internal int dwEventState;
        internal int cbAtr;
        
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = WinSCardAPI.MAX_ATR_SIZE, ArraySubType = UnmanagedType.U1)] 
        internal byte[] rgbAtr;
    }
}