namespace PCSC.Iso7816
{
    /// <summary>File structure types</summary>
    /// <remarks>The following structures of elementary files are defined:
    ///     <list type="bullet">
    ///         <item><description>Transparent structure; the EF contains a sequence of data units.</description></item>
    ///         <item><description>Record structure; the EF contains a sequence of individually identifiable records.</description></item>
    ///     </list>
    ///     <para>When using a record structure, the size of the particular records is either fixed or variable. Furthermore records are organized as a sequence (linear) or as a ring (cyclic).</para>
    /// </remarks>
    public enum FileStructureType : byte
    {
        /// <summary>No information about file structure given</summary>
        NoInformation = 0x0,
        /// <summary>Transparent. The EF has a sequence of data units</summary>
        Transparent = 0x1,
        /// <summary>Linear fixed, no further info</summary>
        LinearFixed = 0x2,
        /// <summary>Linear fixed SIMPLE-TLV (Type-length-value)</summary>
        LinearFixedSimpleTlv = 0x3,
        /// <summary>Linear variable, no further info</summary>
        LinearVariable = 0x4,
        /// <summary>Linear variable SIMPLE-TLV (Type-length-value)</summary>
        LinearVariableSimpleTlv = 0x5,
        /// <summary>Cyclic, no further info </summary>
        Cyclic = 0x6,
        /// <summary>Cyclic, SIMPLE-TLV (Type-length-value)</summary>
        CyclicSimpleTlv = 0x7
    }
}