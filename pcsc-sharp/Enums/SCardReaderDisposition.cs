using System.ComponentModel;

namespace PCSC
{
    public enum SCardReaderDisposition
    {
        [Description("Do nothing on close")]
        Leave = 0x0000,
        [Description("Reset on close")]
        Reset = 0x0001,
        [Description("Power down on close")]
        Unpower = 0x0002,
        [Description("Eject on close")]
        Eject = 0x0003
    }
}