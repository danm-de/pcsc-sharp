namespace PCSC
{
    ///  <summary>Smart card reader attribute enumeration.</summary>
    /// <remarks>Can be used as parameter for the following methods:
    /// <list type="bullet">
    /// <item><term><see cref="M:PCSC.SCardReader.GetAttrib(PCSC.SCardAttribute,System.Byte[]@)" /></term></item>
    /// <item><term><see cref="M:PCSC.SCardReader.GetAttrib(PCSC.SCardAttribute,System.Byte[],System.Int32@)" /></term></item>
    /// <item><term><see cref="M:PCSC.SCardReader.SetAttrib(PCSC.SCardAttribute,System.Byte[])" /></term></item>
    /// <item><term><see cref="M:PCSC.SCardReader.SetAttrib(PCSC.SCardAttribute,System.Byte[],System.Int32)" /></term></item>
    /// </list></remarks>
    public enum SCardAttribute
    {
        /// <summary>
        /// Vendor name. (SCARD_ATTR_VENDOR_NAME)
        /// </summary>
        VendorName = (SCardClass.VendorInfo << 16) | 0x0100,
        /// <summary>
        /// Vendor-supplied interface device type (model designation of reader). (SCARD_ATTR_VENDOR_IFD_TYPE)
        /// </summary>
        VendorInterfaceDeviceType = (SCardClass.VendorInfo << 16) | 0x0101,
        /// <summary>
        /// Vendor-supplied interface device version (DWORD in the form 0xMMmmbbbb where MM = major version, mm = minor version, and bbbb = build number).  (SCARD_ATTR_VENDOR_IFD_VERSION)
        /// </summary>
        VendorInterfaceDeviceTypeVersion = (SCardClass.VendorInfo << 16) | 0x0102,
        /// <summary>
        /// Vendor-supplied interface device serial number. (SCARD_ATTR_VENDOR_IFD_SERIAL_NO)
        /// </summary>
        VendorInterfaceDeviceTypeSerialNumber = (SCardClass.VendorInfo << 16) | 0x0103,
        /// <summary>
        /// DWORD encoded as 0xDDDDCCCC, where DDDD = data channel type and CCCC = channel number (SCARD_ATTR_CHANNEL_ID)
        /// </summary>
        ChannelId = (SCardClass.Communication << 16) | 0x0110,
        /// <summary>
        /// Asynchronous protocol types (SCARD_ATTR_ASYNC_PROTOCOL_TYPES)
        /// </summary>
        AsyncProtocolTypes = (SCardClass.Protocol << 16) | 0x0120,
        /// <summary>
        /// Default clock rate, in kHz. (SCARD_ATTR_DEFAULT_CLK)
        /// </summary>
        DefaultClockRate = (SCardClass.Protocol << 16) | 0x0121,
        /// <summary>
        /// Maximum clock rate, in kHz. (SCARD_ATTR_MAX_CLK)
        /// </summary>
        MaxClockRate = (SCardClass.Protocol << 16) | 0x0122,
        /// <summary>
        /// Default data rate, in bps. (SCARD_ATTR_DEFAULT_DATA_RATE)
        /// </summary>
        DefaultDataRate = (SCardClass.Protocol << 16) | 0x0123,
        /// <summary>
        /// Maximum data rate, in bps. (SCARD_ATTR_MAX_DATA_RATE)
        /// </summary>
        MaxDataRate = (SCardClass.Protocol << 16) | 0x0124,
        /// <summary>
        /// Maximum bytes for information file size device. (SCARD_ATTR_MAX_IFSD)
        /// </summary>
        MaxInformationFileSizeDevice = (SCardClass.Protocol << 16) | 0x0125,
        /// <summary>
        /// Synchronous protocol types (SCARD_ATTR_SYNC_PROTOCOL_TYPES)
        /// </summary>
        SyncProtocolTypes = (SCardClass.Protocol << 16) | 0x0126,
        /// <summary>
        /// Zero if device does not support power down while smart card is inserted. Nonzero otherwise. (SCARD_ATTR_POWER_MGMT_SUPPORT)
        /// </summary>
        PowerManagementSupport = (SCardClass.PowerManagement << 16) | 0x0131,
        /// <summary>
        /// User to card authentication device (SCARD_ATTR_USER_TO_CARD_AUTH_DEVICE)
        /// </summary>
        UserToCardAuthDevice = (SCardClass.Security << 16) | 0x0140,
        /// <summary>
        /// User authentication input device (SCARD_ATTR_USER_AUTH_INPUT_DEVICE)
        /// </summary>
        UserAuthInputDevice = (SCardClass.Security << 16) | 0x0142,
        /// <summary>
        /// DWORD indicating which mechanical characteristics are supported. If zero, no special characteristics are supported. Note that multiple bits can be set (SCARD_ATTR_CHARACTERISTICS)
        /// </summary>
        Characteristics = (SCardClass.Mechanical << 16) | 0x0150,

        /// <summary>
        /// Current protocol type (SCARD_ATTR_CURRENT_PROTOCOL_TYPE)
        /// </summary>
        CurrentProtocolType = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0201,
        /// <summary>
        /// Current clock rate, in kHz. (SCARD_ATTR_CURRENT_CLK)
        /// </summary>
        CurrentClockRate = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0202,
        /// <summary>
        /// Clock conversion factor. (SCARD_ATTR_CURRENT_F)
        /// </summary>
        CurrentClockConversionFactor = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0203,
        /// <summary>
        /// Bit rate conversion factor. (SCARD_ATTR_CURRENT_D)
        /// </summary>
        CurrentBitRateConversionFactor = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0204,
        /// <summary>
        /// Current guard time. (SCARD_ATTR_CURRENT_N)
        /// </summary>
        CurrentGuardTime = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0205,
        /// <summary>
        /// Current work waiting time. (SCARD_ATTR_CURRENT_W)
        /// </summary>
        CurrentWaitingTime = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0206,
        /// <summary>
        /// Current byte size for information field size card. (SCARD_ATTR_CURRENT_IFSC)
        /// </summary>
        CurrentInformationFieldSizeCard = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0207,
        /// <summary>
        /// Current byte size for information field size device. (SCARD_ATTR_CURRENT_IFSD)
        /// </summary>
        CurrentInformationFieldSizeDevice = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0208,
        /// <summary>
        /// Current block waiting time. (SCARD_ATTR_CURRENT_BWT)
        /// </summary>
        CurrentBlockWaitingTime = (SCardClass.InterfaceDeviceProtocol << 16) | 0x0209,
        /// <summary>
        /// Current character waiting time. (SCARD_ATTR_CURRENT_CWT)
        /// </summary>
        CurrentCharacterWaitingTime = (SCardClass.InterfaceDeviceProtocol << 16) | 0x020a,
        /// <summary>
        /// Current error block control encoding. (SCARD_ATTR_CURRENT_EBC_ENCODING)
        /// </summary>
        CurrentErrorBlockControlEncoding = (SCardClass.InterfaceDeviceProtocol << 16) | 0x020b,
        /// <summary>
        /// Extended block wait time. (SCARD_ATTR_EXTENDED_BWT)
        /// </summary>
        ExtendedBlockWaitTime = (SCardClass.InterfaceDeviceProtocol << 16) | 0x020c,

        /// <summary>
        /// Single byte indicating smart card presence(SCARD_ATTR_ICC_PRESENCE)
        /// </summary>
        ICCPresence = (SCardClass.ICCState << 16) | 0x0300,
        /// <summary>
        /// Single byte. Zero if smart card electrical contact is not active; nonzero if contact is active. (SCARD_ATTR_ICC_INTERFACE_STATUS)
        /// </summary>
        ICCInterfaceStatus = (SCardClass.ICCState << 16) | 0x0301,
        /// <summary>
        /// Current IO state (SCARD_ATTR_CURRENT_IO_STATE)
        /// </summary>
        CurrentIOState = (SCardClass.ICCState << 16) | 0x0302,
        /// <summary>
        /// Answer to reset (ATR) string. (SCARD_ATTR_ATR_STRING)
        /// </summary>
        AtrString = (SCardClass.ICCState << 16) | 0x0303,
        /// <summary>
        /// Answer to reset (ATR) string. (SCARD_ATTR_ATR_STRING)
        /// </summary>
        AnswerToResetString = AtrString,
        /// <summary>
        /// Single byte indicating smart card type (SCARD_ATTR_ICC_TYPE_PER_ATR)
        /// </summary>
        ICCTypePerAtr = (SCardClass.ICCState << 16) | 0x0304,

        /// <summary>
        /// Esc reset (SCARD_ATTR_ESC_RESET)
        /// </summary>
        EscReset = (SCardClass.VendorDefined << 16) | 0xA000,
        /// <summary>
        /// Esc cancel (SCARD_ATTR_ESC_CANCEL)
        /// </summary>
        EscCancel = (SCardClass.VendorDefined << 16) | 0xA003,
        /// <summary>
        /// Esc authentication request (SCARD_ATTR_ESC_AUTHREQUEST)
        /// </summary>
        EscAuthRequest = (SCardClass.VendorDefined << 16) | 0xA005,
        /// <summary>
        /// Maximum input (SCARD_ATTR_MAXINPUT)
        /// </summary>
        MaxInput = (SCardClass.VendorDefined << 16) | 0xA007,

        /// <summary>
        /// Instance of this vendor's reader attached to the computer. The first instance will be device unit 0, the next will be unit 1 (if it is the same brand of reader) and so on. Two different brands of readers will both have zero for this value. (SCARD_ATTR_DEVICE_UNIT)
        /// </summary>
        DeviceUnit = (SCardClass.System << 16) | 0x0001,
        /// <summary>
        /// Reserved for future use. (SCARD_ATTR_DEVICE_IN_USE)
        /// </summary>
        DeviceInUse = (SCardClass.System << 16) | 0x0002,
        /// <summary>
        /// Device friendly name ASCII (SCARD_ATTR_DEVICE_FRIENDLY_NAME_A)
        /// </summary>
        DeviceFriendlyNameA = (SCardClass.System << 16) | 0x0003,
        /// <summary>
        /// Device system name ASCII (SCARD_ATTR_DEVICE_SYSTEM_NAME_A)
        /// </summary>
        DeviceSystemNameA = (SCardClass.System << 16) | 0x0004,
        /// <summary>
        /// Device friendly name UNICODE (SCARD_ATTR_DEVICE_FRIENDLY_NAME_W)
        /// </summary>
        DeviceFriendlyNameW = (SCardClass.System << 16) | 0x0005,
        /// <summary>
        /// Device system name UNICODE (SCARD_ATTR_DEVICE_SYSTEM_NAME_W)
        /// </summary>
        DeviceSystemNameW = (SCardClass.System << 16) | 0x0006,
        /// <summary>
        /// Supress T1 information file size request (SCARD_ATTR_SUPRESS_T1_IFS_REQUEST)
        /// </summary>
        SupressT1InformationFileSizeRequest = (SCardClass.System << 16) | 0x0007,

        /// <summary>
        /// Device friendly name (SCARD_ATTR_DEVICE_FRIENDLY_NAME)
        /// </summary>
        DeviceFriendlyName = DeviceFriendlyNameW,
        /// <summary>
        /// Device system name (SCARD_ATTR_DEVICE_SYSTEM_NAME)
        /// </summary>
        DeviceSystemName = DeviceSystemNameW
    }
}