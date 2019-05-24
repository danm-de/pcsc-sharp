using System;

namespace PCSC.Interop.MacOSX.ExtensionMethods {
    internal static class IntPtrExt {
        public static IntPtr Mask(this IntPtr value, long mask) {
            unchecked {
                var newValue = value.ToInt64() & mask;

                return new IntPtr(newValue);
            }
        }
    }
}
