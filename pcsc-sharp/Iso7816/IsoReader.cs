using System;
using System.Threading;

namespace PCSC.Iso7816
{
    public class IsoReader : IIsoReader
    {
        private readonly ISCardContext _context;
        private readonly ISCardReader _reader;
        private readonly bool _releaseContextOnDispose;
        private readonly bool _disconnectReaderOnDispose;

        private int _retransmitWaitTime;
        private int _maxReceiveSize = 128;

        public ISCardContext CurrentContext {
            get { return _context ?? _reader.CurrentContext; }
        }

        public ISCardReader Reader {
            get { return _reader; }
        }

        public string ReaderName {
            get { return _reader.ReaderName; }
        }

        public virtual SCardProtocol ActiveProtocol {
            get { return _reader.ActiveProtocol; }
        }

        public virtual SCardShareMode CurrentShareMode {
            get { return _reader.CurrentShareMode; }
        }

        public virtual int RetransmitWaitTime {
            get { return _retransmitWaitTime; }
            set { _retransmitWaitTime = value; }
        }

        public virtual int MaxReceiveSize {
            get { return _maxReceiveSize; }
            protected set { _maxReceiveSize = value; }
        }

        ~IsoReader() {
            Dispose(false);
        }

        public IsoReader(ISCardReader reader, bool disconnectReaderOnDispose = false) {
            if (reader == null) {
                throw new ArgumentNullException("reader");
            }
            
            _reader = reader;
            _disconnectReaderOnDispose = disconnectReaderOnDispose;
        }

        public IsoReader(ISCardReader reader, string readerName, SCardShareMode mode, SCardProtocol protocol, bool disconnectReaderOnDispose = true) 
            :this(reader, disconnectReaderOnDispose)
        {
            Connect(readerName, mode, protocol);
        }

        public IsoReader(ISCardContext context, bool releaseContextOnDispose = false) {
            if (context == null) {
                throw new ArgumentNullException("context");
            }

            _context = context;
            _reader = new SCardReader(context);
            _releaseContextOnDispose = releaseContextOnDispose;
            _disconnectReaderOnDispose = true;
        }

        public IsoReader(ISCardContext context, string readerName, SCardShareMode mode, SCardProtocol protocol, bool releaseContextOnDispose = true)
            : this(context, releaseContextOnDispose) 
        {
            Connect(readerName, mode, protocol);
        }

        public virtual CommandApdu ConstructCommandApdu(IsoCase isoCase) {
            return new CommandApdu(isoCase, ActiveProtocol);
        }

