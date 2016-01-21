using System;
using System.Threading;

namespace PCSC.Iso7816
{
    /// <summary>A ISO/IEC 7816 compliant reader.</summary>
    public class IsoReader : IIsoReader
    {
        private readonly ISCardContext _context;
        private readonly bool _releaseContextOnDispose;
        private readonly bool _disconnectReaderOnDispose;

        /// <summary>Gets the current context.</summary>
        public ISCardContext CurrentContext => _context ?? Reader.CurrentContext;

        /// <summary>Gets the current reader.</summary>
        public ISCardReader Reader { get; }

        /// <summary>Gets the name of the reader.</summary>
        public string ReaderName => Reader.ReaderName;

        /// <summary>Gets the active protocol.</summary>
        public virtual SCardProtocol ActiveProtocol => Reader.ActiveProtocol;

        /// <summary>Gets the current share mode.</summary>
        public virtual SCardShareMode CurrentShareMode => Reader.CurrentShareMode;

        /// <summary>Gets or sets the wait time in milliseconds that is used if an APDU needs to be retransmitted.</summary>
        /// <value>Default is 0 ms</value>
        public virtual int RetransmitWaitTime { get; set; }

        /// <summary>Gets the maximum number of bytes that can be received.</summary>
        /// <value>Default is 128 bytes.</value>
        public virtual int MaxReceiveSize { get; protected set; } = 128;

        /// <summary>Finalizes an instance of the <see cref="IsoReader" /> class.</summary>
        ~IsoReader() {
            Dispose(false);
        }

        /// <summary>Initializes a new instance of the <see cref="IsoReader" /> class.</summary>
        /// <param name="reader">The supplied reader will be used for communication with the smart card.</param>
        /// <param name="disconnectReaderOnDispose">if set to <c>true</c> the supplied <paramref name="reader" /> will be disconnected on <see cref="Dispose()" />.</param>
        /// <exception cref="System.ArgumentNullException">If reader is <see langword="null" /></exception>
        public IsoReader(ISCardReader reader, bool disconnectReaderOnDispose = false) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }

