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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace PCSC
{
    [Flags] // Needed for CONST casts
    public enum SCardError : int
    {
        [DescriptionAttribute("No error.")]
        Success = 0x00000000,
        [DescriptionAttribute("An internal consistency check failed.")]
        InternalError = unchecked((int)0x80100001),
        [DescriptionAttribute("The action was cancelled by an SCardCancel request.")]
        Cancelled = unchecked((int)0x80100002),
        [DescriptionAttribute("The supplied handle was invalid. ")]
        InvalidHandle = unchecked((int)0x80100003),
        [DescriptionAttribute("One or more of the supplied parameters could not be properly interpreted.")]
        InvalidParameter = unchecked((int)0x80100004),
        [DescriptionAttribute("Registry startup information is missing or invalid.")]
        InvalidTarget = unchecked((int)0x80100005),
        [DescriptionAttribute("Not enough memory available to complete this command.")]
        NoMemory = unchecked((int)0x80100006),
        [DescriptionAttribute("An internal consistency timer has expired.")]
        WaitedTooLong =unchecked((int)0x80100007),
        [DescriptionAttribute("The data buffer to receive returned data is too small for the returned data.")]
        InsufficientBuffer = unchecked((int)0x80100008),
        [DescriptionAttribute("The specified reader name is not recognized.")]
        UnknownReader = unchecked((int)0x80100009),
        [DescriptionAttribute("The user-specified timeout value has expired.")]
        Timeout = unchecked((int)0x8010000A),
        [DescriptionAttribute("The smart card cannot be accessed because of other connections outstanding.")]
        SharingViolation = unchecked((int)0x8010000B),
        [DescriptionAttribute("The operation requires a Smart Card, but no Smart Card is currently in the device.")]
        NoSmartcard = unchecked((int)0x8010000C),
        [DescriptionAttribute("The specified smart card name is not recognized.")]
        UnknownCard = unchecked((int)0x8010000D),
        [DescriptionAttribute("The system could not dispose of the media in the requested manner.")]
        CantDispose = unchecked((int)0x8010000E),
        [DescriptionAttribute("The requested protocols are incompatible with the protocol currently in use with the smart card.")]
        ProtocolMismatch = unchecked((int)0x8010000F),
        [DescriptionAttribute("The reader or smart card is not ready to accept commands.")]
        NotReady = unchecked((int)0x80100010),
        [DescriptionAttribute("One or more of the supplied parameters values could not be properly interpreted.")]
        InvalidValue = unchecked((int)0x80100011),
        [DescriptionAttribute("The action was cancelled by the system, presumably to log off or shut down.")]
        SystemCancelled = unchecked((int)0x80100012),
        [DescriptionAttribute("An internal communications error has been detected.")]
        CommunicationError = unchecked((int)0x80100013),
        [DescriptionAttribute("An internal error has been detected, but the source is unknown.")]
        UnknownError = unchecked((int)0x80100014),
        [DescriptionAttribute("An ATR obtained from the registry is not a valid ATR string.")]
        InvalidAtr = unchecked((int)0x80100015),
        [DescriptionAttribute("An attempt was made to end a non-existent transaction.")]
        NotTransacted = unchecked((int)0x80100016),
        [DescriptionAttribute("The specified reader is not currently available for use.")]
        ReaderUnavailable = unchecked((int)0x80100017),

        [DescriptionAttribute("The operation has been aborted to allow the server application to exit.")]
        Shutdown = unchecked((int)0x80100018),

        [DescriptionAttribute("The reader cannot communicate with the card, due to ATR string configuration conflicts.")]
        UnsupportedCard = unchecked((int)0x80100065),
        [DescriptionAttribute("The smart card is not responding to a reset.")]
        UnresponsiveCard = unchecked((int)0x80100066),
        [DescriptionAttribute("Power has been removed from the smart card, so that further communication is not possible.")]
        UnpoweredCard = unchecked((int)0x80100067),
        [DescriptionAttribute("The smart card has been reset, so any shared state information is invalid.")]
        ResetCard = unchecked((int)0x80100068),
        [DescriptionAttribute("The smart card has been removed, so further communication is not possible.")]
        RemovedCard = unchecked((int)0x80100069),

        [DescriptionAttribute("The PCI Receive buffer was too small.")]
        PciTooSmall = unchecked((int)0x80100019),
        [DescriptionAttribute("The reader driver does not meet minimal requirements for support.")]
        ReaderUnsupported = unchecked((int)0x8010001A),
        [DescriptionAttribute("The reader driver did not produce a unique reader name.")]
        DuplicateReader = unchecked((int)0x8010001B),
        [DescriptionAttribute("The smart card does not meet minimal requirements for support.")]
        CardUnsupported = unchecked((int)0x8010001C),
        [DescriptionAttribute("The Smart card resource manager is not running.")]
        NoService = unchecked((int)0x8010001D),
        [DescriptionAttribute("The Smart card resource manager has shut down.")]
        ServiceStopped = unchecked((int)0x8010001E),

        [DescriptionAttribute("An unexpected card error has occurred.")]
        Unexpected = unchecked((int)0x8010001F),

        [DescriptionAttribute("Cannot find a smart card reader.")]
        NoReadersAvailable = unchecked((int)0x8010002E),

        /** PC/SC Lite specific extensions */
        [DescriptionAttribute("Card inserted.")]
        [Obsolete("PC/SC Lite specific, value conflicts with SCardError.SecurityViolation")]
        InsertedCard = unchecked((int)0x8010006A),
        [DescriptionAttribute("Feature not supported")]
        UnsupportedFeature = unchecked((int)0x8010001F),

        [DescriptionAttribute("No primary provider can be found for the smart card.")]
        ICCInstallation = unchecked((int)0x80100020),
        [DescriptionAttribute("The requested order of object creation is not supported.")]
        ICCCreateOrder = unchecked((int)0x80100021),

        [DescriptionAttribute("The identified directory does not exist in the smart card.")]
        DirectoryNotFound = unchecked((int)0x80100023),
        [DescriptionAttribute("The identified file does not exist in the smart card.")]
        FileNotFound = unchecked((int)0x80100024),
        [DescriptionAttribute("The supplied path does not represent a smart card directory. ")]
        NoDir = unchecked((int)0x80100025),
        [DescriptionAttribute("The supplied path does not represent a smart card file.")]
        NoFile = unchecked((int)0x80100026),
        [DescriptionAttribute("Access is denied to this file.")]
        NoAccess = unchecked((int)0x80100027),

        [DescriptionAttribute("The smart card does not have enough memory to store the information.")]
        WriteTooMany = unchecked((int)0x80100028),
        [DescriptionAttribute("There was an error trying to set the smart card file object pointer.")]
        BadSeek = unchecked((int)0x80100029),

        [DescriptionAttribute("The supplied PIN is incorrect.")]
        InvalidCHV = unchecked((int)0x8010002A),
        [DescriptionAttribute("An unrecognized error code was returned from a layered component.")]
        UnknownResMng = unchecked((int)0x8010002B),

        [DescriptionAttribute("The requested certificate does not exist.")]
        NoSuchCertificate = unchecked((int)0x8010002C),
        [DescriptionAttribute("The requested certificate could not be obtained.")]
        CertificateUnavailable = unchecked((int)0x8010002D),
        [DescriptionAttribute("A communications error with the smart card has been detected.")]
        CommunicationDataLost = unchecked((int)0x8010002F),
        [DescriptionAttribute("The requested key container does not exist on the smart card.")]
        NoKeyContainer = unchecked((int)0x80100030),
        [DescriptionAttribute("The Smart Card Resource Manager is too busy to complete this operation.")]
        ServerTooBusy = unchecked((int)0x80100031),
        [DescriptionAttribute("Access was denied because of a security violation.")]
        SecurityViolation = unchecked((int)0x8010006A),
        [DescriptionAttribute("The card cannot be accessed because the wrong PIN was presented.")]
        WrongCHV = unchecked((int)0x8010006B),
        [DescriptionAttribute("The card cannot be accessed because the maximum number of PIN entry attempts has been reached.")]
        CHVBlocked = unchecked((int)0x8010006C),
        [DescriptionAttribute("The end of the smart card file has been reached.")]
        Eof = unchecked((int)0x8010006D),
        [DescriptionAttribute("The user pressed \"Cancel\" on a Smart Card Selection Dialog.")]
        CancelledByUser = unchecked((int)0x8010006E),
        [DescriptionAttribute("No PIN was presented to the smart card.")]
        CardNotAuthenticated = unchecked((int)0x8010006F)

    }

    /*
    public enum TAG_IFD : long {
        TAG_IFD_ATR                     = 0x0303,
        TAG_IFD_SLOTNUM                 = 0x0180,
        TAG_IFD_SLOT_THREAD_SAFE        = 0x0FAC,
        TAG_IFD_THREAD_SAFE             = 0x0FAD,
        TAG_IFD_SLOTS_NUMBER            = 0x0FAE,
        TAG_IFD_SIMULTANEOUS_ACCESS     = 0x0FAF
    }
    
    /// IFD Handler version number enummerations
    public enum IFD_HVERSION : long {
        IFD_HVERSION_1_0               = 0x00010000,
        IFD_HVERSION_2_0               = 0x00020000,
        IFD_HVERSION_3_0               = 0x00030000
    }
    
    public enum IFD_Handler : long {
        IFD_POWER_UP                    = 500,
        IFD_POWER_DOWN                  = 501,
        IFD_RESET                       = 502,

        IFD_NEGOTIATE_PTS1              = 1,
        IFD_NEGOTIATE_PTS2              = 2,
        IFD_NEGOTIATE_PTS3              = 4,

        IFD_SUCCESS                     = 0,
        IFD_ERROR_TAG                   = 600,
        IFD_ERROR_SET_FAILURE           = 601,
        IFD_ERROR_VALUE_READ_ONLY       = 602,
        IFD_ERROR_PTS_FAILURE           = 605,
        IFD_ERROR_NOT_SUPPORTED         = 606,
        IFD_PROTOCOL_NOT_SUPPORTED      = 607,
        IFD_ERROR_POWER_ACTION          = 608,
        IFD_ERROR_SWALLOW               = 609,
        IFD_ERROR_EJECT                 = 610,
        IFD_ERROR_CONFISCATE            = 611,
        IFD_COMMUNICATION_ERROR         = 612,
        IFD_RESPONSE_TIMEOUT            = 613,
        IFD_NOT_SUPPORTED               = 614,
        IFD_ICC_PRESENT                 = 615,
        IFD_ICC_NOT_PRESENT             = 616,
        IFD_NO_SUCH_DEVICE              = 617
    }
    
    */

    public class SCardHelper
    {
        public static string StringifyError(SCardError code)
        {
            return GetAttrDesc<SCardError>(code);
        }
        public static string GetAttrDesc<T>(T attr)
        {
            FieldInfo fieldinf = attr.GetType().GetField(attr.ToString());

            try
            {
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fieldinf.GetCustomAttributes(
                    typeof(DescriptionAttribute),
                    false);

                return (attributes.Length > 0) ? attributes[0].Description : attr.ToString();
            }
            catch
            {
                return "Error code: " + attr.ToString();
            }
        }

        internal static string[] _RemoveEmptyStrings(string[] array)
        {
            List<string> lst = new List<string>();
            foreach (string str in array)
                if (!str.Equals(""))
                    lst.Add(str);

            return lst.ToArray();
        }

        internal static byte[] _ConvertToByteArray(string[] array, Encoding encoder)
        {
            List<byte> lst = new List<byte>();

            foreach (string s in array)
            {
                if (s != null &&
                    !s.Equals(""))
                {
                    byte[] encstr = encoder.GetBytes(s);
                    lst.AddRange(encstr);
                    lst.Add(0);
                }
            }
            lst.Add(0);

            return lst.ToArray();
        }

        internal static byte[] _ConvertToByteArray(string str, Encoding encoder, int suffixByteCount)
        {
            if (str == null)
                return null;
            if (suffixByteCount == 0)
            {
                return encoder.GetBytes(str);
            }
            else
            {
                int count = encoder.GetByteCount(str);
                byte[] tmp = new byte[count + suffixByteCount];
                encoder.GetBytes(str, 0, str.Length, tmp, 0);
                return tmp;
            }
        }


        internal static string[] _ConvertToStringArray(byte[] array, Encoding decoder)
        {
            if (array != null)
            {
                string tmp = decoder.GetString(array);
                return SCardHelper._RemoveEmptyStrings(tmp.Split('\0'));
            }
            return null;
        }


        internal static string _ConvertToString(byte[] array, int strlen, Encoding encoder)
        {
            if (array != null)
            {
                int count = array.Length;

                if (strlen < array.Length && strlen > 0)
                    count = strlen;

                return encoder.GetString(array, 0, count);
            }
            return null;
        }

        internal static SCardError ToSCardError(IntPtr result)
        {
            try
            {
                return (SCardError)result;
            }
            catch
            {
                return SCardError.UnknownError;
            }
        }
        internal static SCardError ToSCardError(Int32 result)
        {
            try
            {
                return (SCardError)result;
            }
            catch
            {
                return SCardError.UnknownError;
            }
        }

        internal static SCardProtocol ToProto(IntPtr proto)
        {
            try
            {
                return (SCardProtocol)proto;
            }
            catch
            {
                return SCardProtocol.Unset;
            }
        }

        internal static SCardState ToState(IntPtr state)
        {
            try
            {
                return (SCardState)state;
            }
            catch
            {
                return SCardState.Unknown;
            }
        }

        internal static SCRState ToSCRState(long state)
        {
            try
            {
                return (SCRState)state;
            }
            catch
            {
                return (SCRState.Unknown | SCRState.Changed | SCRState.Ignore);
            }
        }

        internal static bool IsSet(byte value, byte mask, byte bits)
        {
            return ((value & mask) == bits) ? true : false;
        }
    }


}
