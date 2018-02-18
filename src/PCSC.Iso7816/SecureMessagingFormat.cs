namespace PCSC.Iso7816
{
    /// <summary>
    /// Secure messaging (SM) format
    /// </summary>
    public enum SecureMessagingFormat : byte
    { 
        /// <summary>No secure messaging</summary>
        None                            = 0x0,
        /// <summary>Proprietary secure messaging format</summary>
        Proprietary                     = 0x4,
        /// <summary>Command header not authenticated</summary>
        CommandHeaderNotAuthenticated   = 0x8,
        /// <summary>Command header authenticated</summary>
        CommandHeaderAuthenticated      = 0xC
    }
}