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
using System.ComponentModel;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using PCSC.Interop;

namespace PCSC
{
    [FlagsAttribute]
    public enum SCRState : int
    {
        [DescriptionAttribute("Application wants status")]
        Unaware = 0x0000,
        [DescriptionAttribute("Ignore this reader")]
        Ignore = 0x0001,
        [DescriptionAttribute("State has changed")]
        Changed = 0x0002,
        [DescriptionAttribute("Reader unknown")]
        Unknown = 0x0004,

        [DescriptionAttribute("Status unavailable")]
        Unavailable = 0x0008,
        [DescriptionAttribute("Card removed")]
        Empty = 0x0010,
        [DescriptionAttribute("Card inserted")]
        Present = 0x0020,
        [DescriptionAttribute("ATR matches card")]
        ATRMatch = 0x0040,
        [DescriptionAttribute("Exclusive Mode")]
        Exclusive = 0x0080,
        [DescriptionAttribute("Shared Mode")]
        InUse = 0x0100,
        [DescriptionAttribute("Unresponsive card")]
        Mute = 0x0200,
        [DescriptionAttribute("Unpowered card")]
        Unpowered = 0x0400
    }


    public class SCardReaderState : IDisposable
    {
        // we're getting values greater than 0xFFFF back from SCardGetStatusChange 
        private const int _EVENTSTATE_RANGE = 0xFFFF;
        private const long _CHCOUNT_RANGE = 0xFFFF0000;

        internal WinSCardAPI.SCARD_READERSTATE winscard_rstate;
        internal PCSCliteAPI.SCARD_READERSTATE pcsclite_rstate;

        private IntPtr pReaderName = IntPtr.Zero;
        private int pReaderNameSize = 0;
        private bool disposed = false;

        ~SCardReaderState()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                if (pReaderName != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pReaderName);
                    pReaderName = IntPtr.Zero;
                    pReaderNameSize = 0;
                }

                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public SCardReaderState()
        {
            if (SCardAPI.IsWindows)
            {
                winscard_rstate = new WinSCardAPI.SCARD_READERSTATE();

                // initialize embedded array
                winscard_rstate.rgbAtr = new byte[WinSCardAPI.MAX_ATR_SIZE];
                winscard_rstate.cbAtr = (Int32)WinSCardAPI.MAX_ATR_SIZE;
            }
            else
            {
                pcsclite_rstate = new PCSCliteAPI.SCARD_READERSTATE();

                // initialize embedded array
                pcsclite_rstate.rgbAtr = new byte[PCSCliteAPI.MAX_ATR_SIZE];
                pcsclite_rstate.cbAtr = (IntPtr)PCSCliteAPI.MAX_ATR_SIZE;
            }
        }

        public long UserData
        {
            get
            {
                return (long)UserDataPointer;
            }
            set
            {
                UserDataPointer = unchecked((IntPtr)value);
            }
        }

        public IntPtr UserDataPointer
        {
            get
            {
                if (SCardAPI.IsWindows)
                    return winscard_rstate.pvUserData;
                else
                    return pcsclite_rstate.pvUserData;
            }
            set
            {
                if (SCardAPI.IsWindows)
                    winscard_rstate.pvUserData = value;
                else
                    pcsclite_rstate.pvUserData = value;
            }
        }

        public SCRState CurrentState
        {
            get
            {
                if (SCardAPI.IsWindows)
                    return SCardHelper.ToSCRState(
                        ((int)winscard_rstate.dwCurrentState & _EVENTSTATE_RANGE));
                else
                    return SCardHelper.ToSCRState(
                        ((int)pcsclite_rstate.dwCurrentState & _EVENTSTATE_RANGE));
            }
            set
            {
                if (SCardAPI.IsWindows)
                    winscard_rstate.dwCurrentState =
                        (Int32)((int)value & _EVENTSTATE_RANGE);
                else
                    pcsclite_rstate.dwCurrentState =
                        (IntPtr)((int)value & _EVENTSTATE_RANGE);
            }
        }

        public SCRState EventState
        {
            get
            {
                if (SCardAPI.IsWindows)
                    return SCardHelper.ToSCRState(
                        (((int)winscard_rstate.dwEventState) & _EVENTSTATE_RANGE));
                else
                    return SCardHelper.ToSCRState(
                        (((int)pcsclite_rstate.dwEventState) & _EVENTSTATE_RANGE));
            }
            set
            {
                long l = CardChangeEventCnt; // save card change event counter
                if (SCardAPI.IsWindows)
                    winscard_rstate.dwEventState = (Int32)
                        (((int)value & _EVENTSTATE_RANGE) | (int)l);
                else
                    pcsclite_rstate.dwEventState = (IntPtr)
                        (((int)value & _EVENTSTATE_RANGE) | (int)l);

            }
        }

