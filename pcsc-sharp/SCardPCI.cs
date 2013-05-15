using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PCSC.Interop;
using SCARD_IO_REQUEST_WINDOWS = PCSC.Interop.Windows.SCARD_IO_REQUEST;
using SCARD_IO_REQUEST_UNIX = PCSC.Interop.Unix.SCARD_IO_REQUEST;

namespace PCSC
{
    /// <summary>Structure of protocol control information.</summary>
    /// <remarks>
    ///     <para>Is a structure containing the following:</para>
    ///     <para>
    ///         <example>
    ///             <code lang="C">
    /// typedef struct {
    /// 	DWORD dwProtocol;    // SCARD_PROTOCOL_T0 or SCARD_PROTOCOL_T1
    /// 	DWORD cbPciLength;   // Length of this structure - not used
    /// } SCARD_IO_REQUEST;
    /// </code>
    ///         </example>
    ///     </para>
    ///     <para>The pointers to the pre-defined / built-in PCI structures are determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</para>
    /// </remarks>
    public class SCardPCI : IDisposable
    {
        private static IntPtr _pciT0 = IntPtr.Zero;
        private static IntPtr _pciT1 = IntPtr.Zero;
        private static IntPtr _pciRaw = IntPtr.Zero;
        private SCARD_IO_REQUEST_WINDOWS _winscardIoRequest = new SCARD_IO_REQUEST_WINDOWS();
        private SCARD_IO_REQUEST_UNIX _pcscliteIoRequest = new SCARD_IO_REQUEST_UNIX();
        private IntPtr _iomem = IntPtr.Zero;

        /// <summary>Destroys the object and frees unmanaged memory.</summary>
        ~SCardPCI() {
            Dispose(false);
        }

        /// <summary>Initializes a new instance of the <see cref="SCardPCI" /> class.</summary>
        public SCardPCI() {
            if (Platform.IsWindows) {
                _winscardIoRequest.dwProtocol = 0;
                _winscardIoRequest.cbPciLength = 0;
            } else {
                _pcscliteIoRequest.dwProtocol = IntPtr.Zero;
                _pcscliteIoRequest.cbPciLength = IntPtr.Zero;
            }
        }

        /// <summary>Creates a new SCardPCI object.</summary>
        /// <param name="protocol">
        ///     <list type="table"><listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list>
        /// </param>
        /// <param name="bufLength">Size of this structure in bytes.</param>
        public SCardPCI(SCardProtocol protocol, int bufLength)
            : this() {
            if (bufLength < 0) {
                throw new ArgumentOutOfRangeException(
                    "bufLength");
            }

            if (Platform.IsWindows) {
                // Windows
                _iomem = unchecked((IntPtr) ((long) Marshal.AllocCoTaskMem(bufLength
                    + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_WINDOWS)))));

                _winscardIoRequest.dwProtocol = (Int32) protocol;
                _winscardIoRequest.cbPciLength = bufLength;
                if (_iomem != IntPtr.Zero) {
                    Marshal.StructureToPtr(_winscardIoRequest, _iomem, false);
                }
                return;
            }

            // Unix
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

        /// <summary>Creates a new SCardPCI object.</summary>
        /// <param name="protocol">
        ///     <list type="table">
        ///         <listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list>
        /// </param>
        /// <param name="pciData">User data.</param>
        public SCardPCI(SCardProtocol protocol, byte[] pciData)
            : this(protocol, pciData.Length) {
            if (pciData == null) {
                throw new ArgumentNullException("pciData");
            }

            if (pciData.Length <= 0 || _iomem == IntPtr.Zero) {
                return;
            }

            if (Platform.IsWindows) {
                // Windows
                Marshal.Copy(pciData, 0,
                    BufferStartAddr,
                    pciData.Length);

                return;
            }

            // Unix
            Marshal.Copy(pciData, 0,
                BufferStartAddr,
                pciData.Length);
        }

