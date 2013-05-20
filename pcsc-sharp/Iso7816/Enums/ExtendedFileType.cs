namespace PCSC.Iso7816
{
    /// <summary>Category of file types</summary>
    public enum ExtendedFileType
    {
        /// <summary>Working elementary file (Working EF) that is intended for storing data not interpreted by the card.</summary>
        WorkingElementary,
        /// <summary>Internal elementary file (Internal EF) that is intended for storing data interpreted and/or analyzed by the card and for control purposes.</summary>
        InternalElementary,
        /// <summary>Proprietary file type.</summary>
        Proprietary,
        /// <summary>Dedicated file (DF) that is used for logical organization of data in a card.</summary>
        Dedicated
    }
}