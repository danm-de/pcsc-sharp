using System.ComponentModel;

namespace PCSC
{
    /// <summary>Mode of connection type: exclusive or shared.</summary>
    /// <remarks>
    ///     <see cref="F:PCSC.SCardShareMode.Direct" /> can be used before using <see cref="M:PCSC.ISCardReader.Control(System.IntPtr,System.Byte[],System.Byte[]@)" />  to send control commands to the reader even if a card is not present in the reader. Contrary to Windows winscard behavior, the reader is accessed in shared mode and not exclusive mode. </remarks>
    public enum SCardShareMode
    {
        /// <summary>This application will NOT allow others to share the reader. (SCARD_SHARE_EXCLUSIVE)</summary>
        [Description("Exclusive mode only")]
        Exclusive = 0x0001,
        /// <summary>This application will allow others to share the reader. (SCARD_SHARE_SHARED)</summary>
        [Description("Shared mode only")]
        Shared = 0x0002,
        /// <summary>Direct control of the reader, even without a card. (SCARD_SHARE_DIRECT)</summary>
        [Description("Raw mode only")]
        Direct = 0x0003
    }
}