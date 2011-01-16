/*
Copyright (c) 2010 
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
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop
{
    internal interface ISCardAPI
    {
        IntPtr GetSymFromLib(string symName);

        int MaxATRSize { get; }
        Encoding TextEncoding { get; set; }
        int CharSize { get; }

        SCardError EstablishContext(
            SCardScope dwScope,
            IntPtr pvReserved1,
            IntPtr pvReserved2,
            ref IntPtr phContext);

        SCardError ReleaseContext(
            IntPtr hContext);

        SCardError IsValidContext(
            IntPtr hContext);

        SCardError ListReaders(
            IntPtr hContext,
            string[] mszGroups,
            out string[] mszReaders);

        SCardError ListReaderGroups(
           IntPtr hContext,
           out string[] Groups);

        SCardError Connect(
            IntPtr hContext,
            string szReader,
            SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols,
            out IntPtr phCard,
            out SCardProtocol pdwActiveProtocol);

        SCardError Disconnect(
            IntPtr hCard,
            SCardReaderDisposition dwDisposition);

        SCardError Reconnect(
            IntPtr hCard,
            SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols,
            SCardReaderDisposition dwInitialization,
            out SCardProtocol pdwActiveProtocol);

        SCardError BeginTransaction(
           IntPtr hCard);

        SCardError EndTransaction(
            IntPtr hCard,
            SCardReaderDisposition dwDisposition);

        SCardError Transmit(
            IntPtr hCard,
            IntPtr pioSendPci,
            byte[] pbSendBuffer,
            IntPtr pioRecvPci,
            byte[] pbRecvBuffer,
            out int pcbRecvLength);
        SCardError Transmit(
            IntPtr hCard,
            IntPtr pioSendPci,
            byte[] pbSendBuffer,
            int pcbSendLength,
            IntPtr pioRecvPci,
            byte[] pbRecvBuffer,
            ref int pcbRecvLength);

        SCardError Control(
            IntPtr hCard,
            IntPtr dwControlCode,
            byte[] pbSendBuffer,
            byte[] pbRecvBuffer,
            out int lpBytesReturned);

        SCardError Status(
            IntPtr hCard,
            out string[] szReaderName,
            out IntPtr pdwState,
            out IntPtr pdwProtocol,
            out byte[] pbAtr);

        SCardError GetStatusChange(
            IntPtr hContext,
            IntPtr dwTimeout,
            SCardReaderState[] rgReaderStates);

        SCardError Cancel(
            IntPtr hContext);

        SCardError GetAttrib(
            IntPtr hCard,
            IntPtr dwAttrId,
            byte[] pbAttr,
            out int pcbAttrLen);

        SCardError SetAttrib(
            IntPtr hCard,
            IntPtr dwAttrId,
            byte[] pbAttr,
            int AttrSize);
    }
}