using PCSC.Interop;

namespace PCSC.Iso7816.Extensions
{
    internal static class SCardErrorExt
    {
        public static bool HasInsufficientBuffer(this SCardError sc) {
            return sc == SCardError.InsufficientBuffer
                   || (Platform.IsWindows && sc == SCardError.WinErrorInsufficientBuffer);
        }
    }
}
