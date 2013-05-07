using System.ComponentModel;

namespace PCSC
{
    public enum SCardClass
    {
        [Description("Vendor information definitions")]
        VendorInfo = 1,
        [Description("Communication definitions")]
        Communications = 2,
        [Description("Protocol definitions")]
        Protocol = 3,
        [Description("Power Management definitions")]
        PowerMgmt = 4,
        [Description("Security Assurance definitions")]
        Security = 5,
        [Description("Mechanical characteristic definitions")]
        Mechanical = 6,
        [Description("Vendor specific definitions")]
        VendorDefined = 7,
        [Description("Interface Device Protocol options")]
        IFDProtocol = 8,
        [Description("ICC State specific definitions")]
        ICCState = 9,
        [Description("System-specific definitions")]
        System = 0x7fff
    }
}