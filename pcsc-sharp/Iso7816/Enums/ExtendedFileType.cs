namespace PCSC.Iso7816
{
    /// <summary>Category of file types</summary>
    public enum ExtendedFileType
    {
        /// <summary>Working elementary file (Working EF)</summary>
        WorkingElementary,
        /// <summary>Internal elementary file (Internal EF)</summary>
        InternalElementary,
        /// <summary>Proprietary file type</summary>
        Proprietary,
        /// <summary>Dedicated file (DF)</summary>
        Dedicated
    }
}