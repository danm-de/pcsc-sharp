using System.ComponentModel;

namespace PCSC
{
    public enum SCardScope
    {
        [Description("Scope in user space")]
        User = 0x0000,
        [Description("Scope in terminal")]
        Terminal = 0x0001,
        [Description("Scope in system")]
        System = 0x0002,

        /** PC/SC Lite specific extensions */
        [Description("Scope is global")]
        Global = 0x0003
    }
}