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

// http://en.wikipedia.org/wiki/64-bit
// Microsoft Win64 (X64/IA64)

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop
{
    internal class WinSCardAPI : ISCardAPI
    {
        internal const int MAX_READER_NAME = 255;
        internal const int MAX_ATR_SIZE = 0x24;
        private const string WINSCARD_DLL = "winscard.dll";
        private const string KERNEL_DLL = "KERNEL32.DLL";
        private const int charsize = sizeof(char);

        private IntPtr dllHandle = IntPtr.Zero;
        private Encoding textEncoding;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal struct SCARD_READERSTATE
        {
            internal IntPtr pszReader;
            internal IntPtr pvUserData;
            internal Int32 dwCurrentState;
            internal Int32 dwEventState;
            internal Int32 cbAtr;
            [MarshalAs(UnmanagedType.ByValArray,
                SizeConst = (int)MAX_ATR_SIZE,
                ArraySubType = UnmanagedType.U1)]
            internal byte[] rgbAtr;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class SCARD_IO_REQUEST
        {
            internal Int32 dwProtocol;
            internal Int32 cbPciLength;
            internal SCARD_IO_REQUEST()
            {
                dwProtocol = 0;
            }
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardEstablishContext(
            [In] Int32 dwScope,
            [In] IntPtr pvReserved1,
            [In] IntPtr pvReserved2,
            [In, Out] ref IntPtr phContext);
        public SCardError EstablishContext(
          SCardScope dwScope,
          IntPtr pvReserved1,
          IntPtr pvReserved2,
          ref IntPtr phContext)
        {
            IntPtr ctx = IntPtr.Zero;
            SCardError rc = SCardHelper.ToSCardError(
                SCardEstablishContext(
                    (Int32)dwScope,
                    pvReserved1,
                    pvReserved2,
                    ref ctx));
            phContext = ctx;
            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardReleaseContext(
            [In] IntPtr hContext);
        public SCardError ReleaseContext(
            IntPtr hContext)
        {
            return SCardHelper.ToSCardError(
                SCardReleaseContext((IntPtr)hContext));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardIsValidContext(
            [In] IntPtr hContext);
        public SCardError IsValidContext(
            IntPtr hContext)
        {
            return SCardHelper.ToSCardError(
                SCardIsValidContext((IntPtr)hContext));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardListReaders(
            [In] IntPtr hContext,
            [In] byte[] mszGroups,
            [Out] byte[] pmszReaders,
            [In, Out] ref Int32 pcchReaders);
        public SCardError ListReaders(
            IntPtr hContext,
            string[] Groups,
            out string[] Readers)
        {
            IntPtr ctx = (IntPtr)hContext;
            Int32 dwReaders = 0;
            SCardError rc;

            // initialize groups array
            byte[] mszGroups = null;
            if (Groups != null)
                mszGroups = SCardHelper._ConvertToByteArray(Groups, textEncoding);

            // determine the needed buffer size
            rc = SCardHelper.ToSCardError(
                SCardListReaders(ctx,
                    mszGroups,
                    null,
                    ref dwReaders));

            if (rc != SCardError.Success)
            {
                Readers = null;
                return rc;
            }

            // initialize array
            byte[] mszReaders = new byte[(int)dwReaders * sizeof(char)];

            rc = SCardHelper.ToSCardError(
                SCardListReaders(ctx,
                    mszGroups,
                    mszReaders,
                    ref dwReaders));

            if (rc == SCardError.Success)
                Readers = SCardHelper._ConvertToStringArray(mszReaders, textEncoding);
            else
                Readers = null;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardListReaderGroups(
            [In] IntPtr hContext,
            [Out] byte[] mszGroups,
            [In, Out] ref Int32 pcchGroups);
        public SCardError ListReaderGroups(
            IntPtr hContext,
            out string[] Groups)
        {
            IntPtr ctx = (IntPtr)hContext;
            Int32 dwGroups = 0;
            SCardError rc;

            // determine the needed buffer size
            rc = SCardHelper.ToSCardError(
                SCardListReaderGroups(
                    ctx,
                    null,
                    ref dwGroups));

            if (rc != SCardError.Success)
            {
                Groups = null;
                return rc;
            }

            // initialize array
            byte[] mszGroups = new byte[(int)dwGroups * sizeof(char)];

            rc = SCardHelper.ToSCardError(
                SCardListReaderGroups(
                    ctx,
                    mszGroups,
                    ref dwGroups));

            if (rc == SCardError.Success)
                Groups = SCardHelper._ConvertToStringArray(mszGroups, textEncoding);
            else
                Groups = null;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardConnect(
            [In] IntPtr hContext,
            [In] byte[] szReader,
            [In] Int32 dwShareMode,
            [In] Int32 dwPreferredProtocols,
            [Out] out IntPtr phCard,
            [Out] out Int32 pdwActiveProtocol);
        public SCardError Connect(
            IntPtr hContext,
            string szReader,
            SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols,
            out IntPtr phCard,
            out SCardProtocol pdwActiveProtocol)
        {
            byte[] readername = SCardHelper._ConvertToByteArray(szReader, textEncoding, SCardAPI.Lib.CharSize);
            Int32 sharemode = (Int32)dwShareMode;
            Int32 prefproto = (Int32)dwPreferredProtocols;
            IntPtr ctx = (IntPtr)hContext;
            IntPtr card;
            Int32 activeproto;

            Int32 result = SCardConnect(ctx,
                readername,
                sharemode,
                prefproto,
                out card,
                out activeproto);

            phCard = card;
            pdwActiveProtocol = (SCardProtocol)activeproto;

            return SCardHelper.ToSCardError(result);
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardReconnect(
            [In] IntPtr hCard,
            [In] Int32 dwShareMode,
            [In] Int32 dwPreferredProtocols,
            [In] Int32 dwInitialization,
            [Out] out Int32 pdwActiveProtocol);
        public SCardError Reconnect(
            IntPtr hCard,
            SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols,
            SCardReaderDisposition dwInitialization,
            out SCardProtocol pdwActiveProtocol)
        {
            Int32 activeproto;
            IntPtr card = (IntPtr)hCard;
            Int32 result = SCardReconnect(
                card,
                (Int32)dwShareMode,
                (Int32)dwPreferredProtocols,
                (Int32)dwInitialization,
                out activeproto);

            pdwActiveProtocol = (SCardProtocol)activeproto;
            return SCardHelper.ToSCardError(result);
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardDisconnect(
            [In] IntPtr hCard,
            [In] Int32 dwDisposition);
        public SCardError Disconnect(
            IntPtr hCard,
            SCardReaderDisposition dwDisposition)
        {
            return SCardHelper.ToSCardError(
                SCardDisconnect(
                    (IntPtr)hCard,
                    (Int32)dwDisposition));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardBeginTransaction(
            [In] IntPtr hCard);
        public SCardError BeginTransaction(
            IntPtr hCard)
        {
            return SCardHelper.ToSCardError(
                SCardBeginTransaction((IntPtr)hCard));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardEndTransaction(
            [In] IntPtr hCard,
            [In] Int32 dwDisposition);
        public SCardError EndTransaction(
            IntPtr hCard,
            SCardReaderDisposition dwDisposition)
        {
            return SCardHelper.ToSCardError(
                SCardEndTransaction(
                    (IntPtr)hCard,
                    (Int32)dwDisposition));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardTransmit(
            [In] IntPtr hCard,
            [In] IntPtr pioSendPci,
            [In] byte[] pbSendBuffer,
            [In] Int32 cbSendLength,
            [In, Out] IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            [In, Out] ref Int32 pcbRecvLength);


        public SCardError Transmit(
            IntPtr hCard,
            IntPtr pioSendPci,
            byte[] pbSendBuffer,
            IntPtr pioRecvPci,
            byte[] pbRecvBuffer,
            out int pcbRecvLength)
        {
            pcbRecvLength = 0;
            if (pbRecvBuffer != null)
                pcbRecvLength = pbRecvBuffer.Length;

            int pcbSendLength = 0;
            if (pbSendBuffer != null)
                pcbSendLength = pbSendBuffer.Length;
            return Transmit(
                hCard,
                pioSendPci,
                pbSendBuffer,
                pcbSendLength,
                pioRecvPci,
                pbRecvBuffer,
                ref pcbRecvLength);
        }
        public SCardError Transmit(
            IntPtr hCard,
            IntPtr pioSendPci,
            byte[] pbSendBuffer,
            int pcbSendLength,
            IntPtr pioRecvPci,
            byte[] pbRecvBuffer,
            ref int pcbRecvLength)
        {
            Int32 recvlen = 0;

            if (pbRecvBuffer != null)
            {
                if (pcbRecvLength <= pbRecvBuffer.Length &&
                    pcbRecvLength >= 0)
                {
                    recvlen = (Int32)pcbRecvLength;
                }
                else
                    throw new ArgumentOutOfRangeException("pcbRecvLength");
            }
            else
            {
                if (pcbRecvLength != 0)
                    throw new ArgumentOutOfRangeException("pcbRecvLength");
            }

            Int32 sendbuflen = 0;
            if (pbSendBuffer != null)
            {
                if (pcbSendLength <= pbSendBuffer.Length &&
                    pcbSendLength >= 0)
                {
                    sendbuflen = (Int32)pcbSendLength;
                }
                else
                    throw new ArgumentOutOfRangeException("pcbSendLength");
            }
            else
            {
                if (pcbSendLength != 0)
                    throw new ArgumentOutOfRangeException("pcbSendLength");
            }

            IntPtr iorecvpci = (IntPtr)pioRecvPci;
            SCardError rc = SCardHelper.ToSCardError((SCardTransmit(
                (IntPtr)hCard,
                (IntPtr)pioSendPci,
                pbSendBuffer,
                sendbuflen,
                iorecvpci,
                pbRecvBuffer,
                ref recvlen)));

            pcbRecvLength = (int)recvlen;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardControl(
            [In] IntPtr hCard,
            [In] Int32 dwControlCode,
            [In] byte[] lpInBuffer,
            [In] Int32 nInBufferSize,
            [In, Out] byte[] lpOutBuffer,
            [In] Int32 nOutBufferSize,
            [Out] out Int32 lpBytesReturned);
        public SCardError Control(
            IntPtr hCard,
            IntPtr dwControlCode,
            byte[] pbSendBuffer,
            byte[] pbRecvBuffer,
            out int lpBytesReturned)
        {
            Int32 sendbuflen = 0;
            if (pbSendBuffer != null)
                sendbuflen = (Int32)pbSendBuffer.Length;

            Int32 recvbuflen = 0;
            if (pbRecvBuffer != null)
                recvbuflen = (Int32)pbRecvBuffer.Length;

            Int32 bytesret;

            SCardError rc = SCardHelper.ToSCardError(SCardControl(
                (IntPtr)hCard,
                // On a 64-bit platform IntPtr.ToInt32() will throw an OverflowException 
                unchecked((Int32)dwControlCode.ToInt64()),
                pbSendBuffer,
                sendbuflen,
                pbRecvBuffer,
                recvbuflen,
                out bytesret));

            lpBytesReturned = (int)bytesret;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardStatus(
            [In] IntPtr hCard,
            [Out] byte[] szReaderName,
            [In, Out] ref Int32 pcchReaderLen,
            [Out] out Int32 pdwState,
            [Out] out Int32 pdwProtocol,
            [Out] byte[] pbAtr,
            [In, Out] ref Int32 pcbAtrLen);
        public SCardError Status(
            IntPtr hCard,
            out string[] szReaderName,
            out IntPtr pdwState,
            out IntPtr pdwProtocol,
            out byte[] pbAtr)
        {
            byte[] readername = new byte[MAX_READER_NAME * CharSize];
            int sreadername = MAX_READER_NAME;
            Int32 tmp_sreadername = (Int32)sreadername;

            pbAtr = new byte[MAX_ATR_SIZE];
            Int32 atrlen = 0;
            atrlen = (Int32)pbAtr.Length;

            Int32 state, proto;
            IntPtr card = (IntPtr)hCard;

            SCardError rc = SCardHelper.ToSCardError(SCardStatus(
                card,
                readername,
                ref tmp_sreadername,
                out state,
                out proto,
                pbAtr,
                ref atrlen));

            if (rc == SCardError.InsufficientBuffer ||
                (sreadername < ((int)tmp_sreadername)) ||
                (pbAtr.Length < (int)atrlen))
            {
                // readername string
                if (sreadername < ((int)tmp_sreadername))
                {
                    sreadername = (int)tmp_sreadername;
                    readername = new byte[sreadername * CharSize];
                }

                if (pbAtr.Length < (int)atrlen)
                {
                    pbAtr = new byte[(int)atrlen];
                }

                rc = SCardHelper.ToSCardError(SCardStatus(
                    card,
                    readername,
                    ref tmp_sreadername,
                    out state,
                    out proto,
                    pbAtr,
                    ref atrlen));
            }

            if (rc == SCardError.Success)
            {
                if ((int)atrlen < pbAtr.Length)
                    Array.Resize<byte>(ref pbAtr, (int)atrlen);

                if (((int)tmp_sreadername) < (readername.Length / CharSize))
                    Array.Resize<byte>(ref readername, (int)tmp_sreadername * CharSize);

                szReaderName = SCardHelper._ConvertToStringArray(
                    readername, textEncoding);
            }
            else
                szReaderName = null;

            pdwState = (IntPtr)state;
            pdwProtocol = (IntPtr)proto;

            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardGetStatusChange(
            [In] IntPtr hContext,
            [In] Int32 dwTimeout,
            [In, Out,
                MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]  
                    SCARD_READERSTATE[] rgReaderStates,
            [In] Int32 cReaders);
        public SCardError GetStatusChange(
            IntPtr hContext,
            IntPtr dwTimeout,
            SCardReaderState[] rgReaderStates)
        {

            SCARD_READERSTATE[] readerstates = null;
            int cReaders = 0;

            if (rgReaderStates != null)
            {
                cReaders = rgReaderStates.Length;
                readerstates = new SCARD_READERSTATE[cReaders];
                for (int i = 0; i < cReaders; i++)
                    readerstates[i] = rgReaderStates[i].winscard_rstate;
            }

            SCardError rc;
            // On a 64-bit platforms .ToInt32() will throw an OverflowException 
            Int32 timeout = unchecked((Int32)dwTimeout.ToInt64());
            rc = SCardHelper.ToSCardError(
                SCardGetStatusChange(
                    (IntPtr)hContext,
                    (Int32)timeout,
                    readerstates,
                    (Int32)cReaders));


            if (rc == SCardError.Success)
                if (rgReaderStates != null)
                {
                    for (int i = 0; i < cReaders; i++)
                        /* replace with returned values */
                        rgReaderStates[i].winscard_rstate = readerstates[i];
                }

            return rc;
        }


        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardCancel(
            [In] IntPtr hContext);
        public SCardError Cancel(
            IntPtr hContext)
        {
            return SCardHelper.ToSCardError(
                SCardCancel((IntPtr)hContext));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardGetAttrib(
            [In] IntPtr hCard,
            [In] Int32 dwAttrId,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pbAttr,
            [In, Out] ref Int32 pcbAttrLen);
        public SCardError GetAttrib(
            IntPtr hCard,
            IntPtr dwAttrId,
            byte[] pbAttr,
            out int pcbAttrLen)
        {
            Int32 attrlen = 0;
            if (pbAttr != null)
                attrlen = (Int32)pbAttr.Length;

            SCardError rc = SCardHelper.ToSCardError(SCardGetAttrib(
                (IntPtr)hCard,
                // On a 64-bit platform IntPtr.ToInt32() will throw an OverflowException 
                unchecked((Int32)dwAttrId.ToInt64()),
                pbAttr,
                ref attrlen));

            pcbAttrLen = (int)attrlen;
            return rc;
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardSetAttrib(
            [In] IntPtr hCard,
            [In] Int32 dwAttrId,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pbAttr,
            [In] Int32 cbAttrLen);
        public SCardError SetAttrib(
            IntPtr hCard,
            IntPtr dwAttrId,
            byte[] pbAttr,
            int AttrSize)
        {
            // On a 64-bit platforms IntPtr.ToInt32() will throw an OverflowException 
            Int32 attrid = unchecked((Int32)dwAttrId.ToInt64());
            Int32 cbAttrLen = 0;
            if (pbAttr != null)
            {
                if (AttrSize <= pbAttr.Length && AttrSize >= 0)
                    cbAttrLen = (Int32)AttrSize;
                else
                    throw new ArgumentOutOfRangeException("AttrSize");
            }

            return SCardHelper.ToSCardError(
                SCardSetAttrib(
                    (IntPtr)hCard,
                    attrid,
                    pbAttr,
                    cbAttrLen));
        }

        [DllImport(WINSCARD_DLL, CharSet = CharSet.Auto)]
        private static extern Int32 SCardFreeMemory(
            [In] IntPtr hContext,
            [In] IntPtr pvMem);

        // Windows specific
        [DllImport(KERNEL_DLL,
            CharSet = CharSet.Auto)]
        private extern static IntPtr LoadLibrary(String lpFileName);

        [DllImport(KERNEL_DLL,
            CharSet = CharSet.Ansi,
            ExactSpelling = true,
            EntryPoint = "GetProcAddress")]
        private extern static IntPtr GetProcAddress(IntPtr hModule, String lpProcName);

        public IntPtr GetSymFromLib(string symName)
        {
            IntPtr symPtr = IntPtr.Zero;
            // Step 1. load dynamic link library
            if (dllHandle == IntPtr.Zero)
            {
                dllHandle = LoadLibrary(WINSCARD_DLL);
                if (dllHandle.Equals(IntPtr.Zero))
                    throw new Exception("PInvoke call LoadLibrary() failed");
            }
            // Step 2. search symbol name in memory
            symPtr = GetProcAddress(dllHandle, symName);

            if (symPtr.Equals(IntPtr.Zero))
                throw new Exception("PInvoke call GetProcAddress() failed");

            return symPtr;
        }

        public int MaxATRSize
        {
            get { return MAX_ATR_SIZE; }
        }
        public Encoding TextEncoding
        {
            get { return textEncoding; }
            set { textEncoding = value; }
        }
        public int CharSize
        {
            get { return charsize; }
        }
    }
}
