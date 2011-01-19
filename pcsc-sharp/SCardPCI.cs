/*
Copyright (c) 2010 Daniel Mueller <daniel@danm.de>
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

Changes to this license can be made only by the copyright author with
explicit written consent.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;
using PCSC.Interop;

namespace PCSC
{

    public class SCardPCI : IDisposable
    {
        static IntPtr pci_t0 = IntPtr.Zero;
        static IntPtr pci_t1 = IntPtr.Zero;
        static IntPtr pci_raw = IntPtr.Zero;
        internal WinSCardAPI.SCARD_IO_REQUEST winscard_iorequest = new WinSCardAPI.SCARD_IO_REQUEST();
        internal PCSCliteAPI.SCARD_IO_REQUEST pcsclite_iorequest = new PCSCliteAPI.SCARD_IO_REQUEST();
        internal IntPtr iomem = IntPtr.Zero;

        public SCardPCI()
        {
            if (SCardAPI.IsWindows)
            {
                winscard_iorequest.dwProtocol = 0;
                winscard_iorequest.cbPciLength = 0;
            }
            else
            {
                pcsclite_iorequest.dwProtocol = IntPtr.Zero;
                pcsclite_iorequest.cbPciLength = IntPtr.Zero;
            }
        }

        public SCardPCI(SCardProtocol protocol, int buflength)
            : this()
        {
            if (buflength < 0)
                throw new System.ArgumentOutOfRangeException(
                    "buflength");

            if (SCardAPI.IsWindows)
            {
                iomem = unchecked((IntPtr)((long)Marshal.AllocCoTaskMem(buflength
                    + Marshal.SizeOf(typeof(WinSCardAPI.SCARD_IO_REQUEST)))));

                winscard_iorequest.dwProtocol = (Int32)protocol;
                winscard_iorequest.cbPciLength = (Int32)buflength;
                if (iomem != IntPtr.Zero)
                {
                    Marshal.StructureToPtr(
                        winscard_iorequest,
                        iomem,
                        false);
                }
            }
            else
            {
                iomem = unchecked((IntPtr)((long)Marshal.AllocCoTaskMem(buflength
                    + Marshal.SizeOf(typeof(PCSCliteAPI.SCARD_IO_REQUEST)))));

                pcsclite_iorequest.dwProtocol = (IntPtr)protocol;
                pcsclite_iorequest.cbPciLength = (IntPtr)buflength;
                if (iomem != IntPtr.Zero)
                {
                    Marshal.StructureToPtr(
                        pcsclite_iorequest,
                        iomem, 
                        false);
                }
            }
        }
        public SCardPCI(SCardProtocol protocol, byte[] pcidata)
            : this(protocol, pcidata.Length)
        {
            if (pcidata == null)
                throw new ArgumentNullException("pcidata");

            if (pcidata.Length > 0 && iomem != IntPtr.Zero)
            {
                if (SCardAPI.IsWindows)
                {
                    Marshal.Copy(pcidata, 0,
                        BufferStartAddr, 
                        pcidata.Length);
                }
                else
                {
                    Marshal.Copy(pcidata, 0,
                        BufferStartAddr, 
                        pcidata.Length);
                }
            }
        }

        ~SCardPCI()
        {
            this.Dispose();
        }
        public void Dispose()
        {
            if (iomem != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(
                    iomem);
            }
        }

        [DescriptionAttribute("Protocol identifier")]
        public SCardProtocol Protocol
        {
            get
            {
                if (iomem != IntPtr.Zero)
                    UpdateIoRequestHeader();

                if (SCardAPI.IsWindows)
                {
                    return (SCardProtocol)winscard_iorequest.dwProtocol;
                }
                else
                {
                    return (SCardProtocol)pcsclite_iorequest.dwProtocol;
                }
            }

        }

        [DescriptionAttribute("Protocol Control Inf Length")]
        public int PciLength
        {
            get
            {
                if (iomem != IntPtr.Zero)
                    UpdateIoRequestHeader();

                if (SCardAPI.IsWindows)
                {
                    return (int)winscard_iorequest.cbPciLength;
                }
                else
                {
                    return (int)pcsclite_iorequest.cbPciLength;
                }
            }
        }

        internal void UpdateIoRequestHeader()
        {
            if (SCardAPI.IsWindows)
            {
                winscard_iorequest = (WinSCardAPI.SCARD_IO_REQUEST)Marshal.PtrToStructure(
                    iomem, 
                    typeof(WinSCardAPI.SCARD_IO_REQUEST));
            }
            else
            {
                pcsclite_iorequest = (PCSCliteAPI.SCARD_IO_REQUEST)Marshal.PtrToStructure(
                    iomem, 
                    typeof(PCSCliteAPI.SCARD_IO_REQUEST));
            }
        }

        [DescriptionAttribute("PCI data")]
        public byte[] Data
        {
            get
            {
                byte[] data = null;

                if (iomem != IntPtr.Zero)
                {
                    // Return PCI header from memory
                    UpdateIoRequestHeader();

                    if (SCardAPI.IsWindows)
                    {
                        // Copy data buffer into managed byte-array.
                        if (winscard_iorequest.cbPciLength != 0)
                        {
                            data = new byte[(int)winscard_iorequest.cbPciLength];
                            Marshal.Copy(
                                BufferStartAddr, 
                                data,
                                0,
                                (int)winscard_iorequest.cbPciLength);
                        }
                    }
                    else
                    {
                        // Copy data buffer into managed byte-array.
                        if (pcsclite_iorequest.cbPciLength != IntPtr.Zero)
                        {
                            data = new byte[(int)pcsclite_iorequest.cbPciLength];
                            Marshal.Copy(
                                BufferStartAddr, // ugly hack because Mono has problems with IntPtr & 64bit
                                data,
                                0,
                                (int)pcsclite_iorequest.cbPciLength);
                        }
                    }
                }
                return data;
            }
        }

        internal IntPtr BufferStartAddr
        {
            get
            {
                if (SCardAPI.IsWindows)
                {
                    return unchecked((IntPtr)((long)iomem +
                        (long)Marshal.SizeOf(typeof(WinSCardAPI.SCARD_IO_REQUEST))));
                }
                else
                {
                    return unchecked((IntPtr)((long)iomem +
                        (long)Marshal.SizeOf(typeof(PCSCliteAPI.SCARD_IO_REQUEST))));
                }
            }
        }

        public static IntPtr T0
        {
            get
            {
                if (pci_t0 == IntPtr.Zero)
                    pci_t0 = SCardAPI.Lib.GetSymFromLib("g_rgSCardT0Pci");
                return pci_t0;

            }
        }

        public static IntPtr T1
        {
            get
            {
                if (pci_t1 == IntPtr.Zero)
                    pci_t1 = SCardAPI.Lib.GetSymFromLib("g_rgSCardT1Pci");
                return pci_t1;
            }
        }

        public static IntPtr Raw
        {
            get
            {
                if (pci_raw == IntPtr.Zero)
                    pci_raw = SCardAPI.Lib.GetSymFromLib("g_rgSCardRawPci");
                return pci_raw;
            }
        }

        public static IntPtr GetPci(SCardProtocol proto)
        {
            switch (proto) {
                case SCardProtocol.T0:
                    return T0;
                case SCardProtocol.T1:
                    return T1;
                case SCardProtocol.Raw:
                    return Raw;
                default:
                    throw new InvalidProtocolException(SCardError.InvalidValue, "Protocol not supported.");
            }
        }
    }
}
