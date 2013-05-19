using System;
using System.ComponentModel;

namespace PCSC
{
    /// <summary>Communication protocol use with the Smart Card.</summary>
    /// <remarks>This is a bit mask of acceptable protocols for the connection. You can use (<see cref="F:PCSC.SCardProtocol.T0" /> | <see cref="F:PCSC.SCardProtocol.T1" />) if you do not have a preferred protocol. </remarks>
    [Flags]
    public enum SCardProtocol
    {
        /// <summary>
        /// Protocol not defined.
        /// </summary>
        [Description("Protocol not set")]
        Unset = 0x0000,
        /// <summary>T=0 active protocol.</summary>
        [Description("T=0 active protocol")]
        T0 = 0x0001,
        /// <summary>T=1 active protocol.</summary>
        [Description("T=1 active protocol")]
        T1 = 0x0002,
        /// <summary>Raw active protocol. Use with memory type cards.</summary>
        [Description("Raw active protocol")]
        Raw = 0x0004,
        /// <summary>T=15 protocol.</summary>
        [Description("T=15 protocol")]
        T15 = 0x0008,

        /// <summary>(<see cref="F:PCSC.SCardProtocol.T0" /> | <see cref="F:PCSC.SCardProtocol.T1" />). IFD (Interface device) determines protocol.</summary>
        [Description("IFD determines protocol")]
        Any = (T0 | T1)
    }
}