using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PCSC.Interop;

namespace PCSC
{
    public class SCardPCI : IDisposable
    {
        private static IntPtr _pciT0 = IntPtr.Zero;
        private static IntPtr _pciT1 = IntPtr.Zero;
        private static IntPtr _pciRaw = IntPtr.Zero;
        internal WinSCardAPI.SCARD_IO_REQUEST _winscardIoRequest = new WinSCardAPI.SCARD_IO_REQUEST();
        internal PCSCliteAPI.SCARD_IO_REQUEST _pcscliteIoRequest = new PCSCliteAPI.SCARD_IO_REQUEST();
        internal IntPtr _iomem = IntPtr.Zero;

        public SCardPCI() {
            if (SCardAPI.IsWindows) {
                _winscardIoRequest.dwProtocol = 0;
                _winscardIoRequest.cbPciLength = 0;
            } else {
                _pcscliteIoRequest.dwProtocol = IntPtr.Zero;
                _pcscliteIoRequest.cbPciLength = IntPtr.Zero;
            }
        }

        public SCardPCI(SCardProtocol protocol, int bufLength)
            : this() 
        {
            if (bufLength < 0) {
                throw new ArgumentOutOfRangeException(
                    "bufLength");
            }

            if (SCardAPI.IsWindows) {
                _iomem = unchecked((IntPtr) ((long) Marshal.AllocCoTaskMem(bufLength
                    + Marshal.SizeOf(typeof(WinSCardAPI.SCARD_IO_REQUEST)))));

                _winscardIoRequest.dwProtocol = (Int32) protocol;
                _winscardIoRequest.cbPciLength = bufLength;
                if (_iomem != IntPtr.Zero) {
                    Marshal.StructureToPtr(
                        _winscardIoRequest,
                        _iomem,
                        false);
                }
            } else {
                _iomem = unchecked((IntPtr) ((long) Marshal.AllocCoTaskMem(bufLength
                    + Marshal.SizeOf(typeof(PCSCliteAPI.SCARD_IO_REQUEST)))));

                _pcscliteIoRequest.dwProtocol = (IntPtr) protocol;
                _pcscliteIoRequest.cbPciLength = (IntPtr) bufLength;
                if (_iomem != IntPtr.Zero) {
                    Marshal.StructureToPtr(
                        _pcscliteIoRequest,
                        _iomem,
                        false);
                }
            }
        }

        public SCardPCI(SCardProtocol protocol, byte[] pciData)
            : this(protocol, pciData.Length) {
            if (pciData == null)
                throw new ArgumentNullException("pciData");

            if (pciData.Length > 0 && _iomem != IntPtr.Zero) {
                if (SCardAPI.IsWindows) {
                    Marshal.Copy(pciData, 0,
                        BufferStartAddr,
                        pciData.Length);
                } else {
                    Marshal.Copy(pciData, 0,
                        BufferStartAddr,
                        pciData.Length);
                }
            }
        }

        ~SCardPCI() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (_iomem != IntPtr.Zero) {
                Marshal.FreeCoTaskMem(
                    _iomem);
                _iomem = IntPtr.Zero;
            }
        }

        [DescriptionAttribute("Protocol identifier")]
        public SCardProtocol Protocol {
            get {
                if (_iomem != IntPtr.Zero)
                    UpdateIoRequestHeader();

                if (SCardAPI.IsWindows) {
                    return (SCardProtocol) _winscardIoRequest.dwProtocol;
                }
                return (SCardProtocol) _pcscliteIoRequest.dwProtocol;
            }

        }

        [DescriptionAttribute("Protocol Control Inf Length")]
        public int PciLength {
            get {
                if (_iomem != IntPtr.Zero)
                    UpdateIoRequestHeader();

                if (SCardAPI.IsWindows) {
                    return _winscardIoRequest.cbPciLength;
                }
                return (int) _pcscliteIoRequest.cbPciLength;
            }
        }

        internal void UpdateIoRequestHeader() {
            if (SCardAPI.IsWindows) {
                _winscardIoRequest = (WinSCardAPI.SCARD_IO_REQUEST) Marshal.PtrToStructure(
                    _iomem,
                    typeof(WinSCardAPI.SCARD_IO_REQUEST));
            } else {
                _pcscliteIoRequest = (PCSCliteAPI.SCARD_IO_REQUEST) Marshal.PtrToStructure(
                    _iomem,
                    typeof(PCSCliteAPI.SCARD_IO_REQUEST));
            }
        }

        [DescriptionAttribute("PCI data")]
        public byte[] Data {
            get {
                byte[] data = null;

                if (_iomem == IntPtr.Zero) {
                    return null;
                }

                // Return PCI header from memory
                UpdateIoRequestHeader();

                if (SCardAPI.IsWindows) {
                    // Copy data buffer into managed byte-array.
                    if (_winscardIoRequest.cbPciLength != 0) {
                        data = new byte[_winscardIoRequest.cbPciLength];
                        Marshal.Copy(
                            BufferStartAddr,
                            data,
                            0,
                            _winscardIoRequest.cbPciLength);
                    }
                } else {
                    // Copy data buffer into managed byte-array.
                    if (_pcscliteIoRequest.cbPciLength != IntPtr.Zero) {
                        data = new byte[(int) _pcscliteIoRequest.cbPciLength];
                        Marshal.Copy(
                            BufferStartAddr, // ugly hack because Mono has problems with IntPtr & 64bit
                            data,
                            0,
                            (int) _pcscliteIoRequest.cbPciLength);
                    }
                }
                return data;
            }
        }

        internal IntPtr BufferStartAddr {
            get {
                if (SCardAPI.IsWindows) {
                    return unchecked((IntPtr) ((long) _iomem +
                        Marshal.SizeOf(typeof(WinSCardAPI.SCARD_IO_REQUEST))));
                }
                return unchecked((IntPtr) ((long) _iomem +
                    Marshal.SizeOf(typeof(PCSCliteAPI.SCARD_IO_REQUEST))));
            }
        }

        public static IntPtr T0 {
            get {
                if (_pciT0 == IntPtr.Zero) {
                    _pciT0 = SCardAPI.Lib.GetSymFromLib("g_rgSCardT0Pci");
                }
                return _pciT0;

            }
        }

        public static IntPtr T1 {
            get {
                if (_pciT1 == IntPtr.Zero) {
                    _pciT1 = SCardAPI.Lib.GetSymFromLib("g_rgSCardT1Pci");
                }
                return _pciT1;
            }
        }

        public static IntPtr Raw {
            get {
                if (_pciRaw == IntPtr.Zero) {
                    _pciRaw = SCardAPI.Lib.GetSymFromLib("g_rgSCardRawPci");
                }
                return _pciRaw;
            }
        }

        public static IntPtr GetPci(SCardProtocol proto) {
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
