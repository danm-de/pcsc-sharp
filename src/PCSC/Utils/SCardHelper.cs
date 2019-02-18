using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PCSC.Utils
{
    /// <summary>
    /// Helper class that offers methods to convert various values into strings.
    /// </summary>
    public static class SCardHelper
    {
        /// <summary>
        /// Returns a human readable text for the given PC/SC error code.
        /// </summary>
        /// <param name="code">Error or return code.</param>
        /// <returns>A human readable string.</returns>
        /// <remarks>Warning! This method behaves differently compared to the original PC/SC-Lite pcsc_stringify_error function. Instead of the (const) variable name it returns a short text description.</remarks>
        public static string StringifyError(SCardError code) {
            return GetAttrDesc(code);
        }

        /// <summary>
        /// Returns a description string of an enumeration attribute.
        /// </summary>
        /// <typeparam name="T">attribute type</typeparam>
        /// <param name="attr">attribute</param>
        /// <returns>If available: a description string of the specified attribute. Otherwise null.</returns>
        private static string GetAttrDesc<T>(T attr) {
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

        private static string[] RemoveEmptyStrings(IEnumerable<string> array) {
            return array
                .Where(str => !str.Equals(""))
                .ToArray();
        }

        internal static byte[] ConvertToByteArray(IEnumerable<string> array, Encoding encoder) {
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
            return (SCardError) unchecked((int) result.ToInt64());
        }

        internal static SCardError ToSCardError(int result) {
            return (SCardError) result;
        }

        internal static SCardProtocol ToProto(IntPtr proto) {
            return (SCardProtocol) unchecked((int) proto.ToInt64());
        }

        internal static SCardState ToState(IntPtr state) {
            return (SCardState) unchecked((int) state.ToInt64());
        }

        internal static SCRState ToSCRState(long state) {
            return (SCRState) unchecked((int) state);
        }
    }
}
