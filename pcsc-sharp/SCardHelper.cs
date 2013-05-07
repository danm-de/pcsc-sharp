using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PCSC
{
    public class SCardHelper
    {
        public static string StringifyError(SCardError code) {
            return GetAttrDesc(code);
        }

        public static string GetAttrDesc<T>(T attr) {
            var fieldinf = attr
                .GetType()
                .GetField(attr.ToString());

            try {
                var attributes = (DescriptionAttribute[]) fieldinf
                    .GetCustomAttributes(
                        typeof(DescriptionAttribute),
                        false);

                return (attributes.Length > 0)
                    ? attributes[0].Description
                    : attr.ToString();
            } catch {
                return "Error code: " + attr;
            }
        }

        internal static string[] RemoveEmptyStrings(string[] array) {
            return array
                .Where(str => !str.Equals(""))
                .ToArray();
        }

        internal static byte[] ConvertToByteArray(string[] array, Encoding encoder) {
            var lst = new List<byte>();

            foreach (var s in array) {
                if (string.IsNullOrWhiteSpace(s)) {
                    continue;
                }

                var encstr = encoder.GetBytes(s);
                lst.AddRange(encstr);
                lst.Add(0);
            }
            lst.Add(0);

            return lst.ToArray();
        }

        internal static byte[] ConvertToByteArray(string str, Encoding encoder, int suffixByteCount) {
            if (str == null) {
                return null;
            }
            if (suffixByteCount == 0) {
                return encoder.GetBytes(str);
            }

            var count = encoder.GetByteCount(str);
            var tmp = new byte[count + suffixByteCount];
            
            encoder.GetBytes(str, 0, str.Length, tmp, 0);
            return tmp;
        }


        internal static string[] ConvertToStringArray(byte[] array, Encoding decoder) {
            if (array == null) {
                return null;
            }

            var tmp = decoder.GetString(array);
            return RemoveEmptyStrings(tmp.Split('\0'));
        }


        internal static string ConvertToString(byte[] array, int strlen, Encoding encoder) {
            if (array == null) {
                return null;
            }

            var count = array.Length;

            if (strlen < array.Length && strlen > 0) {
                count = strlen;
            }

            return encoder.GetString(array, 0, count);
        }

        internal static SCardError ToSCardError(IntPtr result) {
            try {
                return (SCardError) result;
            } catch {
                return SCardError.UnknownError;
            }
        }

        internal static SCardError ToSCardError(Int32 result) {
            try {
                return (SCardError) result;
            } catch {
                return SCardError.UnknownError;
            }
        }

        internal static SCardProtocol ToProto(IntPtr proto) {
            try {
                return (SCardProtocol) proto;
            } catch {
                return SCardProtocol.Unset;
            }
        }

        internal static SCardState ToState(IntPtr state) {
            try {
                return (SCardState) state;
            } catch {
                return SCardState.Unknown;
            }
        }

        internal static SCRState ToSCRState(long state) {
            try {
                return (SCRState) state;
            } catch {
                return (SCRState.Unknown | SCRState.Changed | SCRState.Ignore);
            }
        }

        internal static bool IsSet(byte value, byte mask, byte bits) {
            return ((value & mask) == bits);
        }
    }
}
