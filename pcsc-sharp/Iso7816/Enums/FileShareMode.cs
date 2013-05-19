namespace PCSC.Iso7816
{
    /// <summary>
    /// File share mode
    /// </summary>
    public enum FileShareMode : byte
    {
        /// <summary>Shareable file</summary>
        Shareable                   = 0x40,
        /// <summary>Not shareable file</summary>
        NotShareable                = 0x0
    }
}