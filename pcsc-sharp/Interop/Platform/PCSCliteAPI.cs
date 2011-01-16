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
// API for most Unix and Unix-like systems (Linux)

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop
{
    internal class PCSCliteAPI : ISCardAPI
    {
        internal const int MAX_READER_NAME = 255;
        internal const int MAX_ATR_SIZE = 33;
        private const string PCSC_LIB = "libpcsclite.so.1";
        private const string DL_LIB = "libdl.so.2";
        private const int charsize = sizeof(byte);

        private IntPtr libHandle = IntPtr.Zero;
        private Encoding textEncoding;

        [FlagsAttribute]
        private enum DLOPEN_FLAGS : int
        {
            /* The MODE argument to `dlopen' contains one of the following: */
            [DescriptionAttribute("Lazy function call binding")]
            RTLD_LAZY = 0x00001,
            [DescriptionAttribute("Immediate function call binding")]
            RTLD_NOW = 0x00002,

            [DescriptionAttribute("Mask of binding time value")]
            RTLD_BINDING_MASK = 0x3,
            [DescriptionAttribute("Do not load the object.")]
            RTLD_NOLOAD = 0x00004,
            [DescriptionAttribute("Use deep binding")]
            RTLD_DEEPBIND = 0x00008,
            [DescriptionAttribute("The symbols defined by this library will be made available for symbol resolution of subsequently loaded libraries")]
            RTLD_GLOBAL = 0x00100,
            [DescriptionAttribute("The converse of RTLD_GLOBAL")]
            RTLD_LOCAL = 0x0,
            [DescriptionAttribute("Do not delete object when closed")]
            RTLD_NODELETE = 0x01000
        }

        // C struct for P-Invoke
        [StructLayout(LayoutKind.Sequential)]
        internal struct SCARD_READERSTATE
        {
            internal IntPtr pszReader;
            internal IntPtr pvUserData;
            internal IntPtr dwCurrentState;
            internal IntPtr dwEventState;
            internal IntPtr cbAtr;
            [MarshalAs(UnmanagedType.ByValArray,
                SizeConst = MAX_ATR_SIZE)]
            internal byte[] rgbAtr;
        }

        // C struct for P-Invoke
        [StructLayout(LayoutKind.Sequential)]
        internal class SCARD_IO_REQUEST
        {
            internal SCARD_IO_REQUEST()
            {
                dwProtocol = IntPtr.Zero;
            }
            internal IntPtr dwProtocol;   // Protocol identifier
            internal IntPtr cbPciLength;  // Protocol Control Inf Length
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardEstablishContext(
            [In] IntPtr dwScope,
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
            SCardError rc = SCardHelper.ToSCardError(SCardEstablishContext(
                (IntPtr)dwScope,
                (IntPtr)pvReserved1,
                (IntPtr)pvReserved2,
                ref ctx));
            phContext = (IntPtr)ctx;
            return rc;
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardReleaseContext(
            [In] IntPtr hContext);
        public SCardError ReleaseContext(
            IntPtr hContext)
        {
            return SCardHelper.ToSCardError(
                SCardReleaseContext((IntPtr)hContext));
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardIsValidContext(
            [In] IntPtr hContext);
        public SCardError IsValidContext(
            IntPtr hContext)
        {
            return SCardHelper.ToSCardError(
                SCardIsValidContext((IntPtr)hContext));
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardListReaders(
            [In] IntPtr hContext,
            [In]  byte[] mszGroups,
            [Out] byte[] mszReaders,
            [In, Out] ref IntPtr pcchReaders);
        public SCardError ListReaders(
            IntPtr hContext,
            string[] Groups,
            out string[] Readers)
        {
            IntPtr dwReaders = IntPtr.Zero;
            IntPtr ctx = (IntPtr)hContext;
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
            byte[] mszReaders = new byte[(int)dwReaders];

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

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardListReaderGroups(
            [In] IntPtr hContext,
            [Out] byte[] mszGroups,
            [In, Out] ref IntPtr pcchGroups);
        public SCardError ListReaderGroups(
            IntPtr hContext,
            out string[] Groups)
        {
            IntPtr ctx = (IntPtr)hContext;
            IntPtr dwGroups = IntPtr.Zero;
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
            byte[] mszGroups = new byte[(int)dwGroups];

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

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardConnect(
            [In] IntPtr hContext,
            [In] byte[] szReader,
            [In] IntPtr dwShareMode,
            [In] IntPtr dwPreferredProtocols,
            [Out] out IntPtr phCard,
            [Out] out IntPtr pdwActiveProtocol);
        public SCardError Connect(
            IntPtr hContext,
            string szReader,
            SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols,
            out IntPtr phCard,
            out SCardProtocol pdwActiveProtocol)
        {
            byte[] readername = SCardHelper._ConvertToByteArray(szReader, textEncoding, SCardAPI.Lib.CharSize);
            IntPtr ctx = (IntPtr)hContext;
            IntPtr sharemode = (IntPtr)dwShareMode;
            IntPtr prefproto = (IntPtr)dwPreferredProtocols;
            IntPtr card;
            IntPtr activeproto;

            IntPtr result = SCardConnect(ctx,
                readername,
                sharemode,
                prefproto,
                out card,
                out activeproto);

            phCard = card;
            pdwActiveProtocol = (SCardProtocol)activeproto;

            return SCardHelper.ToSCardError(result);
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardReconnect(
            [In] IntPtr hCard,
            [In] IntPtr dwShareMode,
            [In] IntPtr dwPreferredProtocols,
            [In] IntPtr dwInitialization,
            [Out] out IntPtr pdwActiveProtocol);
        public SCardError Reconnect(
            IntPtr hCard,
            SCardShareMode dwShareMode,
            SCardProtocol dwPreferredProtocols,
            SCardReaderDisposition dwInitialization,
            out SCardProtocol pdwActiveProtocol)
        {
            IntPtr activeproto;
            IntPtr result = SCardReconnect(
                (IntPtr)hCard,
                (IntPtr)dwShareMode,
                (IntPtr)dwPreferredProtocols,
                (IntPtr)dwInitialization,
                out activeproto);

            pdwActiveProtocol = (SCardProtocol)activeproto;
            return SCardHelper.ToSCardError(result);
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardDisconnect(
            [In] IntPtr hCard,
            [In] IntPtr dwDisposition);
        public SCardError Disconnect(
            IntPtr hCard,
            SCardReaderDisposition dwDisposition)
        {
            return SCardHelper.ToSCardError(
                SCardDisconnect(
                    (IntPtr)hCard,
                    (IntPtr)dwDisposition));
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardBeginTransaction(
            [In] IntPtr hCard);
        public SCardError BeginTransaction(
            IntPtr hCard)
        {
            return SCardHelper.ToSCardError(
                SCardBeginTransaction((IntPtr)hCard));
        }


        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardEndTransaction(
            [In] IntPtr hCard,
            [In] IntPtr dwDisposition);
        public SCardError EndTransaction(
            IntPtr hCard,
            SCardReaderDisposition dwDisposition)
        {
            return SCardHelper.ToSCardError(
                SCardEndTransaction(
                    (IntPtr)hCard,
                    (IntPtr)dwDisposition));
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardTransmit(
            [In] IntPtr hCard,
            [In] IntPtr pioSendPci,
            [In] byte[] pbSendBuffer,
            [In] IntPtr cbSendLength,
            [In, Out] IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            [In, Out] ref IntPtr pcbRecvLength);

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
            IntPtr recvlen = IntPtr.Zero;
            if (pbRecvBuffer != null)
            {
                if (pcbRecvLength <= pbRecvBuffer.Length &&
                    pcbRecvLength >= 0)
                {
                    recvlen = (IntPtr)pcbRecvLength;
                }
                else
                    throw new ArgumentOutOfRangeException("pcbRecvLength");
            }
            else
            {
                if (pcbRecvLength != 0)
                    throw new ArgumentOutOfRangeException("pcbRecvLength");
            }

            IntPtr sendbuflen = IntPtr.Zero;
            if (pbSendBuffer != null)
            {
                if (pcbSendLength <= pbSendBuffer.Length &&
                    pcbSendLength >= 0)
                {
                    sendbuflen = (IntPtr)pcbSendLength;
                }
                else
                    throw new ArgumentOutOfRangeException("pcbSendLength");
            }
            else
            {
                if (pcbSendLength != 0)
                    throw new ArgumentOutOfRangeException("pcbSendLength");
            }

            IntPtr card = (IntPtr)hCard;
            IntPtr iosendpci = (IntPtr)pioSendPci;
            IntPtr iorecvpci = (IntPtr)pioRecvPci;

            IntPtr retval = SCardTransmit(
                card,
                iosendpci,
                pbSendBuffer,
                sendbuflen,
                iorecvpci,
                pbRecvBuffer,
                ref recvlen);

            SCardError rc = SCardHelper.ToSCardError(retval);

            pcbRecvLength = (int)recvlen;
            return rc;
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardControl(
            [In] IntPtr hCard,
            [In] IntPtr dwControlCode,
            [In] byte[] pbSendBuffer,
            [In] IntPtr cbSendLength,
            [Out] byte[] pbRecvBuffer,
            [In] IntPtr pcbRecvLength,
            [Out] out IntPtr lpBytesReturned);
        public SCardError Control(
            IntPtr hCard,
            IntPtr dwControlCode,
            byte[] pbSendBuffer,
            byte[] pbRecvBuffer,
            out int lpBytesReturned)
        {
            IntPtr sendbuflen = IntPtr.Zero;
            if (pbSendBuffer != null)
                sendbuflen = (IntPtr)pbSendBuffer.Length;

            IntPtr recvbuflen = IntPtr.Zero;
            if (pbRecvBuffer != null)
                recvbuflen = (IntPtr)pbRecvBuffer.Length;

            IntPtr bytesret;

            SCardError rc = SCardHelper.ToSCardError(SCardControl(
                (IntPtr)hCard,
                (IntPtr)dwControlCode,
                pbSendBuffer,
                sendbuflen,
                pbRecvBuffer,
                recvbuflen,
                out bytesret));

            lpBytesReturned = (int)bytesret;

            return rc;
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardStatus(
            [In] IntPtr hCard,
            [Out] byte[] szReaderName,
            [In, Out] ref IntPtr pcchReaderLen,
            [Out] out IntPtr pdwState,
            [Out] out IntPtr pdwProtocol,
            [Out] byte[] pbAtr,
            [In, Out] ref IntPtr pcbAtrLen);
        public SCardError Status(
            IntPtr hCard,
            out string[] szReaderName,
            out IntPtr pdwState,
            out IntPtr pdwProtocol,
            out byte[] pbAtr)
        {
            IntPtr card = (IntPtr)hCard;
            byte[] readername = new byte[MAX_READER_NAME * CharSize];
            int sreadername = MAX_READER_NAME;
            IntPtr tmp_sreadername = (IntPtr)sreadername;

            pbAtr = new byte[MAX_ATR_SIZE];
            IntPtr atrlen = IntPtr.Zero;
            atrlen = (IntPtr)pbAtr.Length;

            IntPtr state, proto;

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

            pdwState = state;
            pdwProtocol = proto;

            return rc;
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardGetStatusChange(
            [In] IntPtr hContext,
            [In] IntPtr dwTimeout,
            [In, Out] SCARD_READERSTATE[] rgReaderStates,
            [In] IntPtr cReaders);
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
                    readerstates[i] = rgReaderStates[i].pcsclite_rstate;
            }
            IntPtr retval = SCardGetStatusChange(
                    (IntPtr)hContext,
                    (IntPtr)dwTimeout,
                    readerstates,
                    (IntPtr)cReaders);
            SCardError rc = SCardHelper.ToSCardError(retval);
            if (rc == SCardError.Success)
                if (rgReaderStates != null)
                {
                    for (int i = 0; i < cReaders; i++)
                        /* replace with returned values */
                        rgReaderStates[i].pcsclite_rstate = readerstates[i];
                }

            return rc;
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardCancel(
            [In] IntPtr hContext);
        public SCardError Cancel(
            IntPtr hContext)
        {
            return SCardHelper.ToSCardError(
                SCardCancel((IntPtr)hContext));
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardGetAttrib(
            [In] IntPtr hCard,
            [In] IntPtr dwAttrId,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pbAttr,
            [In, Out] ref IntPtr pcbAttrLen);
        public SCardError GetAttrib(
            IntPtr hCard,
            IntPtr dwAttrId,
            byte[] pbAttr,
            out int pcbAttrLen)
        {
            IntPtr attrlen = IntPtr.Zero;
            if (pbAttr != null)
                attrlen = (IntPtr)pbAttr.Length;

            SCardError rc = SCardHelper.ToSCardError(SCardGetAttrib(
                (IntPtr)hCard,
                (IntPtr)dwAttrId,
                pbAttr,
                ref attrlen));

            pcbAttrLen = (int)attrlen;
            return rc;
        }

        [DllImport(PCSC_LIB)]
        private extern static IntPtr SCardSetAttrib(
            [In] IntPtr hCard,
            [In] IntPtr dwAttrId,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)] byte[] pbAttr,
            [In] IntPtr cbAttrLen);
        public SCardError SetAttrib(
            IntPtr hCard,
            IntPtr dwAttrId,
            byte[] pbAttr,
            int AttrSize)
        {
            IntPtr attrid = (IntPtr)dwAttrId;
            IntPtr cbAttrLen = IntPtr.Zero;
            if (pbAttr != null)
            {
                if (AttrSize <= pbAttr.Length && AttrSize >= 0)
                    cbAttrLen = (IntPtr)AttrSize;
                else
                    throw new ArgumentOutOfRangeException("AttrSize");
            }
            else
                cbAttrLen = IntPtr.Zero;

            return SCardHelper.ToSCardError(
                SCardSetAttrib(
                    (IntPtr)hCard,
                    attrid,
                    pbAttr,
                    cbAttrLen));
        }

        [DllImport(PCSC_LIB)]
        private static extern IntPtr SCardFreeMemory(
            [In] IntPtr hContext,
            [In] IntPtr pvMem);

        // Linux/Unix specific
        [DllImport(DL_LIB)]
        private extern static IntPtr dlopen(
            [In] string szFilename,
            [In] int flag);
        [DllImport(DL_LIB)]
        private extern static IntPtr dlsym(
            [In] IntPtr handle,
            [In] string szSymbol);
        [DllImport(DL_LIB)]
        private extern static int dlclose(
            [In] IntPtr handle);

        public IntPtr GetSymFromLib(string symName)
        {
            IntPtr symPtr = IntPtr.Zero;

            // Step 1. load dynamic link library
            if (libHandle == IntPtr.Zero)
            {
                libHandle = dlopen(PCSC_LIB, (int)DLOPEN_FLAGS.RTLD_LAZY);
                if (libHandle.Equals(IntPtr.Zero))
                    throw new Exception("PInvoke call dlopen() failed");
            }

            // Step 2. search symbol name in memory
            symPtr = dlsym(libHandle, symName);

            if (symPtr.Equals(IntPtr.Zero))
                throw new Exception("PInvoke call dlsym() failed");

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
