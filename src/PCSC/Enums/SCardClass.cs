using System.ComponentModel;

namespace PCSC
{
    /// <summary>
    /// Smart card class
    /// </summary>
    public enum SCardClass
    {
        /// <summary>Vendor information definitions</summary>
        [Description("Vendor information definitions")]
        VendorInfo = 1,
        /// <summary>Communication definitions</summary>
        [Description("Communication definitions")]
        Communication = 2,
        /// <summary>Protocol definitions</summary>
        [Description("Protocol definitions")]
        Protocol = 3,
        /// <summary>Power Management definitions</summary>
        [Description("Power Management definitions")]
        PowerManagement = 4,
        /// <summary>Security Assurance definitions</summary>
        [Description("Security Assurance definitions")]
        Security = 5,
        /// <summary>Mechanical characteristic definitions</summary>
        [Description("Mechanical characteristic definitions")]
        Mechanical = 6,
        /// <summary>Vendor specific definitions</summary>
        [Description("Vendor specific definitions")]
        VendorDefined = 7,
        /// <summary>Interface Device Protocol options</summary>
        [Description("Interface Device Protocol options")]
        InterfaceDeviceProtocol = 8,
        /// <summary>ICC State specific definitions</summary>
        [Description("ICC State specific definitions")]
        ICCState = 9,
        /// <summary>System-specific definitions</summary>
        [Description("System-specific definitions")]
        System = 0x7fff
    }
}