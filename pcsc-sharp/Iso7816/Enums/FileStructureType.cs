namespace PCSC.Iso7816
{
    public enum FileStructureType: byte
    {
        NoInformation               = 0x0,
        Transparent                 = 0x1,
        LinearFixed                 = 0x2,
        LinearFixedSimpleTlv        = 0x3,
        LinearVariable              = 0x4,
        LinearVariableSimpleTlv     = 0x5,
        Cyclic                      = 0x6,
        CyclicSimpleTlv             = 0x7
    }
}