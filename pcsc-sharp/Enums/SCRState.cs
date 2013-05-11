using System;
using System.ComponentModel;

namespace PCSC
{
    [Flags]
    public enum SCRState
    {
        [Description("Application wants status")]
        Unaware = 0x0000,
        [Description("Ignore this reader")]
        Ignore = 0x0001,
        [Description("State has changed")]
        Changed = 0x0002,
        [Description("Reader unknown")]
        Unknown = 0x0004,

        [Description("Status unavailable")]
        Unavailable = 0x0008,
        [Description("Card removed")]
        Empty = 0x0010,
        [Description("Card inserted")]
        Present = 0x0020,
        [Description("ATR matches card")]
        AtrMatch = 0x0040,
        [Description("Exclusive Mode")]
        Exclusive = 0x0080,
        [Description("Shared Mode")]
        InUse = 0x0100,
        [Description("Unresponsive card")]
        Mute = 0x0200,
        [Description("Unpowered card")]
        Unpowered = 0x0400
    }
}