        /// <summary>Disposes the instance and frees unmanaged memory.</summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the instance and frees unmanaged memory.
        /// </summary>
        /// <param name="disposing">Ignored</param>
        protected virtual void Dispose(bool disposing) {
            // The MUST free unmanaged memory
            if (_iomem != IntPtr.Zero) {
                Marshal.FreeCoTaskMem(
                    _iomem);
                _iomem = IntPtr.Zero;
            }
        }

        /// <summary>Protocol</summary>
        /// <value>
        ///     <list type="table">
        ///         <listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list>
        /// </value>
        [DescriptionAttribute("Protocol identifier")]
        public SCardProtocol Protocol {
            get {
                if (_iomem != IntPtr.Zero) {
                    UpdateIoRequestHeader();
                }

                if (Platform.IsWindows) {
                    return (SCardProtocol) _winscardIoRequest.dwProtocol;
                }
                return (SCardProtocol) _pcscliteIoRequest.dwProtocol;
            }
        }

        /// <summary>Size of this structure in bytes.</summary>
        [DescriptionAttribute("Protocol Control Inf Length")]
        public int PciLength {
            get {
                if (_iomem != IntPtr.Zero) {
                    UpdateIoRequestHeader();
                }

                if (Platform.IsWindows) {
                    return _winscardIoRequest.cbPciLength;
                }
                return (int) _pcscliteIoRequest.cbPciLength;
            }
        }

        private void UpdateIoRequestHeader() {
            if (Platform.IsWindows) {
                // Windows
                _winscardIoRequest = (SCARD_IO_REQUEST_WINDOWS) Marshal.PtrToStructure(
                    _iomem,
                    typeof(SCARD_IO_REQUEST_WINDOWS));
                return;
            }

            // Unix
            _pcscliteIoRequest = (Interop.Unix.SCARD_IO_REQUEST) Marshal.PtrToStructure(
                _iomem,
                typeof(Interop.Unix.SCARD_IO_REQUEST));
        }

        /// <summary>User data.</summary>
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
                    return data;
                }

                // Copy data buffer into managed byte-array.
                if (_pcscliteIoRequest.cbPciLength != IntPtr.Zero) {
                    data = new byte[(int) _pcscliteIoRequest.cbPciLength];
                    Marshal.Copy(
                        BufferStartAddr, // ugly hack because Mono has problems with IntPtr & 64bit
                        data,
                        0,
                        (int) _pcscliteIoRequest.cbPciLength);
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

        /// <summary>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</summary>
        /// <value>A pointer to the C structure in the system library.</value>
        /// <remarks>This pointer to the pre-defined / built-in PCI structure is determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</remarks>
        public static IntPtr T0 {
            get {
                if (_pciT0 == IntPtr.Zero) {
                    _pciT0 = Platform.Lib.GetSymFromLib("g_rgSCardT0Pci");
                }
                return _pciT0;
            }
        }

        /// <summary>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</summary>
        /// <value>A pointer to the C structure in the system library.</value>
        /// <remarks>This pointer to the pre-defined / built-in PCI structure is determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</remarks>
        public static IntPtr T1 {
            get {
                if (_pciT1 == IntPtr.Zero) {
                    _pciT1 = Platform.Lib.GetSymFromLib("g_rgSCardT1Pci");
                }
                return _pciT1;
            }
        }

        /// <summary>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</summary>
        /// <value>A pointer to the C structure in the system library.</value>
        /// <remarks>This pointer to the pre-defined / built-in PCI structure is determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</remarks>
        public static IntPtr Raw {
            get {
                if (_pciRaw == IntPtr.Zero) {
                    _pciRaw = Platform.Lib.GetSymFromLib("g_rgSCardRawPci");
                }
                return _pciRaw;
            }
        }

        /// <summary>Receives a PCI pointer to a given protocol.</summary>
        /// <param name="protocol">The desired protocol.</param>
        /// <returns>A pointer to the PCI structure in the native system library.</returns>
        public static IntPtr GetPci(SCardProtocol protocol) {
            switch (protocol) {
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