using System;
using System.Threading;

namespace PCSC.Iso7816
{
    public class IsoCard : IDisposable
    {
        private readonly ISCardReader _reader;

        private int _retransmitWaitTime;
        private int _maxRecvSize = 128;

        public SCardProtocol ActiveProtocol {
            get { return _reader.ActiveProtocol; }
        }

        public SCardShareMode CurrentShareMode {
            get { return _reader.CurrentShareMode; }
        }

        public CommandApdu ConstructCommandApdu(IsoCase isoCase) {
            return new CommandApdu(isoCase, ActiveProtocol);
        }

        public ISCardReader Reader {
            get { return _reader; }
        }

        public int RetransmitWaitTime {
            get { return _retransmitWaitTime; }
            set { _retransmitWaitTime = value; }
        }

        public virtual int MaxReceiveSize {
            get { return _maxRecvSize; }
            protected set { _maxRecvSize = value; }
        }

        ~IsoCard() {
            Dispose(false);
        }

        public IsoCard(ISCardReader reader) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }
            _reader = reader;
        }

        public IsoCard(ISCardReader reader, string readerName, SCardShareMode mode, SCardProtocol proto) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }

            _reader = reader;
            Connect(readerName, mode, proto);
        }

        public virtual void Connect(string readerName, SCardShareMode mode, SCardProtocol proto) {
            if (readerName == null) {
                throw new ArgumentNullException("readerName");
            }

            if (proto == SCardProtocol.Unset) {
                throw new InvalidProtocolException(SCardError.InvalidValue);
            }

            if (((long) mode) == 0) {
                throw new InvalidShareModeException(SCardError.InvalidValue);
            }

            var sc = _reader.Connect(readerName, mode, proto);

            // Throws an exception if sc != SCardError.Success
            ThrowExceptionOnSCardError(sc);
        }

        public virtual void Disconnect(SCardReaderDisposition disposition) {
            if (_reader != null) {
                _reader.Disconnect(disposition);
            }
        }

        /// <summary>
        ///     Throws an exception if <paramref name="sc" /> is not <see cref="F:PCSC.SCardError.Success" />.
        /// </summary>
        /// <param name="sc">The error code returned from the native PC/SC library.</param>
        protected virtual void ThrowExceptionOnSCardError(SCardError sc) {
            if (sc == SCardError.Success) {
                return;
            }

            // An error occurred during connect attempt.
            switch (sc) {
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(sc);
                case SCardError.InvalidParameter:
                    throw new InvalidProtocolException(sc);
                case SCardError.InvalidValue:
                    throw new InvalidValueException(sc);
                case SCardError.NoService:
                    throw new NoServiceException(sc);
                case SCardError.NoSmartcard:
                    throw new NoSmartcardException(sc);
                case SCardError.NotReady:
                    throw new NotReadyException(sc);
                case SCardError.ReaderUnavailable:
                    throw new ReaderUnavailableException(sc);
                case SCardError.SharingViolation:
                    throw new SharingViolationException(sc);
                case SCardError.UnknownReader:
                    throw new UnknownReaderException(sc);
                case SCardError.UnsupportedCard:
                    throw new UnsupportedFeatureException(sc);
                case SCardError.CommunicationError:
                    throw new CommunicationErrorException(sc);
                case SCardError.InternalError:
                    throw new InternalErrorException(sc);
                case SCardError.UnpoweredCard:
                    throw new UnpoweredCardException(sc);
                case SCardError.UnresponsiveCard:
                    throw new UnresponsiveCardException(sc);
                case SCardError.RemovedCard:
                    throw new RemovedCardException(sc);
                case SCardError.InsufficientBuffer:
                    throw new InsufficientBufferException(sc);
                default:
                    throw new PCSCException(sc); // unexpected error
            }
        }

        private ResponseApdu SimpleTransmit(byte[] cmdApdu, int cmdApduLength, IsoCase isoCase, SCardProtocol proto, SCardPCI recvPci, ref byte[] recvBuf, ref int recvBufLength) {
            SCardError sc;
            var cmdSent = false;

            do {
                // send Command APDU to the card
                sc = _reader.Transmit(
                    SCardPCI.GetPci(_reader.ActiveProtocol),
                    cmdApdu,
                    cmdApduLength,
                    recvPci,
                    recvBuf,
                    ref recvBufLength);

                // Do we need to resend the command APDU?
                if (sc == SCardError.InsufficientBuffer &&
                    recvBuf.Length < recvBufLength) {
                    // The response buffer was too small. 
                    recvBuf = new byte[recvBufLength];

                    // Shall we wait until we re-send we APDU?
                    if (_retransmitWaitTime > 0) {
                        Thread.Sleep(_retransmitWaitTime);
                    }
                } else {
                    cmdSent = true;
                }
            } while (cmdSent == false);

            if (sc == SCardError.Success) {
                var respApdu = new ResponseApdu(recvBuf, recvBufLength, isoCase, proto);
                return respApdu;
            }

            // An error occurred, throw exception..
            ThrowExceptionOnSCardError(sc);
            return null;
        }

        public virtual Response Transmit(CommandApdu cmdApdu) {
            if (cmdApdu == null) {
                throw new ArgumentNullException("cmdApdu");
            }

            // prepare send buffer (Check Command APDU and convert it to an byte array)
            byte[] sendbuf;
            try {
                sendbuf = cmdApdu.ToArray();
            } catch (InvalidOperationException ex) {
                throw new InvalidApduException("Invalid APDU.", cmdApdu, ex);
            }

            // create Response object
            var response = new Response();

            // prepare receive buffer (Response APDU)
            var recvbuflen = cmdApdu.ExpectedResponseLength; // expected size that shall be returned
            byte[] recvbuf = new byte[recvbuflen];

            var recvPci = new SCardPCI();

            var respApdu = SimpleTransmit(
                sendbuf,
                sendbuf.Length,
                cmdApdu.Case, // ISO case used by the Command APDU
                cmdApdu.Protocol, // Protocol used by the Command APDU
                recvPci,
                ref recvbuf,
                ref recvbuflen);

            /* Check status word SW1SW2:
             * 
             * 1. 0x6cxx -> Set response buffer size Le <- SW2
             * 2. AND/OR 0x61xx -> More data can be read with GET RESPONSE
             */

            // Case 1: SW1=0x6c, Previous Le/P3 not accepted -> Set le = SW2
            if (respApdu.SW1 == (byte) SW1Code.ErrorP3Incorrect) {
                var resendCmdApdu = (CommandApdu) cmdApdu.Clone();
                if (respApdu.SW2 == 0) {
                    resendCmdApdu.Le = 0; // 256
                    recvbuflen = 256 + 2; // 2 bytes for status word
                } else {
                    resendCmdApdu.Le = respApdu.SW2;
                    recvbuflen = respApdu.SW2 + 2; // 2 bytes for status word
                }

                recvbuf = new byte[recvbuflen];
                recvPci = new SCardPCI();

                try {
                    sendbuf = resendCmdApdu.ToArray();

                    // Shall we wait until we re-send we APDU/TPDU?
                    if (_retransmitWaitTime > 0) {
                        Thread.Sleep(_retransmitWaitTime);
                    }

                    // send Command APDU again with new Le value
                    respApdu = SimpleTransmit(
                        sendbuf,
                        sendbuf.Length,
                        resendCmdApdu.Case,
                        resendCmdApdu.Protocol,
                        recvPci,
                        ref recvbuf,
                        ref recvbuflen);
                } catch (InvalidOperationException ex) {
                    throw new InvalidApduException("Got SW1=0x6c. Retransmission failed because of an invalid APDU.",
                        resendCmdApdu, ex);
                }
            }

            // Case 2: SW1=0x61, More data available -> GET RESPONSE
            if (respApdu.SW1 == (byte) SW1Code.NormalDataResponse) {
                /* The transmission system shall issue a GET RESPONSE command APDU (or TPDU)
                 * to the card by assigning the minimum of SW2 and Le to parameter Le (or P3)). 
                 * Le = min(Le,SW2) 
                 */
                var le = (cmdApdu.Le < respApdu.SW2)
                    ? cmdApdu.Le
                    : respApdu.SW2;

                do {
                    // add the last ResponseAPDU to the Response object
                    response.AddResponseApdu(respApdu);
                    response.AddRecvPci(recvPci);

                    var getResponseApdu = ConstructGetResponseApdu(ref le);

                    if (le == 0) {
                        recvbuflen = 256 + 2; // 2 bytes for status word
                    } else {
                        recvbuflen = le + 2; // 2 bytes for status word
                    }

                    recvbuf = new byte[recvbuflen];

                    try {
                        sendbuf = getResponseApdu.ToArray();

                        // Shall we wait until we re-send we APDU/TPDU?
                        if (_retransmitWaitTime > 0) {
                            Thread.Sleep(_retransmitWaitTime);
                        }

                        // send Command APDU again with new Le value
                        respApdu = SimpleTransmit(
                            sendbuf,
                            sendbuf.Length,
                            getResponseApdu.Case,
                            getResponseApdu.Protocol,
                            recvPci,
                            ref recvbuf,
                            ref recvbuflen);
                    } catch (InvalidOperationException ex) {
                        throw new InvalidApduException(
                            "Got SW1=0x61. Retransmission failed because of an invalid GET RESPONSE APDU.",
                            getResponseApdu, ex);
                    }

                    // In case there is more data available.
                    le = respApdu.SW2;
                } while (
                    // More data available.
                    respApdu.SW1 == (byte) SW1Code.NormalDataResponse ||
                        // Warning condition: data may be corrupted. Iso7816-4 7.1.5
                        (respApdu.SW1 == (byte) SW1Code.WarningNVDataNotChanged && respApdu.SW2 == 0x81));
            }

            response.AddResponseApdu(respApdu);
            response.AddRecvPci(recvPci);

            return response;
        }

        private CommandApdu ConstructGetResponseApdu(ref int le) {
            var cmd = ConstructCommandApdu(IsoCase.Case2Short);

            if (le > 255 || le < 0) {
                throw new ArgumentOutOfRangeException("le");
            }

            // Does the card support the requested recvLength?
            if (le > MaxReceiveSize) {
                le = MaxReceiveSize;
            }

            cmd.Le = le;
            cmd.CLA = 0x00;
            cmd.Instruction = InstructionCode.GetResponse;
            cmd.P1 = 0x00;
            cmd.P2 = 0x00;

            return cmd;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (_reader != null && _reader.IsConnected) {
                _reader.Disconnect(SCardReaderDisposition.Leave);
            }
        }
    }
}