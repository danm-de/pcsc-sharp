using PCSC.Interop;

namespace PCSC.Iso7816
{
    internal static class SCardErrorExtensionMethods
    {
        public static bool HasInsufficientBuffer(this SCardError sc) {
            return sc == SCardError.InsufficientBuffer
                   || (Platform.IsWindows && sc == SCardError.WinErrorInsufficientBuffer);
        }
    }
}