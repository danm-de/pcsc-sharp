using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop.Windows
{
    /// <summary>
    /// PC/SC API for Microsoft Win32/Win64 (x86/x64/IA64) 
    /// </summary>
    internal sealed class WinSCardAPI : ISCardAPI
    {
        private const int MAX_READER_NAME = 255;
        private const string WINSCARD_DLL = "winscard.dll";
        private const string KERNEL_DLL = "KERNEL32.DLL";
        private const int CHARSIZE = sizeof(char);
        
        public const int MAX_ATR_SIZE = 0x24;

        private IntPtr _dllHandle = IntPtr.Zero;

        public int MaxAtrSize => MAX_ATR_SIZE;

        public Encoding TextEncoding { get; set; }

        public int CharSize => CHARSIZE;

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardEstablishContext(
            [In] int dwScope,
            [In] IntPtr pvReserved1,
            [In] IntPtr pvReserved2,
            [In, Out] ref IntPtr phContext);

        public SCardError EstablishContext(SCardScope dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out IntPtr phContext) {
            var ctx = IntPtr.Zero;
            var rc = SCardHelper.ToSCardError(
                SCardEstablishContext(
                    (int) dwScope,
                    pvReserved1,
                    pvReserved2,
                    ref ctx));
            phContext = ctx;
            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardReleaseContext(
            [In] IntPtr hContext);

        public SCardError ReleaseContext(IntPtr hContext) {
            return SCardHelper.ToSCardError(SCardReleaseContext(hContext));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardIsValidContext(
            [In] IntPtr hContext);

        public SCardError IsValidContext(IntPtr hContext) {
            return SCardHelper.ToSCardError(SCardIsValidContext(hContext));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardListReaders(
            [In] IntPtr hContext,
            [In] byte[] mszGroups,
            [Out] byte[] pmszReaders,
            [In, Out] ref int pcchReaders);

        public SCardError ListReaders(IntPtr hContext, string[] groups, out string[] readers) {
            var dwReaders = 0;

            // initialize groups array
            byte[] mszGroups = null;
            if (groups != null)
                mszGroups = SCardHelper.ConvertToByteArray(groups, TextEncoding);

            // determine the needed buffer size
            var rc = SCardHelper.ToSCardError(
                SCardListReaders(hContext,
                    mszGroups,
                    null,
                    ref dwReaders));

            if (rc != SCardError.Success) {
                readers = null;
                return rc;
            }

            // initialize array
            var mszReaders = new byte[dwReaders * sizeof(char)];

            rc = SCardHelper.ToSCardError(
                SCardListReaders(hContext,
                    mszGroups,
                    mszReaders,
                    ref dwReaders));

            readers = (rc == SCardError.Success)
                ? SCardHelper.ConvertToStringArray(mszReaders, TextEncoding)
                : null;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardListReaderGroups(
            [In] IntPtr hContext,
            [Out] byte[] mszGroups,
            [In, Out] ref int pcchGroups);

        public SCardError ListReaderGroups(IntPtr hContext, out string[] groups) {
            var dwGroups = 0;

            // determine the needed buffer size
            var rc = SCardHelper.ToSCardError(
                SCardListReaderGroups(
                    hContext,
                    null,
                    ref dwGroups));

            if (rc != SCardError.Success) {
                groups = null;
                return rc;
            }

            // initialize array
            var mszGroups = new byte[dwGroups * sizeof(char)];

            rc = SCardHelper.ToSCardError(
                SCardListReaderGroups(
                    hContext,
                    mszGroups,
                    ref dwGroups));

            groups = (rc == SCardError.Success)
                ? SCardHelper.ConvertToStringArray(mszGroups, TextEncoding)
                : null;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardConnect(
            [In] IntPtr hContext,
            [In] byte[] szReader,
            [In] int dwShareMode,
            [In] int dwPreferredProtocols,
            [Out] out IntPtr phCard,
            [Out] out int pdwActiveProtocol);

        public SCardError Connect(IntPtr hContext, string szReader, SCardShareMode dwShareMode, SCardProtocol dwPreferredProtocols, out IntPtr phCard, out SCardProtocol pdwActiveProtocol) {
            var readername = SCardHelper.ConvertToByteArray(szReader, TextEncoding, Platform.Lib.CharSize);
            int activeproto;

            var result = SCardConnect(hContext,
                readername,
                (int) dwShareMode,
                (int) dwPreferredProtocols,
                out phCard,
                out activeproto);

            pdwActiveProtocol = (SCardProtocol) activeproto;

            return SCardHelper.ToSCardError(result);
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardReconnect(
            [In] IntPtr hCard,
            [In] int dwShareMode,
            [In] int dwPreferredProtocols,
            [In] int dwInitialization,
            [Out] out int pdwActiveProtocol);

        public SCardError Reconnect(IntPtr hCard, SCardShareMode dwShareMode, SCardProtocol dwPreferredProtocols, SCardReaderDisposition dwInitialization, out SCardProtocol pdwActiveProtocol) {
            int activeproto;
            var result = SCardReconnect(
                hCard,
                (int) dwShareMode,
                (int) dwPreferredProtocols,
                (int) dwInitialization,
                out activeproto);

            pdwActiveProtocol = (SCardProtocol) activeproto;
            return SCardHelper.ToSCardError(result);
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardDisconnect(
            [In] IntPtr hCard,
            [In] int dwDisposition);

        public SCardError Disconnect(IntPtr hCard, SCardReaderDisposition dwDisposition) {
            return SCardHelper.ToSCardError(SCardDisconnect(hCard, (int) dwDisposition));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardBeginTransaction(
            [In] IntPtr hCard);

        public SCardError BeginTransaction(IntPtr hCard) {
            return SCardHelper.ToSCardError(SCardBeginTransaction(hCard));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardEndTransaction(
            [In] IntPtr hCard,
            [In] int dwDisposition);

        public SCardError EndTransaction(IntPtr hCard, SCardReaderDisposition dwDisposition) {
            return SCardHelper.ToSCardError(SCardEndTransaction(hCard, (int) dwDisposition));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardTransmit(
            [In] IntPtr hCard,
            [In] IntPtr pioSendPci,
            [In] byte[] pbSendBuffer,
            [In] int cbSendLength,
            [In, Out] IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            [In, Out] ref int pcbRecvLength);

        public SCardError Transmit(IntPtr hCard, IntPtr pioSendPci, byte[] pbSendBuffer, IntPtr pioRecvPci, byte[] pbRecvBuffer, out int pcbRecvLength) {
            pcbRecvLength = 0;
            if (pbRecvBuffer != null) {
                pcbRecvLength = pbRecvBuffer.Length;
            }

            var pcbSendLength = 0;
            if (pbSendBuffer != null) {
                pcbSendLength = pbSendBuffer.Length;
            }

            return Transmit(
                hCard,
                pioSendPci,
                pbSendBuffer,
                pcbSendLength,
                pioRecvPci,
                pbRecvBuffer,
                ref pcbRecvLength);
        }

        public SCardError Transmit(IntPtr hCard, IntPtr pioSendPci, byte[] pbSendBuffer, int pcbSendLength, IntPtr pioRecvPci, byte[] pbRecvBuffer, ref int pcbRecvLength) {
            var recvlen = 0;

            if (pbRecvBuffer != null) {
                if (pcbRecvLength > pbRecvBuffer.Length || pcbRecvLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbRecvLength));
                }
                recvlen = pcbRecvLength;
            } else {
                if (pcbRecvLength != 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbRecvLength));
                }
            }

            var sendbuflen = 0;
            if (pbSendBuffer != null) {
                if (pcbSendLength > pbSendBuffer.Length || pcbSendLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbSendLength));
                }
                sendbuflen = pcbSendLength;
            } else {
                if (pcbSendLength != 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbSendLength));
                }
            }

            var rc = SCardHelper.ToSCardError((SCardTransmit(
                hCard,
                pioSendPci,
                pbSendBuffer,
                sendbuflen,
                pioRecvPci,
                pbRecvBuffer,
                ref recvlen)));

            pcbRecvLength = recvlen;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardControl(
            [In] IntPtr hCard,
            [In] int dwControlCode,
            [In] byte[] lpInBuffer,
            [In] int nInBufferSize,
            [In, Out] byte[] lpOutBuffer,
            [In] int nOutBufferSize,
            [Out] out int lpBytesReturned);

        public SCardError Control(IntPtr hCard, IntPtr dwControlCode, byte[] pbSendBuffer, byte[] pbRecvBuffer, out int lpBytesReturned) {
            var sendbuflen = 0;
            if (pbSendBuffer != null) {
                sendbuflen = pbSendBuffer.Length;
            }

            var recvbuflen = 0;
            if (pbRecvBuffer != null) {
                recvbuflen = pbRecvBuffer.Length;
            }

            int bytesret;

            var rc = SCardHelper.ToSCardError(SCardControl(
                hCard,
                unchecked((int)dwControlCode.ToInt64()), // On a 64-bit platform IntPtr.ToInt32() will throw an OverflowException 
                pbSendBuffer,
                sendbuflen,
                pbRecvBuffer,
                recvbuflen,
                out bytesret));

            lpBytesReturned = bytesret;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardStatus(
            [In] IntPtr hCard,
            [Out] byte[] szReaderName,
            [In, Out] ref int pcchReaderLen,
            [Out] out int pdwState,
            [Out] out int pdwProtocol,
            [Out] byte[] pbAtr,
            [In, Out] ref int pcbAtrLen);

        public SCardError Status(IntPtr hCard, out string[] szReaderName, out IntPtr pdwState, out IntPtr pdwProtocol, out byte[] pbAtr) {
            var readerName = new byte[MAX_READER_NAME * CharSize];
            var readerNameSize = MAX_READER_NAME;

            pbAtr = new byte[MAX_ATR_SIZE];
            var atrlen = pbAtr.Length;

            int state, proto;

            var rc = SCardHelper.ToSCardError(SCardStatus(
                hCard,
                readerName,
                ref readerNameSize,
                out state,
                out proto,
                pbAtr,
                ref atrlen));

            if (rc == SCardError.InsufficientBuffer || (MAX_READER_NAME < readerNameSize) || (pbAtr.Length < atrlen)) {
                // second try
               
                if (MAX_READER_NAME < readerNameSize) {
                    // readername byte array was too short
                    readerName = new byte[readerNameSize * CharSize];
                }

                if (pbAtr.Length < atrlen) {
                    // ATR byte array was too short
                    pbAtr = new byte[atrlen];
                }

                rc = SCardHelper.ToSCardError(SCardStatus(
                    hCard,
                    readerName,
                    ref readerNameSize,
                    out state,
                    out proto,
                    pbAtr,
                    ref atrlen));
            }

            pdwState = (IntPtr)state;
            pdwProtocol = (IntPtr)proto;

            if (rc == SCardError.Success) {
                if (atrlen < pbAtr.Length) {
                    Array.Resize(ref pbAtr, atrlen);
                }

                if (readerNameSize < (readerName.Length / CharSize)) {
                    Array.Resize(ref readerName, readerNameSize * CharSize);
                }

                szReaderName = SCardHelper.ConvertToStringArray(readerName, TextEncoding);
            } else {
                szReaderName = null;
            }
            
            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardGetStatusChange(
            [In] IntPtr hContext,
            [In] int dwTimeout,
            [In, Out,
             MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] SCARD_READERSTATE[] rgReaderStates,
            [In] int cReaders);

        public SCardError GetStatusChange(IntPtr hContext, IntPtr dwTimeout, SCardReaderState[] rgReaderStates) {
            SCARD_READERSTATE[] readerstates = null;
            var cReaders = 0;

            if (rgReaderStates != null) {
                // copy the last known state into the buffer
                cReaders = rgReaderStates.Length;
                readerstates = new SCARD_READERSTATE[cReaders];
                for (var i = 0; i < cReaders; i++) {
                    readerstates[i] = rgReaderStates[i].WindowsReaderState;
                }
            }

            // On a 64-bit platforms .ToInt32() will throw an OverflowException 
            var timeout = unchecked((int) dwTimeout.ToInt64());
            var rc = SCardHelper.ToSCardError(
                SCardGetStatusChange(
                    hContext,
                    timeout,
                    readerstates,
                    cReaders));

            if (rc != SCardError.Success || rgReaderStates == null) {
                return rc;
            }

            for (var i = 0; i < cReaders; i++) {
                // replace with returned values
                rgReaderStates[i].WindowsReaderState = readerstates[i];
            }

            return rc;
        }


        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardCancel(
            [In] IntPtr hContext);

        public SCardError Cancel(IntPtr hContext) {
            return SCardHelper.ToSCardError(SCardCancel(hContext));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardGetAttrib(
            [In] IntPtr hCard,
            [In] int dwAttrId,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pbAttr,
            [In, Out] ref int pcbAttrLen);

        public SCardError GetAttrib(IntPtr hCard, IntPtr dwAttrId, byte[] pbAttr, out int pcbAttrLen) {
            var attrlen = (pbAttr != null) 
                ? pbAttr.Length
                : 0;

            var rc = SCardHelper.ToSCardError(SCardGetAttrib(
                hCard,
                unchecked((int)dwAttrId.ToInt64()), // On a 64-bit platform IntPtr.ToInt32() will throw an OverflowException 
                pbAttr,
                ref attrlen));

            pcbAttrLen = attrlen;
            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardSetAttrib(
            [In] IntPtr hCard,
            [In] int dwAttrId,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pbAttr,
            [In] int cbAttrLen);

        public SCardError SetAttrib(IntPtr hCard, IntPtr dwAttrId, byte[] pbAttr, int attrSize) {
            // On a 64-bit platforms IntPtr.ToInt32() will throw an OverflowException 
            var attrid = unchecked((int) dwAttrId.ToInt64());
            var cbAttrLen = 0;
            
            if (pbAttr != null) {
                if (attrSize > pbAttr.Length || attrSize < 0) {
                    throw new ArgumentOutOfRangeException(nameof(attrSize));
                }
                cbAttrLen = attrSize;
            }

            return SCardHelper.ToSCardError(
                SCardSetAttrib(
                    hCard,
                    attrid,
                    pbAttr,
                    cbAttrLen));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern int SCardFreeMemory(
            [In] IntPtr hContext,
            [In] IntPtr pvMem);

        // Windows specific DLL imports

        [DllImport(KERNEL_DLL, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport(KERNEL_DLL, CharSet = CharSet.Ansi, ExactSpelling = true, EntryPoint = "GetProcAddress")]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        public IntPtr GetSymFromLib(string symName) {
            // Step 1. load dynamic link library
            if (_dllHandle == IntPtr.Zero) {
                _dllHandle = LoadLibrary(WINSCARD_DLL);
                if (_dllHandle.Equals(IntPtr.Zero)) {
                    throw new Exception("PInvoke call LoadLibrary() failed");
                }
            }
            // Step 2. search symbol name in memory
            var symPtr = GetProcAddress(_dllHandle, symName);

            if (symPtr.Equals(IntPtr.Zero)) {
                throw new Exception("PInvoke call GetProcAddress() failed");
            }

            return symPtr;
        }
    }
}
