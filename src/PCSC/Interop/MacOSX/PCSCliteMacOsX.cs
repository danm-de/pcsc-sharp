using System;
using System.Text;
using PCSC.Utils;
using PCSC.Interop.MacOSX.ExtensionMethods;
using PCSC.Interop.Windows.Extensions;

namespace PCSC.Interop.MacOSX {
    /// <summary>
    /// PC/SC API for MacOS X
    /// </summary>
    internal sealed class PCSCliteMacOsX : ISCardApi {
        private const int MAX_READER_NAME = 255;

        private const int CHARSIZE = sizeof(byte);

        private const int STATUS_MASK = (int)(SCardState.Absent
                                              | SCardState.Negotiable
                                              | SCardState.Powered
                                              | SCardState.Present
                                              | SCardState.Specific
                                              | SCardState.Swallowed
                                              | SCardState.Unknown);

        public const int MAX_ATR_SIZE = 33;

        public int MaxAtrSize => MAX_ATR_SIZE;
        public Encoding TextEncoding { get; } = Encoding.UTF8;

        public int CharSize => CHARSIZE;

        public SCardError EstablishContext(SCardScope dwScope, IntPtr pvReserved1, IntPtr pvReserved2,
            out IntPtr phContext) {
            var ctx = 0;
            var rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardEstablishContext(
                (int)dwScope,
                pvReserved1,
                pvReserved2,
                ref ctx));
            phContext = (IntPtr)ctx;
            return rc;
        }

        public SCardError ReleaseContext(IntPtr hContext) =>
            SCardHelper.ToSCardError(MacOsxNativeMethods.SCardReleaseContext(hContext.ToInt32()));

        public SCardError IsValidContext(IntPtr hContext) =>
            SCardHelper.ToSCardError(MacOsxNativeMethods.SCardIsValidContext(hContext.ToInt32()));

        public SCardError ListReaders(IntPtr hContext, string[] groups, out string[] readers) {
            var dwReaders = 0;

            // initialize groups array
            byte[] mszGroups = null;
            if (groups != null)
                mszGroups = SCardHelper.ConvertToByteArray(groups, TextEncoding);

            // determine the needed buffer size

            var ctx = hContext.ToInt32();
            var rc = SCardHelper.ToSCardError(
                MacOsxNativeMethods.SCardListReaders(
                    ctx,
                    mszGroups,
                    null,
                    ref dwReaders));

            if (rc != SCardError.Success) {
                readers = null;
                return rc;
            }

            // initialize array for returning reader names
            var mszReaders = new byte[(int)dwReaders];

            rc = SCardHelper.ToSCardError(
                MacOsxNativeMethods.SCardListReaders(
                    ctx,
                    mszGroups,
                    mszReaders,
                    ref dwReaders));

            readers = (rc == SCardError.Success)
                ? SCardHelper.ConvertToStringArray(mszReaders, TextEncoding)
                : null;

            return rc;
        }

        public SCardError ListReaderGroups(IntPtr hContext, out string[] groups) {
            var dwGroups = 0;

            // determine the needed buffer size
            var ctx = hContext.ToInt32();
            var rc = SCardHelper.ToSCardError(
                MacOsxNativeMethods.SCardListReaderGroups(
                    ctx,
                    null,
                    ref dwGroups));

            if (rc != SCardError.Success) {
                groups = null;
                return rc;
            }

            // initialize array for returning group names
            var mszGroups = new byte[(int)dwGroups];

            rc = SCardHelper.ToSCardError(
                MacOsxNativeMethods.SCardListReaderGroups(
                    ctx,
                    mszGroups,
                    ref dwGroups));

            groups = (rc == SCardError.Success)
                ? SCardHelper.ConvertToStringArray(mszGroups, TextEncoding)
                : null;

            return rc;
        }

        public SCardError Connect(IntPtr hContext, string szReader, SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols, out IntPtr phCard, out SCardProtocol pdwActiveProtocol) {
            var readername = SCardHelper.ConvertToByteArray(szReader, TextEncoding, Platform.Lib.CharSize);
            var result = MacOsxNativeMethods.SCardConnect(
                hContext.ToInt32(),
                readername,
                (int)dwShareMode,
                (int)dwPreferredProtocols,
                out var card,
                out var activeproto);

            phCard = (IntPtr)card;
            pdwActiveProtocol = (SCardProtocol)activeproto;

            return SCardHelper.ToSCardError(result);
        }

        public SCardError Reconnect(IntPtr hCard, SCardShareMode dwShareMode, SCardProtocol dwPreferredProtocols,
            SCardReaderDisposition dwInitialization, out SCardProtocol pdwActiveProtocol) {
            var result = MacOsxNativeMethods.SCardReconnect(
                hCard.ToInt32(),
                (int)dwShareMode,
                (int)dwPreferredProtocols,
                (int)dwInitialization,
                out var activeproto);

            pdwActiveProtocol = (SCardProtocol)activeproto;
            return SCardHelper.ToSCardError(result);
        }

        public SCardError Disconnect(IntPtr hCard, SCardReaderDisposition dwDisposition) {
            return SCardHelper.ToSCardError(MacOsxNativeMethods.SCardDisconnect(hCard.ToInt32(), (int)dwDisposition));
        }

        public SCardError BeginTransaction(IntPtr hCard) {
            return SCardHelper.ToSCardError(MacOsxNativeMethods.SCardBeginTransaction(hCard.ToInt32()));
        }