        public virtual void Connect(string readerName, SCardShareMode mode, SCardProtocol protocol) {
            if (readerName == null) {
                throw new ArgumentNullException("readerName");
            }

            if (protocol == SCardProtocol.Unset) {
                throw new InvalidProtocolException(SCardError.InvalidValue);
            }

            if (((long) mode) == 0) {
                throw new InvalidShareModeException(SCardError.InvalidValue);
            }

            var sc = _reader.Connect(readerName, mode, protocol);

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

        private ResponseApdu SimpleTransmit(byte[] commandApdu, int commandApduLength, IsoCase isoCase, SCardProtocol protocol, SCardPCI receivePci, ref byte[] receiveBuffer, ref int receiveBufferLength) {
            SCardError sc;
            var cmdSent = false;

            do {
                // send Command APDU to the card
                sc = _reader.Transmit(
                    SCardPCI.GetPci(_reader.ActiveProtocol),
                    commandApdu,
                    commandApduLength,
                    receivePci,
                    receiveBuffer,
                    ref receiveBufferLength);

                // Do we need to resend the command APDU?
                if (sc == SCardError.InsufficientBuffer &&
                    receiveBuffer.Length < receiveBufferLength) {
                    // The response buffer was too small. 
                    receiveBuffer = new byte[receiveBufferLength];

                    // Shall we wait until we re-send we APDU?
                    if (_retransmitWaitTime > 0) {
                        Thread.Sleep(_retransmitWaitTime);
                    }
                } else {
                    cmdSent = true;
                }
            } while (cmdSent == false);

            if (sc == SCardError.Success) {
                return new ResponseApdu(receiveBuffer, receiveBufferLength, isoCase, protocol);
            }

            // An error occurred, throw exception..
            ThrowExceptionOnSCardError(sc);
            return null;
        }

        public virtual Response Transmit(CommandApdu commandApdu) {
            if (commandApdu == null) {
                throw new ArgumentNullException("commandApdu");
            }

            // prepare send buffer (Check Command APDU and convert it to an byte array)
            byte[] sendBuffer;
            try {
                sendBuffer = commandApdu.ToArray();
            } catch (InvalidOperationException exception) {
                throw new InvalidApduException("Invalid APDU.", commandApdu, exception);
            }

            // create Response object
            var response = new Response();

            // prepare receive buffer (Response APDU)
            var receiveBufferLength = commandApdu.ExpectedResponseLength; // expected size that shall be returned
            byte[] receiveBuffer = new byte[receiveBufferLength];

            var receivePci = new SCardPCI();

            var responseApdu = SimpleTransmit(
                sendBuffer,
                sendBuffer.Length,
                commandApdu.Case,       // ISO case used by the Command APDU
                commandApdu.Protocol,   // Protocol used by the Command APDU
                receivePci,
                ref receiveBuffer,
                ref receiveBufferLength);

            /* Check status word SW1SW2:
             * 
             * 1. 0x6cxx -> Set response buffer size Le <- SW2
             * 2. AND/OR 0x61xx -> More data can be read with GET RESPONSE
             */

            
            if (responseApdu.SW1 == (byte) SW1Code.ErrorP3Incorrect) {
                // Case 1: SW1=0x6c, Previous Le/P3 not accepted -> Set le = SW2
                var resendCmdApdu = (CommandApdu) commandApdu.Clone();
                if (responseApdu.SW2 == 0) {
                    resendCmdApdu.Le = 0; // 256
                    receiveBufferLength = 256 + 2; // 2 bytes for status word
                } else {
                    resendCmdApdu.Le = responseApdu.SW2;
                    receiveBufferLength = responseApdu.SW2 + 2; // 2 bytes for status word
                }

                receiveBuffer = new byte[receiveBufferLength];
                receivePci = new SCardPCI();

                try {
                    sendBuffer = resendCmdApdu.ToArray();

                    // Shall we wait until we re-send we APDU/TPDU?
                    if (_retransmitWaitTime > 0) {
                        Thread.Sleep(_retransmitWaitTime);
                    }

                    // send Command APDU again with new Le value
                    responseApdu = SimpleTransmit(
                        sendBuffer,
                        sendBuffer.Length,
                        resendCmdApdu.Case,
                        resendCmdApdu.Protocol,
                        receivePci,
                        ref receiveBuffer,
                        ref receiveBufferLength);
                } catch (InvalidOperationException ex) {
                    throw new InvalidApduException("Got SW1=0x6c. Retransmission failed because of an invalid APDU.",
                        resendCmdApdu, ex);
                }
            }

            if (responseApdu.SW1 == (byte) SW1Code.NormalDataResponse) {
                // Case 2: SW1=0x61, More data available -> GET RESPONSE

                /* The transmission system shall issue a GET RESPONSE command APDU (or TPDU)
                 * to the card by assigning the minimum of SW2 and Le to parameter Le (or P3)). 
                 * Le = min(Le,SW2) 
                 */
                var le = (commandApdu.Le < responseApdu.SW2)
                    ? commandApdu.Le
                    : responseApdu.SW2;

                do {
                    // add the last ResponseAPDU to the Response object
                    response.Add(responseApdu);
                    response.Add(receivePci);

                    var getResponseApdu = ConstructGetResponseApdu(ref le);

                    if (le == 0) {
                        receiveBufferLength = 256 + 2; // 2 bytes for status word
                    } else {
                        receiveBufferLength = le + 2; // 2 bytes for status word
                    }

                    receiveBuffer = new byte[receiveBufferLength];

                    try {
                        sendBuffer = getResponseApdu.ToArray();

                        // Shall we wait until we re-send we APDU/TPDU?
                        if (_retransmitWaitTime > 0) {
                            Thread.Sleep(_retransmitWaitTime);
                        }

                        // send Command APDU again with new Le value
                        responseApdu = SimpleTransmit(
                            sendBuffer,
                            sendBuffer.Length,
                            getResponseApdu.Case,
                            getResponseApdu.Protocol,
                            receivePci,
                            ref receiveBuffer,
                            ref receiveBufferLength);
                    } catch (InvalidOperationException ex) {
                        throw new InvalidApduException(
                            "Got SW1=0x61. Retransmission failed because of an invalid GET RESPONSE APDU.",
                            getResponseApdu, ex);
                    }

                    // In case there is more data available.
                    le = responseApdu.SW2;
                } while (
                    // More data available.
                    responseApdu.SW1 == (byte) SW1Code.NormalDataResponse ||
                        // Warning condition: data may be corrupted. Iso7816-4 7.1.5
                        (responseApdu.SW1 == (byte) SW1Code.WarningNVDataNotChanged && responseApdu.SW2 == 0x81));
            }

            response.Add(responseApdu);
            response.Add(receivePci);

            return response;
        }

        private CommandApdu ConstructGetResponseApdu(ref int le) {
            var commandApdu = ConstructCommandApdu(IsoCase.Case2Short);

            if (le > 255 || le < 0) {
                throw new ArgumentOutOfRangeException("le");
            }

            // Does the card/reader support the requested receiveLength?
            if (le > MaxReceiveSize) {
                le = MaxReceiveSize;
            }

            commandApdu.Le = le;
            commandApdu.CLA = 0x00;
            commandApdu.Instruction = InstructionCode.GetResponse;
            commandApdu.P1 = 0x00;
            commandApdu.P2 = 0x00;

            return commandApdu;
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_disconnectReaderOnDispose && _reader != null && _reader.IsConnected) {
                    _reader.Dispose();
                }

                if (_releaseContextOnDispose && _context != null && _context.IsValid()) {
                    _context.Dispose();
                }
            }
        }
    }
}