using System;
using System.Runtime.InteropServices;

namespace PCSC.Interop.MacOSX
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal class SCARD_IO_REQUEST
    {
        internal int dwProtocol; // Protocol identifier
        internal int cbPciLength; // Protocol Control Inf Length
    }
}
