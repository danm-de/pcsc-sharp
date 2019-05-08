using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using PCSC.Exceptions;
using PCSC.Interop;
using SCARD_IO_REQUEST_WINDOWS = PCSC.Interop.Windows.SCARD_IO_REQUEST;
using SCARD_IO_REQUEST_UNIX = PCSC.Interop.Linux.SCARD_IO_REQUEST;
using SCARD_IO_REQUEST_MACOSX = PCSC.Interop.MacOSX.SCARD_IO_REQUEST;

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
        private SCARD_IO_REQUEST_UNIX _linuxIoRequest = new SCARD_IO_REQUEST_UNIX();
        private SCARD_IO_REQUEST_MACOSX _macosIoRequest = new SCARD_IO_REQUEST_MACOSX();

        /// <summary>Destroys the object and frees unmanaged memory.</summary>
        ~SCardPCI() {
            Dispose(false);
        }

        /// <summary>Initializes a new instance of the <see cref="SCardPCI" /> class.</summary>
        public SCardPCI() {
            switch (Platform.Type) {
                case PlatformType.Windows:
                    _winscardIoRequest.dwProtocol = 0;
                    _winscardIoRequest.cbPciLength = 0;
                    break;
                case PlatformType.Linux:
                    _linuxIoRequest.dwProtocol = IntPtr.Zero;
                    _linuxIoRequest.cbPciLength = IntPtr.Zero;
                    break;
                case PlatformType.MacOSX:
                    _linuxIoRequest.dwProtocol = IntPtr.Zero;
                    _linuxIoRequest.cbPciLength = IntPtr.Zero;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>Creates a new SCardPCI object.</summary>
        /// <param name="protocol">
        ///     <list type="table"><listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list>
        /// </param>
        /// <param name="bufLength">Size of this structure in bytes.</param>
        public SCardPCI(SCardProtocol protocol, int bufLength)
            : this() {
            if (bufLength < 0) {
                throw new ArgumentOutOfRangeException(
                    nameof(bufLength));
            }

            switch (Platform.Type) {
                case PlatformType.Windows:
                    MemoryPtr = unchecked((IntPtr) ((long) Marshal
                            .AllocCoTaskMem(bufLength + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_WINDOWS)))
                        ));
                    _winscardIoRequest.dwProtocol = (int) protocol;
                    _winscardIoRequest.cbPciLength = bufLength;
                    if (MemoryPtr != IntPtr.Zero) {
                        Marshal.StructureToPtr(_winscardIoRequest, MemoryPtr, false);
                    }

                    break;
                case PlatformType.Linux:
                    MemoryPtr = unchecked((IntPtr) ((long) Marshal
                        .AllocCoTaskMem(bufLength + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_UNIX)))));
                    _linuxIoRequest.dwProtocol = (IntPtr) protocol;
                    _linuxIoRequest.cbPciLength = (IntPtr) bufLength;
                    if (MemoryPtr != IntPtr.Zero) {
                        Marshal.StructureToPtr(
                            _linuxIoRequest,
                            MemoryPtr,
                            false);
                    }

                    break;
                case PlatformType.MacOSX:
                    MemoryPtr = unchecked((IntPtr) ((long) Marshal
                            .AllocCoTaskMem(bufLength + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_MACOSX))))
                    );
                    _macosIoRequest.dwProtocol = (IntPtr) protocol;
                    _macosIoRequest.cbPciLength = (IntPtr) bufLength;
                    if (MemoryPtr != IntPtr.Zero) {
                        Marshal.StructureToPtr(
                            _macosIoRequest,
                            MemoryPtr,
                            false);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>Creates a new SCardPCI object.</summary>
        /// <param name="protocol">
        ///     <list type="table">
        ///         <listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list>
        /// </param>
        /// <param name="pciData">User data.</param>
        public SCardPCI(SCardProtocol protocol, byte[] pciData)
            : this(protocol, pciData.Length) {
            if (pciData == null) {
                throw new ArgumentNullException(nameof(pciData));
            }

            if (pciData.Length <= 0 || MemoryPtr == IntPtr.Zero) {
                return;
            }

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
            if (MemoryPtr != IntPtr.Zero) {
                Marshal.FreeCoTaskMem(
                    MemoryPtr);
                MemoryPtr = IntPtr.Zero;
            }
        }

        /// <summary>Protocol</summary>
        /// <value>
        ///     <list type="table">
        ///         <listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list>
        /// </value>
        [Description("Protocol identifier")]
        public SCardProtocol Protocol {
            get {
                if (MemoryPtr != IntPtr.Zero) {
                    UpdateIoRequestHeader();
                }

                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return (SCardProtocol) _winscardIoRequest.dwProtocol;
                    case PlatformType.Linux:
                        return (SCardProtocol) _linuxIoRequest.dwProtocol;
                    case PlatformType.MacOSX:
                        return (SCardProtocol) _macosIoRequest.dwProtocol;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>Size of this structure in bytes.</summary>
        [Description("Protocol Control Inf Length")]
        public int PciLength {
            get {
                if (MemoryPtr != IntPtr.Zero) {
                    UpdateIoRequestHeader();
                }

                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return _winscardIoRequest.cbPciLength;
                    case PlatformType.Linux:
                        return unchecked((int) (long) _linuxIoRequest.cbPciLength);
                    case PlatformType.MacOSX:
                        return unchecked((int) (long) _macosIoRequest.cbPciLength);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private void UpdateIoRequestHeader() {
            switch (Platform.Type) {
                case PlatformType.Windows:
                    _winscardIoRequest = (SCARD_IO_REQUEST_WINDOWS) Marshal.PtrToStructure(
                        MemoryPtr,
                        typeof(SCARD_IO_REQUEST_WINDOWS));
                    break;
                case PlatformType.Linux:
                    _linuxIoRequest = (SCARD_IO_REQUEST_UNIX) Marshal.PtrToStructure(
                        MemoryPtr,
                        typeof(SCARD_IO_REQUEST_UNIX));
                    break;
                case PlatformType.MacOSX:
                    _macosIoRequest = (SCARD_IO_REQUEST_MACOSX) Marshal.PtrToStructure(
                        MemoryPtr,
                        typeof(SCARD_IO_REQUEST_UNIX));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>User data.</summary>
        [Description("PCI data")]
        public byte[] Data {
            get {
                byte[] data = null;

                if (MemoryPtr == IntPtr.Zero) {
                    return null;
                }

                // Return PCI header from memory
                UpdateIoRequestHeader();

                switch (Platform.Type) {
                    case PlatformType.Windows:
                        if (_winscardIoRequest.cbPciLength != 0) {
                            data = new byte[_winscardIoRequest.cbPciLength];
                            Marshal.Copy(
                                BufferStartAddr,
                                data,
                                0,
                                _winscardIoRequest.cbPciLength);
                        }

                        return data;
                    case PlatformType.Linux:
                        if (_linuxIoRequest.cbPciLength != IntPtr.Zero) {
                            data = new byte[(int) _linuxIoRequest.cbPciLength];
                            Marshal.Copy(
                                BufferStartAddr, // ugly hack because Mono has problems with IntPtr & 64bit
                                data,
                                0,
                                unchecked((int) (long) _linuxIoRequest.cbPciLength));
                        }

                        return data;
                    case PlatformType.MacOSX:
                        if (_macosIoRequest.cbPciLength != IntPtr.Zero) {
                            data = new byte[(int) _macosIoRequest.cbPciLength];
                            Marshal.Copy(
                                BufferStartAddr, // ugly hack because Mono has problems with IntPtr & 64bit
                                data,
                                0,
                                unchecked((int) (long) _macosIoRequest.cbPciLength));
                        }

                        return data;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private IntPtr BufferStartAddr {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return unchecked((IntPtr) (
                            (long) MemoryPtr + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_WINDOWS))));
                    case PlatformType.Linux:
                        return unchecked((IntPtr) (
                            (long) MemoryPtr + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_UNIX))));
                    case PlatformType.MacOSX:
                        return unchecked((IntPtr) (
                            (long) MemoryPtr + Marshal.SizeOf(typeof(SCARD_IO_REQUEST_MACOSX))));
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
                    throw new InvalidProtocolException(SCardError.InvalidValue,
                        $"Protocol '{protocol}' not supported.");
            }
        }

        internal IntPtr MemoryPtr { get; private set; } = IntPtr.Zero;
    }
}
