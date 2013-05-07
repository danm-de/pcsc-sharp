using System.ComponentModel;

namespace PCSC
{
    public enum SCardShareMode
    {
        [Description("Exclusive mode only")]
        Exclusive = 0x0001,
        [Description("Shared mode only")]
        Shared = 0x0002,
        [Description("Raw mode only")]
        Direct = 0x0003
    }
}