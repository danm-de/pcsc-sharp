using System;
using System.ComponentModel;

namespace PCSC
{
    [Flags]
    public enum SCardProtocol
    {
        [Description("Protocol not set")]
        Unset = 0x0000,
        [Description("T=0 active protocol")]
        T0 = 0x0001,
        [Description("T=1 active protocol")]
        T1 = 0x0002,
        [Description("Raw active protocol")]
        Raw = 0x0004,
        [Description("T=15 protocol")]
        T15 = 0x0008,

        [Description("IFD determines protocol")]
        Any = (T0 | T1)
    }
}