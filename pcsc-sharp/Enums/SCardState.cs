using System;
using System.ComponentModel;

namespace PCSC
{
    /// <summary>State of the smart card in the reader.</summary>
    public enum SCardState
    {
        /// <summary>Unknown status.</summary>
        [Description("Unknown state")]
        Unknown = 0x00,
        /// <summary>There is no card in the reader.</summary>
        [Description("Card is absent")]
        Absent = 0x01,
        /// <summary>There is a card in the reader, but it has not been moved into position for use.</summary>
        [Description("Card is present")]
        Present = 0x02,
        /// <summary>There is a card in the reader in position for use. The card is not powered.</summary>
        [Description("Card not powered")]
        Swallowed = 0x03,
        /// <summary>Power is being provided to the card, but the reader driver is unaware of the mode of the card.</summary>
        [Description("Card is powered")]
        Powered = 0x04,
        /// <summary>The card has been reset and is awaiting PTS negotiation.</summary>
        [Description("Ready for PTS")]
        Negotiable = 0x05,
        /// <summary>The card has been reset and specific communication protocols have been established.</summary>
        [Description("PTS has been set")]
        Specific = 0x06
    }
}