        public SCardError EndTransaction(IntPtr hCard, SCardReaderDisposition dwDisposition) {
            return SCardHelper.ToSCardError(MacOsxNativeMethods.SCardEndTransaction(hCard.ToInt32(), (int)dwDisposition));
        }

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
            var recvlen = 0;
            if (pbRecvBuffer != null) {
                if (pcbRecvLength > pbRecvBuffer.Length || pcbRecvLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbRecvLength));
                }

                recvlen = pcbRecvLength;
            } else {
                if (pcbRecvLength != 0)
                    throw new ArgumentOutOfRangeException(nameof(pcbRecvLength));
            }

            var sendbuflen = IntPtr.Zero;
            if (pbSendBuffer != null) {
                if (pcbSendLength > pbSendBuffer.Length || pcbSendLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbSendLength));
                }

                sendbuflen = (IntPtr)pcbSendLength;
            } else {
                if (pcbSendLength != 0) {
                    throw new ArgumentOutOfRangeException(nameof(pcbSendLength));
                }
            }

            var rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardTransmit(
                hCard.ToInt32(),
                pioSendPci,
                pbSendBuffer,
                sendbuflen.ToInt32(),
                pioRecvPci,
                pbRecvBuffer,
                ref recvlen));

            pcbRecvLength = recvlen;
            return rc;
        }

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

            var rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardControl(
                hCard.ToInt32(),
                dwControlCode.ToInt32(),
                pbSendBuffer,
                sendBufferLength,
                pbRecvBuffer,
                recvBufferLength,
                out var bytesret));

            lpBytesReturned = (int)bytesret;

            return rc;
        }

        public SCardError Status(IntPtr hCard, out string[] szReaderName, out IntPtr pdwState, out IntPtr pdwProtocol,
            out byte[] pbAtr) {
            var readerName = new byte[MAX_READER_NAME * CharSize];
            var readerNameSize = MAX_READER_NAME;

            pbAtr = new byte[MAX_ATR_SIZE];
            var atrlen = pbAtr.Length;
            var rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardStatus(
                hCard.ToInt32(),
                readerName,
                ref readerNameSize,
                out var state,
                out var proto,
                pbAtr,
                ref atrlen));

            if (rc == SCardError.InsufficientBuffer || (MAX_READER_NAME < ((int)readerNameSize)) ||
                (pbAtr.Length < (int)atrlen)) {
                // second try

                if (MAX_READER_NAME < ((int)readerNameSize)) {
                    // readername byte array was too short
                    readerName = new byte[(int)readerNameSize * CharSize];
                }

                if (pbAtr.Length < (int)atrlen) {
                    // ATR byte array was too short
                    pbAtr = new byte[(int)atrlen];
                }

                rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardStatus(
                    hCard.ToInt32(),
                    readerName,
                    ref readerNameSize,
                    out state,
                    out proto,
                    pbAtr,
                    ref atrlen));
            }

            pdwProtocol = (IntPtr)proto;

            if (rc == SCardError.Success) {
                pdwState = (IntPtr)state.Mask(STATUS_MASK);
                if (atrlen < pbAtr.Length) {
                    Array.Resize(ref pbAtr, atrlen);
                }

                if (readerNameSize < (readerName.Length / CharSize)) {
                    Array.Resize(ref readerName, readerNameSize * CharSize);
                }

                szReaderName = SCardHelper.ConvertToStringArray(readerName, TextEncoding);
            } else {
                pdwState = (IntPtr)SCardState.Unknown;
                szReaderName = null;
            }

            return rc;
        }

        public SCardError GetStatusChange(IntPtr hContext, IntPtr dwTimeout, SCardReaderState[] rgReaderStates) {
            SCARD_READERSTATE[] readerstates = null;
            var cReaders = 0;

            if (rgReaderStates != null) {
                // copy the last known state into the buffer
                cReaders = rgReaderStates.Length;
                readerstates = new SCARD_READERSTATE[cReaders];
                for (var i = 0; i < cReaders; i++) {
                    readerstates[i] = rgReaderStates[i].MacOsXReaderState;
                }
            }

            var timeout = unchecked((int)dwTimeout.ToInt64());
            var rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardGetStatusChange(
                hContext.ToInt32(),
                timeout,
                readerstates,
                cReaders));

            if (rc != SCardError.Success || rgReaderStates == null) {
                return rc;
            }

            for (var i = 0; i < cReaders; i++) {
                // replace with returned values
                rgReaderStates[i].MacOsXReaderState = readerstates[i];
            }

            return rc;
        }

        public SCardError Cancel(IntPtr hContext) =>
            SCardHelper.ToSCardError(MacOsxNativeMethods.SCardCancel(hContext.ToInt32()));

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

            var attrlen = receiveBufferLength;
            var rc = SCardHelper.ToSCardError(MacOsxNativeMethods.SCardGetAttrib(
                hCard.ToInt32(),
                attributeId.ToInt32(),
                receiveBuffer,
                ref attrlen));

            attributeLength = attrlen;
            return rc;
        }

        public SCardError SetAttrib(IntPtr hCard, IntPtr attributeId, byte[] sendBuffer, int sendBufferLength) {
            IntPtr cbAttrLen;

            if (sendBuffer != null) {
                if (sendBufferLength > sendBuffer.Length || sendBufferLength < 0) {
                    throw new ArgumentOutOfRangeException(nameof(sendBufferLength));
                }

                cbAttrLen = (IntPtr)sendBufferLength;
            } else {
                cbAttrLen = IntPtr.Zero;
            }

            return SCardHelper.ToSCardError(
                MacOsxNativeMethods.SCardSetAttrib(
                    hCard.ToInt32(),
                    attributeId.ToInt32(),
                    sendBuffer,
                    cbAttrLen.ToInt32()));
        }

        public IntPtr GetSymFromLib(string symName) {
            return MacOsxNativeMethods.GetSymFromLib(symName);
        }
    }
}
