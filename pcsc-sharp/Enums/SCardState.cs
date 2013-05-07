using System;
using System.ComponentModel;

namespace PCSC
{
    [Flags]
    public enum SCardState
    {
        [Description("Unknown state")]
        Unknown = 0x0001,
        [Description("Card is absent")]
        Absent = 0x0002,
        [Description("Card is present")]
        Present = 0x0004,
        [Description("Card not powered")]
        Swallowed = 0x0008,
        [Description("Card is powered")]
        Powered = 0x0010,
        [Description("Ready for PTS")]
        Negotiable = 0x0020,
        [Description("PTS has been set")]
        Specific = 0x0040
    }
}