using System;
using System.ComponentModel;

namespace PCSC
{
    /// <summary>Reader state.</summary>
    [Flags]
    public enum SCRState
    {
        /// <summary>The application is unaware of the current state, and would like to know. The use of this value results in an immediate return from state transition monitoring services. This is represented by all bits set to zero. (SCARD_STATE_UNAWARE)</summary>
        [Description("Application wants status")]
        Unaware = 0x0000,
        /// <summary>This reader should be ignored. (SCARD_STATE_IGNORE)</summary>
        [Description("Ignore this reader")]
        Ignore = 0x0001,
        /// <summary>There is a difference between the state believed by the application, and the state known by the resource manager. When this bit is set, the application may assume a significant state change has occurred on this reader. (SCARD_STATE_CHANGED)</summary>
        [Description("State has changed")]
        Changed = 0x0002,
        /// <summary>The given reader name is not recognized by the resource manager. If this bit is set, then <see cref="F:PCSC.SCRState.Changed" />  and <see cref="F:PCSC.SCRState.Ignore" /> will also be set. (SCARD_STATE_UNKNOWN)</summary>
        [Description("Reader unknown")]
        Unknown = 0x0004,

        /// <summary>The actual state of this reader is not available. If this bit is set, then all the following bits are clear. (SCARD_STATE_UNAVAILABLE)</summary>
        [Description("Status unavailable")]
        Unavailable = 0x0008,
        /// <summary>There is no card in the reader. If this bit is set, all the following bits will be clear. (SCARD_STATE_EMPTY)</summary>
        [Description("Card removed")]
        Empty = 0x0010,
        /// <summary>There is a card in the reader. (SCARD_STATE_PRESENT)</summary>
        [Description("Card inserted")]
        Present = 0x0020,
        /// <summary>There is a card in the reader with an ATR matching one of the target cards. If this bit is set, <see cref="F:PCSC.SCRState.Present" /> will also be set. This bit is only returned on the SCardLocateCards() function. (SCARD_STATE_ATRMATCH)</summary>
        [Description("ATR matches card")]
        AtrMatch = 0x0040,
        /// <summary>The card in the reader is allocated for exclusive use by another application. If this bit is set, <see cref="F:PCSC.SCRState.Present" /> will also be set. (SCARD_STATE_EXCLUSIVE)</summary>
        [Description("Exclusive Mode")]
        Exclusive = 0x0080,
        /// <summary>The card in the reader is in use by one or more other applications, but may be connected to in shared mode. If this bit is set, <see cref="F:PCSC.SCRState.Present" />  will also be set. (SCARD_STATE_INUSE)</summary>
        [Description("In use")]
        InUse = 0x0100,
        /// <summary>There is an unresponsive card in the reader. (SCARD_STATE_MUTE)</summary>
        [Description("Unresponsive card")]
        Mute = 0x0200,
        /// <summary>The card is unpowered. (SCARD_STATE_UNPOWERED)</summary>
        [Description("Unpowered card")]
        Unpowered = 0x0400
    }
}