            Reader = reader;
            _disconnectReaderOnDispose = disconnectReaderOnDispose;
        }

        /// <summary>Initializes a new instance of the <see cref="IsoReader" /> class and immediately connects to the reader.</summary>
        /// <param name="reader">The supplied reader will be used for communication with the smart card.</param>
        /// <param name="readerName">Name of the reader to connect with.</param>
        /// <param name="mode">The share mode.</param>
        /// <param name="protocol">The communication protocol. <seealso cref="ISCardReader.Connect(string,SCardShareMode,SCardProtocol)" /></param>
        /// <param name="disconnectReaderOnDispose">if set to <c>true</c> the supplied <paramref name="reader" /> will be disconnected on <see cref="Dispose()" />.</param>
        public IsoReader(ISCardReader reader, string readerName, SCardShareMode mode, SCardProtocol protocol,
            bool disconnectReaderOnDispose = true)
            : this(reader, disconnectReaderOnDispose) {
            Connect(readerName, mode, protocol);
        }

        /// <summary>Initializes a new instance of the <see cref="IsoReader" /> class that will create its own instance of a <see cref="SCardReader" />.</summary>
        /// <param name="context">A context to the PC/SC Resource Manager.</param>
        /// <param name="releaseContextOnDispose">if set to <c>true</c> the <paramref name="context" /> will be released on <see cref="Dispose()" />.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="context" /> is <see langword="null" /></exception>
        public IsoReader(ISCardContext context, bool releaseContextOnDispose = false) {
            if (context == null) {
                throw new ArgumentNullException(nameof(context));
            }

            _context = context;
            Reader = new SCardReader(context);
            _releaseContextOnDispose = releaseContextOnDispose;
            _disconnectReaderOnDispose = true;
        }

        /// <summary>Initializes a new instance of the <see cref="IsoReader" /> class that will create its own instance of a <see cref="SCardReader" /> and immediately connect.</summary>
        /// <param name="context">A context to the PC/SC Resource Manager.</param>
        /// <param name="readerName">Name of the reader to connect with.</param>
        /// <param name="mode">The share mode.</param>
        /// <param name="protocol">The communication protocol. <seealso cref="ISCardReader.Connect(string,SCardShareMode,SCardProtocol)" /></param>
        /// <param name="releaseContextOnDispose">if set to <c>true</c> the <paramref name="context" /> will be released on <see cref="Dispose()" />.</param>
        public IsoReader(ISCardContext context, string readerName, SCardShareMode mode, SCardProtocol protocol,
            bool releaseContextOnDispose = true)
            : this(context, releaseContextOnDispose) {
            Connect(readerName, mode, protocol);
        }

        /// <summary>Constructs a command APDU using the active protocol of the reader.</summary>
        /// <param name="isoCase">The ISO case that shall be used for this command.</param>
        /// <returns>An empty command APDU.</returns>
        public virtual CommandApdu ConstructCommandApdu(IsoCase isoCase) {
            return new CommandApdu(isoCase, ActiveProtocol);
        }

        /// <summary>Connects the specified reader.</summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="mode">The share mode.</param>
        /// <param name="protocol">The communication protocol. <seealso cref="ISCardReader.Connect(string,SCardShareMode,SCardProtocol)" /></param>
        public virtual void Connect(string readerName, SCardShareMode mode, SCardProtocol protocol) {
            if (readerName == null) {
                throw new ArgumentNullException(nameof(readerName));
            }

            if (protocol == SCardProtocol.Unset) {
                throw new InvalidProtocolException(SCardError.InvalidValue);
            }

            if (((long) mode) == 0) {
                throw new InvalidShareModeException(SCardError.InvalidValue);
            }

            var sc = Reader.Connect(readerName, mode, protocol);

            // Throws an exception if sc != SCardError.Success
	        sc.ThrowIfNotSuccess();
        }

        /// <summary>Disconnects the currently connected reader.</summary>
        /// <param name="disposition">The action that shall be executed after disconnect.</param>
        public virtual void Disconnect(SCardReaderDisposition disposition)
        {
            Reader?.Disconnect(disposition);
        }

        private ResponseApdu SimpleTransmit(byte[] commandApdu, int commandApduLength, IsoCase isoCase,
            SCardProtocol protocol, SCardPCI receivePci, ref byte[] receiveBuffer, ref int receiveBufferLength) {
            SCardError sc;
            var cmdSent = false;

            do {
                // send Command APDU to the card
                sc = Reader.Transmit(
                    SCardPCI.GetPci(Reader.ActiveProtocol),
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
                    if (RetransmitWaitTime > 0) {
                        Thread.Sleep(RetransmitWaitTime);
                    }
                } else {
                    cmdSent = true;
                }
            } while (cmdSent == false);

            if (sc == SCardError.Success) {
                return new ResponseApdu(receiveBuffer, receiveBufferLength, isoCase, protocol);
            }

            // An error occurred, throw exception..
		    sc.Throw();
		    return null;
        }

        /// <summary>Transmits the specified command APDU.</summary>
        /// <param name="commandApdu">The command APDU.</param>
        /// <returns>A response containing one ore more <see cref="ResponseApdu" />.</returns>
        public virtual Response Transmit(CommandApdu commandApdu) {
            if (commandApdu == null) {
                throw new ArgumentNullException(nameof(commandApdu));
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
            var receiveBuffer = new byte[receiveBufferLength];

            var receivePci = new SCardPCI();

            var responseApdu = SimpleTransmit(
                sendBuffer,
                sendBuffer.Length,
                commandApdu.Case, // ISO case used by the Command APDU
                commandApdu.Protocol, // Protocol used by the Command APDU
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
                    if (RetransmitWaitTime > 0) {
                        Thread.Sleep(RetransmitWaitTime);
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
                        if (RetransmitWaitTime > 0) {
                            Thread.Sleep(RetransmitWaitTime);
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
                throw new ArgumentOutOfRangeException(nameof(le));
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

        /// <summary>
        /// Releases unmanaged and managed resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_disconnectReaderOnDispose && Reader != null && Reader.IsConnected) {
                    Reader.Dispose();
                }

                if (_releaseContextOnDispose && _context != null && _context.IsValid()) {
                    _context.Dispose();
                }
            }
        }
    }
}