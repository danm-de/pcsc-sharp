using System.ComponentModel;

namespace PCSC
{
    public enum SCardReaderDisposition
    {
        [Description("Do nothing")]
        Leave = 0x0000,
        [Description("Reset the reader")]
        Reset = 0x0001,
        [Description("Power down the reader")]
        Unpower = 0x0002,
        [Description("Eject")]
        Eject = 0x0003
    }
}