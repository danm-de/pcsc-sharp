namespace PCSC
{
    public enum SCardAttr
    {
        VendorName = (SCardClass.VendorInfo << 16) | 0x0100,
        VendorIFDType = (SCardClass.VendorInfo << 16) | 0x0101,
        VendorIFDVersion = (SCardClass.VendorInfo << 16) | 0x0102,
        VendorIFDSerialNo = (SCardClass.VendorInfo << 16) | 0x0103,
        ChannelId = (SCardClass.Communications << 16) | 0x0110,
        AsyncProtocolTypes = (SCardClass.Protocol << 16) | 0x0120,
        DefaultClk = (SCardClass.Protocol << 16) | 0x0121,
        MaxClk = (SCardClass.Protocol << 16) | 0x0122,
        DefaultDataRate = (SCardClass.Protocol << 16) | 0x0123,
        MaxDataRate = (SCardClass.Protocol << 16) | 0x0124,
        MaxIfsd = (SCardClass.Protocol << 16) | 0x0125,
        SyncProtocolTypes = (SCardClass.Protocol << 16) | 0x0126,
        PowerMgmtSupport = (SCardClass.PowerMgmt << 16) | 0x0131,
        UserToCardAuthDevice = (SCardClass.Security << 16) | 0x0140,
        UserAuthInputDevice = (SCardClass.Security << 16) | 0x0142,
        Characteristics = (SCardClass.Mechanical << 16) | 0x0150,

        CurrentProtocolType = (SCardClass.IFDProtocol << 16) | 0x0201,
        CurrentClk = (SCardClass.IFDProtocol << 16) | 0x0202,
        CurrentF = (SCardClass.IFDProtocol << 16) | 0x0203,
        CurrentD = (SCardClass.IFDProtocol << 16) | 0x0204,
        CurrentN = (SCardClass.IFDProtocol << 16) | 0x0205,
        CurrentW = (SCardClass.IFDProtocol << 16) | 0x0206,
        CurrentIfsc = (SCardClass.IFDProtocol << 16) | 0x0207,
        CurrentIfsd = (SCardClass.IFDProtocol << 16) | 0x0208,
        CurrentBwt = (SCardClass.IFDProtocol << 16) | 0x0209,
        CurrentCwt = (SCardClass.IFDProtocol << 16) | 0x020a,
        CurrentEbcEncoding = (SCardClass.IFDProtocol << 16) | 0x020b,
        ExtendedBwt = (SCardClass.IFDProtocol << 16) | 0x020c,

        ICCPresence = (SCardClass.ICCState << 16) | 0x0300,
        ICCInterfaceStatus = (SCardClass.ICCState << 16) | 0x0301,
        CurrentIOState = (SCardClass.ICCState << 16) | 0x0302,
        AtrString = (SCardClass.ICCState << 16) | 0x0303,
        ICCTypePerAtr = (SCardClass.ICCState << 16) | 0x0304,

        EscReset = (SCardClass.VendorDefined << 16) | 0xA000,
        EscCancel = (SCardClass.VendorDefined << 16) | 0xA003,
        EscAuthRequest = (SCardClass.VendorDefined << 16) | 0xA005,
        MaxInput = (SCardClass.VendorDefined << 16) | 0xA007,

        DeviceUnit = (SCardClass.System << 16) | 0x0001,
        DeviceInUse = (SCardClass.System << 16) | 0x0002,
        DeviceFriendlyNameA = (SCardClass.System << 16) | 0x0003,
        DeviceSystemNameA = (SCardClass.System << 16) | 0x0004,
        DeviceFriendlyNameW = (SCardClass.System << 16) | 0x0005,
        DeviceSystemNameW = (SCardClass.System << 16) | 0x0006,
        SupressT1IFSRequest = (SCardClass.System << 16) | 0x0007,

        DeviceFriendlyName = DeviceFriendlyNameW,
        DeviceSystemName = DeviceSystemNameW

        /* ASCII *
        DEVICE_FRIENDLY_NAME     = DEVICE_FRIENDLY_NAME_A,
        DEVICE_SYSTEM_NAME       = DEVICE_SYSTEM_NAME_A
        */
    }
}