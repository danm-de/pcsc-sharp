using System;

namespace PCSC.Interop.Unix.ExtensionMethods
{
	internal static class IntPtrExt
	{
		public static IntPtr Mask(this IntPtr value, long mask) {
			unchecked {
				var new_value = value.ToInt64() & mask;

				return new IntPtr(new_value);
			}
		}
	}
}