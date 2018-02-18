using System.ComponentModel;

namespace PCSC
{
    /// <summary>Action to be taken on the reader.</summary>
    /// <remarks>The disposition action is not currently used in PC/SC Lite.</remarks>
    public enum SCardReaderDisposition
    {

        /// <summary>Do nothing. (SCARD_LEAVE_CARD)</summary>
        [Description("Do nothing")]
        Leave = 0x0000,
        /// <summary>Reset the card. (SCARD_RESET_CARD)</summary>
        [Description("Reset the reader")]
        Reset = 0x0001,
        /// <summary>Unpower the card. (SCARD_UNPOWER_CARD)</summary>
        [Description("Unpower the card")]
        Unpower = 0x0002,
        /// <summary>Eject the card. (SCARD_EJECT_CARD)</summary>
        [Description("Eject the card")]
        Eject = 0x0003
    }
}