﻿using System;
using System.Runtime.InteropServices;
using System.Text;
using PCSC.Interop.Unix.ExtensionMethods;
using PCSC.Utils;

namespace PCSC.Interop.Unix
{
    /// <summary>
    /// PC/SC API for most Unix and Unix-like systems (x86/x64/IA64) 
    /// </summary>
    internal sealed class PCSCliteAPI : ISCardApi
    {
        private const int MAX_READER_NAME = 255;
        private const string C_LIB = "libc";
        private const string OS_NAME_OSX = "Darwin";
        private const string PCSC_LIB_UNIX = "libpcsclite.so.1";
        private const string PCSC_LIB_OSX = "PCSC.framework/PCSC";
        private const string DL_LIB = "libdl.so.2";
        private const int CHARSIZE = sizeof(byte);

        private const int STATUS_MASK = (int) (SCardState.Absent
                                               | SCardState.Negotiable
                                               | SCardState.Powered
                                               | SCardState.Present
                                               | SCardState.Specific
                                               | SCardState.Swallowed
                                               | SCardState.Unknown);

        private readonly string _pcscLibDlopenName;

        private IntPtr _libHandle = IntPtr.Zero;

        public const int MAX_ATR_SIZE = 33;

        public int MaxAtrSize => MAX_ATR_SIZE;
        public Encoding TextEncoding { get; } = Encoding.UTF8;

        public int CharSize => CHARSIZE;

        public PCSCliteAPI() {
            _pcscLibDlopenName = DeterminePcscLibName();
        }

        private static string DeterminePcscLibName() {
            return GetUnameSysName() == OS_NAME_OSX
                ? PCSC_LIB_OSX
                : PCSC_LIB_UNIX;
        }

        private static string GetUnameSysName() {
            var utsNameBuffer = new byte[1000];

            if (uname(utsNameBuffer) == 0) {
                int terminator;

                // Find the null terminator of the first string in struct utsname.
                for (terminator = 0;
                    terminator < utsNameBuffer.Length && utsNameBuffer[terminator] != 0;
                    terminator++) ;

                return Encoding.ASCII.GetString(utsNameBuffer, 0, terminator);
            }

            return null;
        }

        [DllImport(C_LIB, CharSet = CharSet.Ansi)]
        private static extern int uname(
            [Out] byte[] buffer);

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardEstablishContext(
            [In] IntPtr dwScope,
            [In] IntPtr pvReserved1,
            [In] IntPtr pvReserved2,
            [In, Out] ref IntPtr phContext);

