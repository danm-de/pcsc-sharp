namespace PCSC.Iso7816
{
    public enum SecureMessagingFormat : byte
    { 
        None                        = 0x0,
        Proprietary                 = 0x4,
        CmdHeaderNotAuthenticated   = 0x8,
        CmdHeaderAuthenticated      = 0xC
    }
}