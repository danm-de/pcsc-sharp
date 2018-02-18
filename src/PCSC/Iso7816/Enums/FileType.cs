namespace PCSC.Iso7816
{
    /// <summary>File type</summary>
    public enum FileType
    {
        /// <summary>Dedicated file (DF) that is used for logical organization of data in the card.</summary>
        Dedicated,
        /// <summary>Elementary file (EF) that is used for storing data.</summary>
        /// <remarks>The data may or may not be interpreted/analyzed by the card. You need to check the extended file type <see cref="ExtendedFileType"/>.</remarks>
        Elementary,
    }
}