        public SCardError EstablishContext(SCardScope dwScope, IntPtr pvReserved1, IntPtr pvReserved2,
            out IntPtr phContext) {
            var ctx = IntPtr.Zero;
            var rc = SCardHelper.ToSCardError(SCardEstablishContext(
                (IntPtr) dwScope,
                pvReserved1,
                pvReserved2,
                ref ctx));
            phContext = ctx;
            return rc;
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardReleaseContext(
            [In] IntPtr hContext);

        public SCardError ReleaseContext(IntPtr hContext) {
            return SCardHelper.ToSCardError(SCardReleaseContext(hContext));
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardIsValidContext(
            [In] IntPtr hContext);

        public SCardError IsValidContext(IntPtr hContext) {
            return SCardHelper.ToSCardError(SCardIsValidContext(hContext));
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardListReaders(
            [In] IntPtr hContext,
            [In] byte[] mszGroups,
            [Out] byte[] mszReaders,
            [In, Out] ref IntPtr pcchReaders);

        public SCardError ListReaders(IntPtr hContext, string[] groups, out string[] readers) {
            var dwReaders = IntPtr.Zero;

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

            // initialize array for returning reader names
            var mszReaders = new byte[(int) dwReaders];

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

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardListReaderGroups(
            [In] IntPtr hContext,
            [Out] byte[] mszGroups,
            [In, Out] ref IntPtr pcchGroups);

        public SCardError ListReaderGroups(IntPtr hContext, out string[] groups) {
            var dwGroups = IntPtr.Zero;

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

            // initialize array for returning group names
            var mszGroups = new byte[(int) dwGroups];

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

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardConnect(
            [In] IntPtr hContext,
            [In] byte[] szReader,
            [In] IntPtr dwShareMode,
            [In] IntPtr dwPreferredProtocols,
            [Out] out IntPtr phCard,
            [Out] out IntPtr pdwActiveProtocol);

        public SCardError Connect(IntPtr hContext, string szReader, SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols, out IntPtr phCard, out SCardProtocol pdwActiveProtocol) {
            var readername = SCardHelper.ConvertToByteArray(szReader, TextEncoding, Platform.Lib.CharSize);

            var result = SCardConnect(hContext,
                readername,
                (IntPtr) dwShareMode,
                (IntPtr) dwPreferredProtocols,
                out phCard,
                out var activeproto);

            pdwActiveProtocol = (SCardProtocol) activeproto;

            return SCardHelper.ToSCardError(result);
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardReconnect(
            [In] IntPtr hCard,
            [In] IntPtr dwShareMode,
            [In] IntPtr dwPreferredProtocols,
            [In] IntPtr dwInitialization,
            [Out] out IntPtr pdwActiveProtocol);

        public SCardError Reconnect(IntPtr hCard, SCardShareMode dwShareMode, SCardProtocol dwPreferredProtocols,
            SCardReaderDisposition dwInitialization, out SCardProtocol pdwActiveProtocol) {
            var result = SCardReconnect(
                hCard,
                (IntPtr) dwShareMode,
                (IntPtr) dwPreferredProtocols,
                (IntPtr) dwInitialization,
                out var activeproto);

            pdwActiveProtocol = (SCardProtocol) activeproto;
            return SCardHelper.ToSCardError(result);
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardDisconnect(
            [In] IntPtr hCard,
            [In] IntPtr dwDisposition);

        public SCardError Disconnect(IntPtr hCard, SCardReaderDisposition dwDisposition) {
            return SCardHelper.ToSCardError(SCardDisconnect(hCard, (IntPtr) dwDisposition));
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardBeginTransaction(
            [In] IntPtr hCard);

        public SCardError BeginTransaction(IntPtr hCard) {
            return SCardHelper.ToSCardError(SCardBeginTransaction(hCard));
        }


        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardEndTransaction(
            [In] IntPtr hCard,
            [In] IntPtr dwDisposition);

        public SCardError EndTransaction(IntPtr hCard, SCardReaderDisposition dwDisposition) {
            return SCardHelper.ToSCardError(SCardEndTransaction(hCard, (IntPtr) dwDisposition));
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardTransmit(
            [In] IntPtr hCard,
            [In] IntPtr pioSendPci,
            [In] byte[] pbSendBuffer,
            [In] IntPtr cbSendLength,
            [In, Out] IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            [In, Out] ref IntPtr pcbRecvLength);

        public SCardError Transmit(IntPtr hCard, IntPtr pioSendPci, byte[] pbSendBuffer, IntPtr pioRecvPci,
            byte[] pbRecvBuffer, out int pcbRecvLength) {
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

        public SCardError Transmit(IntPtr hCard, IntPtr pioSendPci, byte[] pbSendBuffer, int pcbSendLength,
            IntPtr pioRecvPci, byte[] pbRecvBuffer, ref int pcbRecvLength) {
            var recvlen = IntPtr.Zero;
            if (pbRecvBuffer != null) {
                if (pcbRecvLength > pbRecvBuffer.Length || pcbRecvLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbRecvLength));
                }

                recvlen = (IntPtr) pcbRecvLength;
            } else {
                if (pcbRecvLength != 0)
                    throw new ArgumentOutOfRangeException(nameof(pcbRecvLength));
            }

            var sendbuflen = IntPtr.Zero;
            if (pbSendBuffer != null) {
                if (pcbSendLength > pbSendBuffer.Length || pcbSendLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbSendLength));
                }

                sendbuflen = (IntPtr) pcbSendLength;
            } else {
                if (pcbSendLength != 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbSendLength));
                }
            }

            var rc = SCardHelper.ToSCardError(SCardTransmit(
                hCard,
                pioSendPci,
                pbSendBuffer,
                sendbuflen,
                pioRecvPci,
                pbRecvBuffer,
                ref recvlen));

            pcbRecvLength = (int) recvlen;
            return rc;
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardControl(
            [In] IntPtr hCard,
            [In] IntPtr dwControlCode,
            [In] byte[] pbSendBuffer,
            [In] IntPtr cbSendLength,
            [Out] byte[] pbRecvBuffer,
            [In] IntPtr pcbRecvLength,
            [Out] out IntPtr lpBytesReturned);

        public SCardError Control(IntPtr hCard, IntPtr dwControlCode, byte[] pbSendBuffer, byte[] pbRecvBuffer,
            out int lpBytesReturned) {
            return Control(
                hCard,
                dwControlCode,
                pbSendBuffer,
                pbSendBuffer?.Length ?? 0,
                pbRecvBuffer,
                pbRecvBuffer?.Length ?? 0,
                out lpBytesReturned);
        }

        public SCardError Control(IntPtr hCard, IntPtr dwControlCode, byte[] pbSendBuffer, int sendBufferLength,
            byte[] pbRecvBuffer, int recvBufferLength,
            out int lpBytesReturned) {
            if (pbSendBuffer == null && sendBufferLength > 0) {
                throw new ArgumentException("send buffer is null", nameof(sendBufferLength));
            }

            if ((pbSendBuffer != null && pbSendBuffer.Length < sendBufferLength) || sendBufferLength < 0) {
                throw new ArgumentOutOfRangeException(nameof(sendBufferLength));
            }

            if (pbRecvBuffer == null && recvBufferLength > 0) {
                throw new ArgumentException("receive buffer is null", nameof(recvBufferLength));
            }

            if ((pbRecvBuffer != null && pbRecvBuffer.Length < recvBufferLength) || recvBufferLength < 0) {
                throw new ArgumentOutOfRangeException(nameof(recvBufferLength));
            }

            var sendbuflen = (IntPtr) sendBufferLength;
            var recvbuflen = (IntPtr) recvBufferLength;

            var rc = SCardHelper.ToSCardError(SCardControl(
                hCard,
                dwControlCode,
                pbSendBuffer,
                sendbuflen,
                pbRecvBuffer,
                recvbuflen,
                out var bytesret));

            lpBytesReturned = (int) bytesret;

            return rc;
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardStatus(
            [In] IntPtr hCard,
            [Out] byte[] szReaderName,
            [In, Out] ref IntPtr pcchReaderLen,
            [Out] out IntPtr pdwState,
            [Out] out IntPtr pdwProtocol,
            [Out] byte[] pbAtr,
            [In, Out] ref IntPtr pcbAtrLen);

        public SCardError Status(IntPtr hCard, out string[] szReaderName, out IntPtr pdwState, out IntPtr pdwProtocol,
            out byte[] pbAtr) {
            var readerName = new byte[MAX_READER_NAME * CharSize];
            var readerNameSize = (IntPtr) MAX_READER_NAME;

            pbAtr = new byte[MAX_ATR_SIZE];
            var atrlen = (IntPtr) pbAtr.Length;
            var rc = SCardHelper.ToSCardError(SCardStatus(
                hCard,
                readerName,
                ref readerNameSize,
                out var state,
                out var proto,
                pbAtr,
                ref atrlen));

            if (rc == SCardError.InsufficientBuffer || (MAX_READER_NAME < ((int) readerNameSize)) ||
                (pbAtr.Length < (int) atrlen)) {
                // second try

                if (MAX_READER_NAME < ((int) readerNameSize)) {
                    // readername byte array was too short
                    readerName = new byte[(int) readerNameSize * CharSize];
                }

                if (pbAtr.Length < (int) atrlen) {
                    // ATR byte array was too short
                    pbAtr = new byte[(int) atrlen];
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

            pdwState = state;
            pdwProtocol = proto;

            if (rc == SCardError.Success) {
                state = state.Mask(STATUS_MASK);
                if ((int) atrlen < pbAtr.Length) {
                    Array.Resize(ref pbAtr, (int) atrlen);
                }

                if (((int) readerNameSize) < (readerName.Length / CharSize)) {
                    Array.Resize(ref readerName, (int) readerNameSize * CharSize);
                }

                szReaderName = SCardHelper.ConvertToStringArray(readerName, TextEncoding);
            } else {
                szReaderName = null;
            }

            return rc;
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardGetStatusChange(
            [In] IntPtr hContext,
            [In] IntPtr dwTimeout,
            [In, Out] SCARD_READERSTATE[] rgReaderStates,
            [In] IntPtr cReaders);

        public SCardError GetStatusChange(IntPtr hContext, IntPtr dwTimeout, SCardReaderState[] rgReaderStates) {
            SCARD_READERSTATE[] readerstates = null;
            var cReaders = 0;

            if (rgReaderStates != null) {
                // copy the last known state into the buffer
                cReaders = rgReaderStates.Length;
                readerstates = new SCARD_READERSTATE[cReaders];
                for (var i = 0; i < cReaders; i++) {
                    readerstates[i] = rgReaderStates[i].UnixReaderState;
                }
            }

            var rc = SCardHelper.ToSCardError(SCardGetStatusChange(
                hContext,
                dwTimeout,
                readerstates,
                (IntPtr) cReaders));

            if (rc != SCardError.Success || rgReaderStates == null) {
                return rc;
            }

            for (var i = 0; i < cReaders; i++)
                // replace with returned values 
                rgReaderStates[i].UnixReaderState = readerstates[i];

            return rc;
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardCancel(
            [In] IntPtr hContext);

        public SCardError Cancel(IntPtr hContext) {
            return SCardHelper.ToSCardError(SCardCancel(hContext));
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardGetAttrib(
            [In] IntPtr hCard,
            [In] IntPtr dwAttrId,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
            [In, Out] ref IntPtr pcbAttrLen);


        public SCardError GetAttrib(IntPtr hCard, IntPtr attributeId, byte[] receiveBuffer, out int attributeLength) {
            var receiveBufferSize = receiveBuffer?.Length ?? 0;
            return GetAttrib(hCard, attributeId, receiveBuffer, receiveBufferSize, out attributeLength);
        }

        public SCardError GetAttrib(IntPtr hCard, IntPtr attributeId, byte[] receiveBuffer, int receiveBufferLength,
            out int attributeLength) {
            if (receiveBuffer == null && receiveBufferLength != 0) {
                throw new ArgumentOutOfRangeException(nameof(receiveBufferLength));
            }

            if (receiveBuffer != null && (receiveBufferLength < 0 || receiveBufferLength > receiveBuffer.Length)) {
                throw new ArgumentOutOfRangeException(nameof(receiveBufferLength));
            }

            var attrlen = (IntPtr) receiveBufferLength;
            var rc = SCardHelper.ToSCardError(SCardGetAttrib(
                hCard,
                attributeId,
                receiveBuffer,
                ref attrlen));

            attributeLength = (int) attrlen;
            return rc;
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardSetAttrib(
            [In] IntPtr hCard,
            [In] IntPtr dwAttrId,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
            [In] IntPtr cbAttrLen);
        
        public SCardError SetAttrib(IntPtr hCard, IntPtr attributeId, byte[] sendBuffer, int sendBufferLength) {
            IntPtr cbAttrLen;

            if (sendBuffer != null) {
                if (sendBufferLength > sendBuffer.Length || sendBufferLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(sendBufferLength));
                }

                cbAttrLen = (IntPtr) sendBufferLength;
            } else {
                cbAttrLen = IntPtr.Zero;
            }

            return SCardHelper.ToSCardError(
                SCardSetAttrib(
                    hCard,
                    attributeId,
                    sendBuffer,
                    cbAttrLen));
        }

        [DllImport(PCSC_LIB_UNIX)]
        private static extern IntPtr SCardFreeMemory(
            [In] IntPtr hContext,
            [In] IntPtr pvMem);

        // Linux/Unix specific DLL imports

        [DllImport(DL_LIB)]
        private static extern IntPtr dlopen(
            [In] string szFilename,
            [In] int flag);

        [DllImport(DL_LIB)]
        private static extern IntPtr dlsym(
            [In] IntPtr handle,
            [In] string szSymbol);

        [DllImport(DL_LIB)]
        private static extern int dlclose(
            [In] IntPtr handle);

        public IntPtr GetSymFromLib(string symName) {
            // Step 1. load dynamic link library
            if (_libHandle == IntPtr.Zero) {
                _libHandle = dlopen(_pcscLibDlopenName, (int) DLOPEN_FLAGS.RTLD_LAZY);
                if (_libHandle.Equals(IntPtr.Zero)) {
                    throw new Exception("PInvoke call dlopen() failed");
                }
            }

            // Step 2. search symbol name in memory
            var symPtr = dlsym(_libHandle, symName);

            if (symPtr.Equals(IntPtr.Zero)) {
                throw new Exception("PInvoke call dlsym() failed");
            }

            return symPtr;
        }
    }
}
