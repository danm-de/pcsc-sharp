namespace PCSC.Iso7816.Extensions
{
    internal static class ByteExt
    {
        public static bool IsSet(this byte value, byte mask, byte bits) {
            return ((value & mask) == bits);
        }
    }
}
