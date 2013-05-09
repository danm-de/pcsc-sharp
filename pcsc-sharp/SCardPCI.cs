using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PCSC.Interop;
using SCARD_IO_REQUEST_WINDOWS = PCSC.Interop.Windows.SCARD_IO_REQUEST;
using SCARD_IO_REQUEST_UNIX = PCSC.Interop.Unix.SCARD_IO_REQUEST;

namespace PCSC
{
    public class SCardPCI: IDisposable
    {
        private static IntPtr _pciT0 = IntPtr.Zero;
        private static IntPtr _pciT1 = IntPtr.Zero;
        private static IntPtr _pciRaw = IntPtr.Zero;
        private SCARD_IO_REQUEST_WINDOWS _winscardIoRequest = new SCARD_IO_REQUEST_WINDOWS();
        private SCARD_IO_REQUEST_UNIX _pcscliteIoRequest = new SCARD_IO_REQUEST_UNIX();
        private IntPtr _iomem = IntPtr.Zero;

        public SCardPCI() {
            if (Platform.IsWindows) {
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

            if (Platform.IsWindows) {
                _iomem = unchecked((IntPtr) ((long) Marshal.AllocCoTaskMem(bufLength
                    + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_WINDOWS)))));

                _winscardIoRequest.dwProtocol = (Int32) protocol;
                _winscardIoRequest.cbPciLength = bufLength;
                if (_iomem != IntPtr.Zero) {
                    Marshal.StructureToPtr(_winscardIoRequest, _iomem, false);
                }
            } else {
                _iomem = unchecked((IntPtr) ((long) Marshal.AllocCoTaskMem(bufLength
                    + Marshal.SizeOf(typeof(Interop.Unix.SCARD_IO_REQUEST)))));

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
                if (Platform.IsWindows) {
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
        }

        protected virtual void Dispose(bool disposing) {
            // The MUST free unmanaged memory
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

                if (Platform.IsWindows) {
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

                if (Platform.IsWindows) {
                    return _winscardIoRequest.cbPciLength;
                }
                return (int) _pcscliteIoRequest.cbPciLength;
            }
        }

        private void UpdateIoRequestHeader() {
            if (Platform.IsWindows) {
                _winscardIoRequest = (SCARD_IO_REQUEST_WINDOWS) Marshal.PtrToStructure(
                    _iomem,
                    typeof(SCARD_IO_REQUEST_WINDOWS));
            } else {
                _pcscliteIoRequest = (Interop.Unix.SCARD_IO_REQUEST) Marshal.PtrToStructure(
                    _iomem,
                    typeof(Interop.Unix.SCARD_IO_REQUEST));
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

                if (Platform.IsWindows) {
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

        private IntPtr BufferStartAddr {
            get {
                if (Platform.IsWindows) {
                    return unchecked((IntPtr) ((long) _iomem +
                        Marshal.SizeOf(typeof(SCARD_IO_REQUEST_WINDOWS))));
                }
                return unchecked((IntPtr) ((long) _iomem +
                    Marshal.SizeOf(typeof(Interop.Unix.SCARD_IO_REQUEST))));
            }
        }

        public static IntPtr T0 {
            get {
                if (_pciT0 == IntPtr.Zero) {
                    _pciT0 = Platform.Lib.GetSymFromLib("g_rgSCardT0Pci");
                }
                return _pciT0;

            }
        }

        public static IntPtr T1 {
            get {
                if (_pciT1 == IntPtr.Zero) {
                    _pciT1 = Platform.Lib.GetSymFromLib("g_rgSCardT1Pci");
                }
                return _pciT1;
            }
        }

        public static IntPtr Raw {
            get {
                if (_pciRaw == IntPtr.Zero) {
                    _pciRaw = Platform.Lib.GetSymFromLib("g_rgSCardRawPci");
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

        internal IntPtr MemoryPtr {
            get { return _iomem; }
        }
    }
}
