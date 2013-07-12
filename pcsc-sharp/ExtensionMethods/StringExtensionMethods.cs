namespace PCSC
{
    internal static class StringExtensionMethods
    {
        public static bool IsNullOrWhiteSpace(this string @string) {
            if (@string == null) {
                return true;
            }

            return @string.Trim() == string.Empty;
        }
    }
}