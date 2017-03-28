using System;

namespace PCSC.Interop.Windows.Extensions
{
    internal static class IntExt
    {
        public static SCardState ConvertToSCardState(this int value) {
            var state = (SCardStateWindows) value;
            switch (state) {
                case SCardStateWindows.Unknown:
                    return SCardState.Unknown;
                case SCardStateWindows.Absent:
                    return SCardState.Absent;
                case SCardStateWindows.Present:
                    return SCardState.Present;
                case SCardStateWindows.Swallowed:
                    return SCardState.Swallowed;
                case SCardStateWindows.Powered:
                    return SCardState.Powered;
                case SCardStateWindows.Negotiable:
                    return SCardState.Negotiable;
                case SCardStateWindows.Specific:
                    return SCardState.Specific;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value),
                        "Unsupported CardReaderState value. See https://msdn.microsoft.com/en-us/library/cc242847.aspx for supported values.");
            }
        }
    }
}