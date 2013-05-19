using System;
using System.ComponentModel;

namespace PCSC
{
    /// <summary>State of the smart card in the reader.</summary>
    /// <remarks>Is a bit mask that represents the current reader status.</remarks>
    [Flags]
    public enum SCardState
    {
        /// <summary>Unknown status.</summary>
        [Description("Unknown state")]
        Unknown = 0x0001,
        /// <summary>There is no card in the reader.</summary>
        [Description("Card is absent")]
        Absent = 0x0002,
        /// <summary>There is a card in the reader, but it has not been moved into position for use.</summary>
        [Description("Card is present")]
        Present = 0x0004,
        /// <summary>There is a card in the reader in position for use. The card is not powered.</summary>
        [Description("Card not powered")]
        Swallowed = 0x0008,
        /// <summary>Power is being provided to the card, but the reader driver is unaware of the mode of the card.</summary>
        [Description("Card is powered")]
        Powered = 0x0010,
        /// <summary>The card has been reset and is awaiting PTS negotiation.</summary>
        [Description("Ready for PTS")]
        Negotiable = 0x0020,
        /// <summary>The card has been reset and specific communication protocols have been established.</summary>
        [Description("PTS has been set")]
        Specific = 0x0040
    }
}