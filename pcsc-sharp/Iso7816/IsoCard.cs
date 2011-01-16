/*
Copyright (C) 2010
    Daniel Mueller <daniel@danm.de>

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

Changes to this license can be made only by the copyright author with
explicit written consent.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using PCSC;

namespace PCSC.Iso7816
{
    public class IsoCard
    {
        private int _retransmitWaitTime = 0;
        private int _maxRecvSize = 128;

        ISCardReader reader = null;
        public IsoCard(ISCardReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");
            this.reader = reader;
        }
        public IsoCard(ISCardReader reader, string readerName, SCardShareMode mode, SCardProtocol proto)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            this.reader = reader;
            Connect(readerName, mode, proto);
        }
        public virtual void Connect(string readerName, SCardShareMode mode, SCardProtocol proto)
        {
            if (readerName == null)             // Invalid reader name?
                throw new ArgumentNullException("readerName");  
            if (proto == SCardProtocol.Unset)   // Invalid protocol?
                throw new InvalidProtocolException(SCardError.InvalidValue);
            if (((long)mode) == 0)              // Invalid sharing mode?
                throw new InvalidShareModeException(SCardError.InvalidValue);
            
            SCardError sc = reader.Connect(readerName, mode, proto);

            // Throws an exception if sc != SCardError.Success
            ThrowExceptionOnSCardError(sc);
        }

        /// <summary>
        /// Throws an exception if <paramref name="sc"/> is not <see cref="F:PCSC.SCardError.Success"/>.
        /// </summary>
        /// <param name="sc">The error code returned from the native PC/SC library.</param>
        protected void ThrowExceptionOnSCardError(SCardError sc)
        {
            if (sc == SCardError.Success)
                return; // No Error

            // An error occurred during connect attempt.
            if (sc == SCardError.InvalidHandle)
                throw new InvalidContextException(sc);
            if (sc == SCardError.InvalidParameter)
                throw new InvalidProtocolException(sc);
            if (sc == SCardError.InvalidParameter)
                throw new InvalidParameterException(sc);
            if (sc == SCardError.InvalidValue)
                throw new InvalidValueException(sc);
            if (sc == SCardError.NoService)
                throw new NoServiceException(sc);
            if (sc == SCardError.NoSmartcard)
                throw new NoSmartcardException(sc);
            if (sc == SCardError.NotReady)
                throw new NotReadyException(sc);
            if (sc == SCardError.ReaderUnavailable)
                throw new ReaderUnavailableException(sc);
            if (sc == SCardError.SharingViolation)
                throw new SharingViolationException(sc);
            if (sc == SCardError.UnknownReader)
                throw new UnknownReaderException(sc);
            if (sc == SCardError.UnsupportedCard)
                throw new UnsupportedFeatureException(sc);
            if (sc == SCardError.CommunicationError)
                throw new CommunicationErrorException(sc);
            if (sc == SCardError.InternalError)
                throw new InternalErrorException(sc);
            if (sc == SCardError.UnpoweredCard)
                throw new UnpoweredCardException(sc);
            if (sc == SCardError.UnresponsiveCard)
                throw new UnresponsiveCardException(sc);
            if (sc == SCardError.RemovedCard)
                throw new RemovedCardException(sc);
            if (sc == SCardError.InsufficientBuffer)
                throw new InsufficientBufferException(sc);

            // unexpected error
            throw new PCSCException(sc);
        }

        private ResponseApdu _SimpleTransmit(byte[] cmdApdu, int cmdApduLength, IsoCase isoCase, SCardProtocol proto, SCardPCI recvPci, ref byte[] recvBuf, ref int recvBufLength)
        {
            SCardError sc = SCardError.UnknownError;
            bool cmdSent = false;

            do
            {
                // send Command APDU to the card
                sc = reader.Transmit(
                    SCardPCI.GetPci(reader.ActiveProtocol),
                    cmdApdu,
                    cmdApduLength,
                    recvPci,
                    recvBuf,
                    ref recvBufLength);

                // Do we need to resend the command APDU?
                if (sc == SCardError.InsufficientBuffer &&
                    recvBuf.Length < recvBufLength)
                {
                    // The response buffer was too small. 
                    recvBuf = new byte[recvBufLength];

                    // Shall we wait until we re-send we APDU?
                    if (_retransmitWaitTime > 0)
                        Thread.Sleep(_retransmitWaitTime);
                }
                else
                    cmdSent = true;
            } while (cmdSent == false);

            if (sc == SCardError.Success)
            {
                ResponseApdu respApdu = new ResponseApdu(recvBuf, recvBufLength, isoCase, proto);
                return respApdu;
            }
            
            // An error occurred, throw exception..
            ThrowExceptionOnSCardError(sc);
            return null;
        }

        public virtual Response Transmit(CommandApdu cmdApdu)
        {
            if (cmdApdu == null)
                throw new ArgumentNullException("cmdApdu");

            // prepare send buffer (Check Command APDU and convert it to an byte array)
            byte[] sendbuf;
            try
            {
                sendbuf = cmdApdu.ToArray();
            }
            catch (InvalidOperationException ex)
            {
                throw new InvalidApduException("Invalid APDU.", cmdApdu, ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            // create Response object
            Response resp = new Response();

            // prepare receive buffer (Response APDU)
            byte[] recvbuf = null;
            int recvbuflen = cmdApdu.ExpectedResponseLength; // expected size that shall be returned
            recvbuf = new byte[recvbuflen];

            ResponseApdu respApdu = null;
            SCardPCI recvPci = new SCardPCI();

            respApdu = _SimpleTransmit(
                sendbuf,
                sendbuf.Length,
                cmdApdu.Case,       // ISO case used by the Command APDU
                cmdApdu.Protocol,   // Protocol used by the Command APDU
                recvPci,
                ref recvbuf,
                ref recvbuflen);

            /* Check status word SW1SW2:
             * 
             * 1. 0x6cxx -> Set response buffer size Le <- SW2
             * 2. AND/OR 0x61xx -> More data can be read with GET RESPONSE
             */

            // Case 1: SW1=0x6c, Previous Le/P3 not accepted -> Set le = SW2
            if (respApdu.SW1 == (byte)SW1Code.ErrorP3Incorrect)
            {
                CommandApdu resendCmdApdu = (CommandApdu)cmdApdu.Clone();
                if (respApdu.SW2 == 0)
                {
                    resendCmdApdu.Le = 0; // 256
                    recvbuflen = 256 + 2;           // 2 bytes for status word
                }
                else
                {
                    resendCmdApdu.Le = respApdu.SW2;
                    recvbuflen = respApdu.SW2 + 2;  // 2 bytes for status word
                }

                recvbuf = new byte[recvbuflen];
                recvPci = new SCardPCI();

                try
                {
                    sendbuf = resendCmdApdu.ToArray();

                    // Shall we wait until we re-send we APDU/TPDU?
                    if (_retransmitWaitTime > 0)
                        Thread.Sleep(_retransmitWaitTime);

                    // send Command APDU again with new Le value
                    respApdu = _SimpleTransmit(
                        sendbuf,
                        sendbuf.Length,
                        resendCmdApdu.Case,
                        resendCmdApdu.Protocol,
                        recvPci,
                        ref recvbuf,
                        ref recvbuflen);
                }
                catch (InvalidOperationException ex)
                {
                    throw new InvalidApduException("Got SW1=0x6c. Retransmission failed because of an invalid APDU.", resendCmdApdu, ex);
                }
            }

            // Case 2: SW1=0x61, More data available -> GET RESPONSE
            if (respApdu.SW1 == (byte)SW1Code.NormalDataResponse)
            {
                /* The transmission system shall issue a GET RESPONSE command APDU (or TPDU)
                 * to the card by assigning the minimum of SW2 and Le to parameter Le (or P3)). 
                 * Le = min(Le,SW2) 
                 */
                int _le = (cmdApdu.Le < respApdu.SW2) ? cmdApdu.Le : respApdu.SW2;

                do
                {
                    // add the last ResponseAPDU to the Response object
                    resp.AddResponseApdu(respApdu);
                    resp.AddRecvPci(recvPci);

                    CommandApdu getResponseApdu = _constructGetResponseApdu(ref _le);

                    if (_le == 0)
                        recvbuflen = 256 + 2; // 2 bytes for status word
                    else
                        recvbuflen = _le + 2; // 2 bytes for status word

                    recvbuf = new byte[recvbuflen];

                    try
                    {
                        sendbuf = getResponseApdu.ToArray();

                        // Shall we wait until we re-send we APDU/TPDU?
                        if (_retransmitWaitTime > 0)
                            Thread.Sleep(_retransmitWaitTime);

                        // send Command APDU again with new Le value
                        respApdu = _SimpleTransmit(
                            sendbuf,
                            sendbuf.Length,
                            getResponseApdu.Case,
                            getResponseApdu.Protocol,
                            recvPci,
                            ref recvbuf,
                            ref recvbuflen);
                    }
                    catch (InvalidOperationException ex)
                    {
                        throw new InvalidApduException("Got SW1=0x61. Retransmission failed because of an invalid GET RESPONSE APDU.", getResponseApdu, ex);
                    }

                    // In case there is more data available.
                    _le = respApdu.SW2;
                } while (
                    // More data available.
                    respApdu.SW1 == (byte)SW1Code.NormalDataResponse 
                    
                    ||

                    // Warning condition: data may be corrupted. Iso7816-4 7.1.5
                    (respApdu.SW1 == (byte)SW1Code.WarningNVDataNotChanged && 
                     respApdu.SW2 == (byte)0x81)
                    );
            }

            resp.AddResponseApdu(respApdu);
            resp.AddRecvPci(recvPci);
                        
            return resp;
        }
        public SCardProtocol ActiveProtocol
        {
            get { return reader.ActiveProtocol; }
        }
        public SCardShareMode CurrentShareMode
        {
            get { return reader.CurrentShareMode; }
        }
        public CommandApdu ConstructCommandApdu(IsoCase isoCase)
        {
            return new CommandApdu(isoCase, ActiveProtocol);
        }
        private CommandApdu _constructGetResponseApdu(ref int le)
        {
            CommandApdu cmd;
            cmd = ConstructCommandApdu(IsoCase.Case2Short);

            if (le > 255 || le < 0)
                throw new ArgumentOutOfRangeException("le");

            // Does the card support the requested recvLength?
            if (le > MaxReceiveSize)
                le = MaxReceiveSize;
            
            cmd.Le = le;
            cmd.CLA = 0x00;
            cmd.Instruction = InstructionCode.GetResponse;
            cmd.P1 = 0x00;
            cmd.P2 = 0x00;

            return cmd;   
        }
        public int RetransmitWaitTime
        {
            get { return _retransmitWaitTime; }
            set { _retransmitWaitTime = value; }
        }

        public virtual int MaxReceiveSize
        {
            get { return _maxRecvSize; }
            protected set { _maxRecvSize = value; }
        }
    }
}
