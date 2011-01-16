/*
Text description/attribute names.
Copyright (C) 1999-2003
    David Corcoran <corcoran@linuxnet.com>
    Ludovic Rousseau <ludovic.rousseau@free.fr>

Program code. 
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
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using PCSC.Interop;

namespace PCSC
{
    public enum SCardShareMode : int
    {
        [DescriptionAttribute("Exclusive mode only")]
        Exclusive = 0x0001,
        [DescriptionAttribute("Shared mode only")]
        Shared = 0x0002,
        [DescriptionAttribute("Raw mode only")]
        Direct = 0x0003
    }

    [FlagsAttribute]
    public enum SCardProtocol : int
    {
        [DescriptionAttribute("Protocol not set")]
        Unset = 0x0000,
        [DescriptionAttribute("T=0 active protocol")]
        T0 = 0x0001,
        [DescriptionAttribute("T=1 active protocol")]
        T1 = 0x0002,
        [DescriptionAttribute("Raw active protocol")]
        Raw = 0x0004,
        [DescriptionAttribute("T=15 protocol")]
        T15 = 0x0008,

        [DescriptionAttribute("IFD determines protocol")]
        Any = (T0 | T1)
    }

    public enum SCardReaderDisposition : int
    {
        [DescriptionAttribute("Do nothing on close")]
        Leave = 0x0000,
        [DescriptionAttribute("Reset on close")]
        Reset = 0x0001,
        [DescriptionAttribute("Power down on close")]
        Unpower = 0x0002,
        [DescriptionAttribute("Eject on close")]
        Eject = 0x0003
    }

    [FlagsAttribute]
    public enum SCardState : int
    {
        [DescriptionAttribute("Unknown state")]
        Unknown = 0x0001,
        [DescriptionAttribute("Card is absent")]
        Absent = 0x0002,
        [DescriptionAttribute("Card is present")]
        Present = 0x0004,
        [DescriptionAttribute("Card not powered")]
        Swallowed = 0x0008,
        [DescriptionAttribute("Card is powered")]
        Powered = 0x0010,
        [DescriptionAttribute("Ready for PTS")]
        Negotiable = 0x0020,
        [DescriptionAttribute("PTS has been set")]
        Specific = 0x0040
    }

    public enum SCardClass : int
    {
        [DescriptionAttribute("Vendor information definitions")]
        VendorInfo = 1,
        [DescriptionAttribute("Communication definitions")]
        Communications = 2,
        [DescriptionAttribute("Protocol definitions")]
        Protocol = 3,
        [DescriptionAttribute("Power Management definitions")]
        PowerMgmt = 4,
        [DescriptionAttribute("Security Assurance definitions")]
        Security = 5,
        [DescriptionAttribute("Mechanical characteristic definitions")]
        Mechanical = 6,
        [DescriptionAttribute("Vendor specific definitions")]
        VendorDefined = 7,
        [DescriptionAttribute("Interface Device Protocol options")]
        IFDProtocol = 8,
        [DescriptionAttribute("ICC State specific definitions")]
        ICCState = 9,
        [DescriptionAttribute("System-specific definitions")]
        System = 0x7fff
    }

    public enum SCardAttr : int
    {
        VendorName = (SCardClass.VendorInfo << 16) | 0x0100,
        VendorIFDType = (SCardClass.VendorInfo << 16) | 0x0101,
        VendorIFDVersion = (SCardClass.VendorInfo << 16) | 0x0102,
        VendorIFDSerialNo = (SCardClass.VendorInfo << 16) | 0x0103,
        ChannelId = (SCardClass.Communications << 16) | 0x0110,
        AsyncProtocolTypes = (SCardClass.Protocol << 16) | 0x0120,
        DefaultClk = (SCardClass.Protocol << 16) | 0x0121,
        MaxClk = (SCardClass.Protocol << 16) | 0x0122,
        DefaultDataRate = (SCardClass.Protocol << 16) | 0x0123,
        MaxDataRate = (SCardClass.Protocol << 16) | 0x0124,
        MaxIfsd = (SCardClass.Protocol << 16) | 0x0125,
        SyncProtocolTypes = (SCardClass.Protocol << 16) | 0x0126,
        PowerMgmtSupport = (SCardClass.PowerMgmt << 16) | 0x0131,
        UserToCardAuthDevice = (SCardClass.Security << 16) | 0x0140,
        UserAuthInputDevice = (SCardClass.Security << 16) | 0x0142,
        Characteristics = (SCardClass.Mechanical << 16) | 0x0150,

        CurrentProtocolType = (SCardClass.IFDProtocol << 16) | 0x0201,
        CurrentClk = (SCardClass.IFDProtocol << 16) | 0x0202,
        CurrentF = (SCardClass.IFDProtocol << 16) | 0x0203,
        CurrentD = (SCardClass.IFDProtocol << 16) | 0x0204,
        CurrentN = (SCardClass.IFDProtocol << 16) | 0x0205,
        CurrentW = (SCardClass.IFDProtocol << 16) | 0x0206,
        CurrentIfsc = (SCardClass.IFDProtocol << 16) | 0x0207,
        CurrentIfsd = (SCardClass.IFDProtocol << 16) | 0x0208,
        CurrentBwt = (SCardClass.IFDProtocol << 16) | 0x0209,
        CurrentCwt = (SCardClass.IFDProtocol << 16) | 0x020a,
        CurrentEbcEncoding = (SCardClass.IFDProtocol << 16) | 0x020b,
        ExtendedBwt = (SCardClass.IFDProtocol << 16) | 0x020c,

        ICCPresence = (SCardClass.ICCState << 16) | 0x0300,
        ICCInterfaceStatus = (SCardClass.ICCState << 16) | 0x0301,
        CurrentIOState = (SCardClass.ICCState << 16) | 0x0302,
        ATRString = (SCardClass.ICCState << 16) | 0x0303,
        ICCTypePerATR = (SCardClass.ICCState << 16) | 0x0304,

        EscReset = (SCardClass.VendorDefined << 16) | 0xA000,
        EscCancel = (SCardClass.VendorDefined << 16) | 0xA003,
        EscAuthRequest = (SCardClass.VendorDefined << 16) | 0xA005,
        MaxInput = (SCardClass.VendorDefined << 16) | 0xA007,

        DeviceUnit = (SCardClass.System << 16) | 0x0001,
        DeviceInUse = (SCardClass.System << 16) | 0x0002,
        DeviceFriendlyNameA = (SCardClass.System << 16) | 0x0003,
        DeviceSystemNameA = (SCardClass.System << 16) | 0x0004,
        DeviceFriendlyNameW = (SCardClass.System << 16) | 0x0005,
        DeviceSystemNameW = (SCardClass.System << 16) | 0x0006,
        SupressT1IFSRequest = (SCardClass.System << 16) | 0x0007,

        DeviceFriendlyName = DeviceFriendlyNameW,
        DeviceSystemName = DeviceSystemNameW

        /* ASCII *
        DEVICE_FRIENDLY_NAME     = DEVICE_FRIENDLY_NAME_A,
        DEVICE_SYSTEM_NAME       = DEVICE_SYSTEM_NAME_A
        */
    }


    public class SCardReader : ISCardReader
    {
        public SCardReader(SCardContext context)
        {
            this.context = context;
        }

        public SCardError Connect(string name,
                                  SCardShareMode mode,
                                  SCardProtocol prefProto)
        {
            if (name == null ||
                name.Equals(""))
                throw new UnknownReaderException(SCardError.InvalidValue, "Invalid card reader name.");

            if (context == null || context.contextPtr.Equals(IntPtr.Zero))
                throw new InvalidContextException(SCardError.InvalidHandle, "Invalid connection context.");

            SCardError rc;
            IntPtr hCard = IntPtr.Zero;
            SCardProtocol dwActiveProtocol;

            rc = SCardAPI.Lib.Connect(context.contextPtr,
                              name,
                              mode,
                              prefProto,
                              out hCard,
                              out dwActiveProtocol);

            if (rc == SCardError.Success)
            {
                cardHandle = hCard;
                activeprot = dwActiveProtocol;
                readername = name;
                sharemode = mode;
            }

            return rc;
        }

        public SCardError Disconnect(SCardReaderDisposition discntExec)
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            SCardError rc = SCardAPI.Lib.Disconnect(cardHandle,
                discntExec);

            if (rc == SCardError.Success)
            {
                // reset local variables
                readername = null;
                cardHandle = IntPtr.Zero;
                activeprot = SCardProtocol.Unset;
                sharemode = SCardShareMode.Shared;
            }

            return rc;
        }


        public SCardError Reconnect(SCardShareMode mode,
                                   SCardProtocol prefProto,
                                   SCardReaderDisposition initExec)
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            SCardProtocol dwActiveProtocol = activeprot;

            SCardError rc = SCardAPI.Lib.Reconnect(cardHandle,
                                         mode,
                                         prefProto,
                                         initExec,
                                         out dwActiveProtocol);


            if (rc == SCardError.Success)
            {
                activeprot = dwActiveProtocol;
                sharemode = mode;
            }

            return rc;
        }

        public SCardError BeginTransaction()
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            return SCardAPI.Lib.BeginTransaction(cardHandle);
        }

        public SCardError EndTransaction(SCardReaderDisposition disposition)
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            return SCardAPI.Lib.EndTransaction(cardHandle, disposition);
        }

        public SCardError Transmit(SCardPCI ioSendPci,
                                  byte[] sendBuffer,
                                  SCardPCI ioRecvPci,
                                  ref byte[] recvBuffer)
        {
            if (ioSendPci == null)
                throw new ArgumentNullException("ioSendPci");
            if (ioSendPci.iomem == IntPtr.Zero)
                throw new ArgumentException("ioSendPci");

            return Transmit(ioSendPci.iomem,
                sendBuffer,
                ioRecvPci,
                ref recvBuffer);
        }


        public SCardError Transmit(IntPtr pioSendPci,
                                  byte[] sendBuffer,
                                  ref byte[] recvBuffer)
        {
            return Transmit(pioSendPci,
                            sendBuffer,
                            null,
                            ref recvBuffer);
        }

        public SCardError Transmit(IntPtr pioSendPci,
                                  byte[] sendBuffer,
                                  int sendBufLength,
                                  SCardPCI ioRecvPci,
                                  byte[] recvBuffer,
                                  ref int recvBufLength)
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            IntPtr ioRecvPciPtr = IntPtr.Zero;
            if (ioRecvPci != null)
                ioRecvPciPtr = ioRecvPci.iomem;

            SCardError rc = SCardAPI.Lib.Transmit(cardHandle,
                                                pioSendPci,
                                                sendBuffer,
                                                sendBufLength,
                                                ioRecvPciPtr,
                                                recvBuffer,
                                                ref recvBufLength);
            return rc;

        }
        public SCardError Transmit(IntPtr pioSendPci,
                                  byte[] sendBuffer,
                                  SCardPCI ioRecvPci,
                                  ref byte[] recvBuffer)
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            int pcbRecvLength;

            IntPtr ioRecvPciPtr = IntPtr.Zero;
            if (ioRecvPci != null)
                ioRecvPciPtr = ioRecvPci.iomem;

            SCardError rc = SCardAPI.Lib.Transmit(cardHandle,
                                                pioSendPci,
                                                sendBuffer,
                                                ioRecvPciPtr,
                                                recvBuffer,
                                                out pcbRecvLength);

            if (rc == SCardError.Success)
            {
                if (recvBuffer != null &&
                    pcbRecvLength < recvBuffer.Length)
                        Array.Resize<byte>(ref recvBuffer, (int)pcbRecvLength);
            }

            return rc;
        }

        public SCardError Transmit(byte[] sendBuffer,
                    int sendBufferLength,
                    byte[] recvBuffer,
                    ref int recvBufferLength)
        {
            SCardPCI iorecvpci = new SCardPCI(); // will be discarded
            return Transmit(
                SCardPCI.GetPci(activeprot),
                sendBuffer,
                sendBufferLength,
                iorecvpci,
                recvBuffer,
                ref recvBufferLength);
        }

        public SCardError Transmit(byte[] sendBuffer,
                      byte[] recvBuffer,
                      ref int recvBufferLength)
        {
            int sendbufsize = 0;
            if (sendBuffer != null)
                sendbufsize = sendBuffer.Length;

            return Transmit(sendBuffer,
                sendbufsize,
                recvBuffer,
                ref recvBufferLength);
        }

        public SCardError Transmit(byte[] sendBuffer,
                    ref byte[] recvBuffer)
        {
            int recvbufsize = 0;
            if (recvBuffer != null)
                recvbufsize = recvBuffer.Length;

            SCardError sc = Transmit(sendBuffer,
                recvBuffer,
                ref recvbufsize);

            if (sc == SCardError.Success)
                if (recvBuffer != null &&
                    recvbufsize < recvBuffer.Length)
                    Array.Resize<byte>(ref recvBuffer, recvbufsize);

            return sc;
        }

        public SCardError Control(IntPtr ControlCode,
                                 byte[] SendBuffer,
                                 ref byte[] RecvBuffer)
        {
            if (cardHandle.Equals(IntPtr.Zero))
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");

            int lpBytesReturned;

            SCardError rc = SCardAPI.Lib.Control(cardHandle,
                                               ControlCode,
                                               SendBuffer,
                                               RecvBuffer,
                                               out lpBytesReturned);

            if (rc == SCardError.Success)
                if (RecvBuffer != null)
                    if (lpBytesReturned < RecvBuffer.Length)
                        Array.Resize<byte>(ref RecvBuffer, (int)lpBytesReturned);

            return rc;
        }


        public SCardError Status(out string[] ReaderName,
                                 out SCardState State,
                                 out SCardProtocol Protocol,
                                 out byte[] Atr)
        {
            SCardError rc;
            IntPtr dwState = IntPtr.Zero;
            IntPtr dwProtocol = IntPtr.Zero;

            rc = SCardAPI.Lib.Status(cardHandle,
                out ReaderName,
                out dwState,
                out dwProtocol,
                out Atr);

            if (rc == SCardError.Success)
            {
                Protocol = SCardHelper.ToProto(dwProtocol);
                State = SCardHelper.ToState(dwState);

                // update local copies 
                activeprot = Protocol;
                if (ReaderName.Length >= 1)
                    readername = ReaderName[0];
            }
            else
            {
                Protocol = SCardProtocol.Unset;
                State = SCardState.Unknown;
            }

            return rc;
        }

        public SCardError GetAttrib(
            SCardAttr AttrId,
            out byte[] pbAttr)
        {
            return GetAttrib(
                (IntPtr)AttrId,
                out pbAttr);
        }
        public SCardError GetAttrib(
            IntPtr AttrId,
            out byte[] pbAttr)
        {
            int attrlen;

            // receive needed size for pbAttr
            SCardError rc = GetAttrib(AttrId, null, out attrlen);

            if (rc != SCardError.Success)
                throw new PCSCException(rc, SCardHelper.StringifyError(rc));

            pbAttr = new byte[attrlen];

            return GetAttrib(AttrId, pbAttr, out attrlen);
        }
        public SCardError GetAttrib(
            SCardAttr AttrId,
            byte[] pbAttr,
            out int AttrLen)
        {
            return GetAttrib(
                (IntPtr)AttrId,
                pbAttr,
                out AttrLen);
        }
        public SCardError GetAttrib(IntPtr dwAttrId,
                                    byte[] pbAttr,
                                    out int AttrLen)
        {
            SCardError rc = SCardAPI.Lib.GetAttrib(
                cardHandle,
                dwAttrId,
                pbAttr,
                out AttrLen);

            return rc;
        }

        public SCardError SetAttrib(
            SCardAttr attr,
            byte[] pbAttr)
        {
            return SetAttrib((IntPtr)attr, pbAttr, pbAttr.Length);
        }
        public SCardError SetAttrib(
            IntPtr attr,
            byte[] pbAttr)
        {
            return SetAttrib(attr, pbAttr, pbAttr.Length);
        }
        public SCardError SetAttrib(
            SCardAttr attr,
            byte[] pbAttr,
            int AttrSize)
        {
            return SetAttrib((IntPtr)attr, pbAttr, AttrSize);
        }
        public SCardError SetAttrib(
            IntPtr attr,
            byte[] pbAttr,
            int AttrSize)
        {
            return SCardAPI.Lib.SetAttrib(
                cardHandle,
                attr,
                pbAttr,
                AttrSize);
        }


        private string readername;
        public string ReaderName
        {
            get { return readername; }
            protected set { readername = value; }
        }

        private SCardContext context;
        public SCardContext CurrentContext
        {
            get { return context; }
            protected set { context = value; }
        }

        private SCardShareMode sharemode;
        public SCardShareMode CurrentShareMode
        {
            get { return sharemode; }
            protected set { sharemode = value; }
        }

        private SCardProtocol activeprot;
        public SCardProtocol ActiveProtocol
        {
            get { return activeprot; }
            protected set { activeprot = value; }
        }

        private IntPtr cardHandle;
        public IntPtr CardHandle
        {
            get { return cardHandle; }
        }

        public static IntPtr Infinite
        {
            get { return SCardContext.Infinite; }
        }
    }
}
