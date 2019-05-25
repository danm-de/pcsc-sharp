namespace PCSC.Interop.MacOSX.ExtensionMethods {
    internal static class IntExt {
        public static int Mask(this int value, int mask) => value & mask;
    }
}
