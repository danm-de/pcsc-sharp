using System;
using System.Runtime.InteropServices;

namespace PCSC.Interop.Windows
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal class SCARD_IO_REQUEST
    {
        internal int dwProtocol;
        internal int cbPciLength;
        internal SCARD_IO_REQUEST()
        {
            dwProtocol = 0;
        }
    }
}