        public IntPtr EventStateValue
        {
            get
            {
                if (SCardAPI.IsWindows)
                    return (IntPtr)winscard_rstate.dwEventState;
                else
                    return (IntPtr)pcsclite_rstate.dwEventState;
            }
            set
            {
                if (SCardAPI.IsWindows)
                    // On a 64-bit platforms .ToInt32() will throw an OverflowException 
                    winscard_rstate.dwEventState = unchecked((Int32)value.ToInt64());
                else
                    pcsclite_rstate.dwEventState = (IntPtr)value;
            }
        }
        public IntPtr CurrentStateValue
        {
            get
            {
                if (SCardAPI.IsWindows)
                    return (IntPtr)winscard_rstate.dwCurrentState;
                else
                    return (IntPtr)pcsclite_rstate.dwCurrentState;
            }
            set
            {
                if (SCardAPI.IsWindows)
                    // On a 64-bit platform .ToInt32() will throw an OverflowException 
                    winscard_rstate.dwCurrentState = unchecked((Int32)value.ToInt64()); 
                else
                    pcsclite_rstate.dwCurrentState = (IntPtr)value;
            }
        }

        public int CardChangeEventCnt
        {
            get
            {
                if (SCardAPI.IsWindows)
                    return (int)((
                        ((long)winscard_rstate.dwEventState) & _CHCOUNT_RANGE) >> 16);
                else
                    return (int)((
                        ((long)pcsclite_rstate.dwEventState) & _CHCOUNT_RANGE) >> 16);
            }
            set
            {
                long e = (long)EventState; // save event state
                if (SCardAPI.IsWindows)
                    winscard_rstate.dwEventState = unchecked((Int32)
                        (((long)(value & _CHCOUNT_RANGE) << 16) | e));
                else
                    pcsclite_rstate.dwEventState = unchecked((IntPtr)
                        (((long)(value & _CHCOUNT_RANGE) << 16) | e));
            }
        }

        public string ReaderName
        {
            get
            {
                if (pReaderName == IntPtr.Zero)
                    return null;

                byte[] tmp = new byte[pReaderNameSize];
                Marshal.Copy(pReaderName, tmp, 0, pReaderNameSize);
                return SCardHelper._ConvertToString(tmp, tmp.Length, SCardAPI.Lib.TextEncoding);
            }
            set
            {
                // Free reserved memory
                if (pReaderName != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem(pReaderName);
                    pReaderName = IntPtr.Zero;
                    pReaderNameSize = 0;
                }

                if (value != null)
                {
                    byte[] tmp = SCardHelper._ConvertToByteArray(value, SCardAPI.Lib.TextEncoding, 0);
                    pReaderName = Marshal.AllocCoTaskMem(tmp.Length + SCardAPI.Lib.CharSize);
                    pReaderNameSize = tmp.Length;
                    Marshal.Copy(tmp, 0, pReaderName, tmp.Length);
                    for (int i = 0; i < (SCardAPI.Lib.CharSize); i++)
                    {
                        Marshal.WriteByte(pReaderName, tmp.Length + i, (byte)0); // String ends with \0 (or 0x00 0x00)
                    }

                }

                if (SCardAPI.IsWindows)
                    winscard_rstate.pszReader = pReaderName;
                else
					pcsclite_rstate.pszReader = pReaderName;
            }
        }

        public byte[] ATR
        {
            get
            {
                byte[] tmp = null;

                if (SCardAPI.IsWindows)
                {
                    if ((int)winscard_rstate.cbAtr <= WinSCardAPI.MAX_ATR_SIZE)
                        tmp = new byte[(int)winscard_rstate.cbAtr];
                    else
                    { // error occurred during SCardGetStatusChange()
                        tmp = new byte[WinSCardAPI.MAX_ATR_SIZE];
                        winscard_rstate.cbAtr = (Int32)WinSCardAPI.MAX_ATR_SIZE;
                    }
                    Array.Copy(winscard_rstate.rgbAtr, tmp, (int)winscard_rstate.cbAtr);
                }
                else
                {
                    if ((int)pcsclite_rstate.cbAtr <= PCSCliteAPI.MAX_ATR_SIZE)
                        tmp = new byte[(int)pcsclite_rstate.cbAtr];
                    else
                    { // error occurred during SCardGetStatusChange()
                        tmp = new byte[PCSCliteAPI.MAX_ATR_SIZE];
                        pcsclite_rstate.cbAtr = (IntPtr)PCSCliteAPI.MAX_ATR_SIZE;
                    }
                    Array.Copy(pcsclite_rstate.rgbAtr, tmp, (int)pcsclite_rstate.cbAtr);
                }

                return tmp;
            }
            set
            {
                byte[] tmp = value;
                // the size of rstate.rgbAtr MUST(!) be MAX_ATR_SIZE 
                if (SCardAPI.IsWindows)
                {
                    if (tmp.Length != WinSCardAPI.MAX_ATR_SIZE)
                        Array.Resize<byte>(ref tmp, WinSCardAPI.MAX_ATR_SIZE);
                    winscard_rstate.rgbAtr = tmp;
                    winscard_rstate.cbAtr = (Int32)value.Length;
                }
                else
                {
                    if (tmp.Length != PCSCliteAPI.MAX_ATR_SIZE)
                        Array.Resize<byte>(ref tmp, PCSCliteAPI.MAX_ATR_SIZE);
                    pcsclite_rstate.rgbAtr = tmp;
                    pcsclite_rstate.cbAtr = (IntPtr)value.Length;
                }
            }
        }
    }
}
