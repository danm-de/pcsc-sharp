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

namespace PCSC
{
    public interface ISCardReader
    {
        SCardError Connect(string name,
                           SCardShareMode mode,
                           SCardProtocol prefProto);
        SCardError Disconnect(SCardReaderDisposition discntExec);
        SCardError Reconnect(SCardShareMode mode,
                             SCardProtocol prefProto,
                             SCardReaderDisposition initExec);

        SCardError BeginTransaction();
        SCardError EndTransaction(SCardReaderDisposition disposition);

        SCardError Transmit(SCardPCI ioSendPci,
                            byte[] sendBuffer,
                            SCardPCI ioRecvPci,
                            ref byte[] RecvBuffer);
        SCardError Transmit(IntPtr pioSendPci,
                            byte[] sendBuffer,
                            ref byte[] recvBuffer);
        SCardError Transmit(IntPtr pioSendPci,
                            byte[] sendBuffer,
                            SCardPCI ioRecvPci,
                            ref byte[] recvBuffer);
        SCardError Transmit(IntPtr pioSendPci,
                            byte[] sendBuffer,
                            int sendBufLength,
                            SCardPCI ioRecvPci,
                            byte[] recvBuffer,
                            ref int recvBufLength);
        SCardError Transmit(byte[] sendBuffer,
                            int sendBufferLength,
                            byte[] recvBuffer,
                            ref int recvBufferLength);
        SCardError Transmit(byte[] sendBuffer,
                            byte[] recvBuffer,
                            ref int recvBufferLength);
        SCardError Transmit(byte[] sendBuffer,
                            ref byte[] recvBuffer);

        SCardError Control(IntPtr controlCode,
                          byte[] sendBuffer,
                          ref byte[] recvBuffer);


        SCardError Status(out string[] ReaderName,
                          out SCardState State,
                          out SCardProtocol Protocol,
                          out byte[] Atr);

        SCardError GetAttrib(SCardAttr AttrId,
                             out byte[] pbAttr);
        SCardError GetAttrib(IntPtr AttrId,
                             out byte[] pbAttr);
        SCardError GetAttrib(SCardAttr AttrId,
                             byte[] pbAttr,
                             out int AttrLen);
        SCardError GetAttrib(IntPtr dwAttrId,
                             byte[] pbAttr,
                             out int AttrLen);

        SCardError SetAttrib(SCardAttr attr,
                             byte[] pbAttr);
        SCardError SetAttrib(IntPtr attr,
                             byte[] pbAttr);
        SCardError SetAttrib(SCardAttr attr,
                             byte[] pbAttr,
                             int AttrBufSize);
        SCardError SetAttrib(IntPtr attr,
                             byte[] pbAttr,
                             int AttrBufSize);

        string ReaderName { get; }
        SCardContext CurrentContext { get; }
        SCardShareMode CurrentShareMode { get; }
        SCardProtocol ActiveProtocol { get; }
        IntPtr CardHandle { get; }
    }
}
