namespace PCSC.Iso7816
{
    /// <summary>File structure types</summary>
    public enum FileStructureType: byte
    {
        /// <summary>No information about file structure given</summary>
        NoInformation               = 0x0,
        /// <summary>Transparent</summary>
        Transparent                 = 0x1,
        /// <summary>Linear fixed, no further info</summary>
        LinearFixed                 = 0x2,
        /// <summary>Linear fixed SIMPLE-TLV (Type-length-value)</summary>
        LinearFixedSimpleTlv        = 0x3,
        /// <summary>Linear variable, no further info</summary>
        LinearVariable              = 0x4,
        /// <summary>Linear variable SIMPLE-TLV (Type-length-value)</summary>
        LinearVariableSimpleTlv     = 0x5,
        /// <summary>Cyclic, no further info </summary>
        Cyclic                      = 0x6,
        /// <summary>Cyclic, SIMPLE-TLV (Type-length-value)</summary>
        CyclicSimpleTlv             = 0x7
    }
}