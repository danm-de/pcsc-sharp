namespace PCSC.Iso7816
{
    /// <summary>
    /// File share mode
    /// </summary>
    public enum FileShareMode : byte
    {
        /// <summary>Shareable file that supports at least concurrent access on different logical channels.</summary>
        Shareable                   = 0x40,
        /// <summary>Non shareable file.</summary>
        NotShareable                = 0x0
    }
}