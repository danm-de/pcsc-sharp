using System.ComponentModel;

namespace PCSC
{
    /// <summary>Scope of the establishment.</summary>
    /// <remarks>The following scopes are not used on Linux/UNIX machines using the PC/SC Lite daemon:
    ///     <list type="bullet">
    ///         <item><term><see cref="F:PCSC.SCardScope.User" /></term></item>
    ///         <item><term><see cref="F:PCSC.SCardScope.Terminal" /></term></item>
    ///         <item><term><see cref="F:PCSC.SCardScope.Global" /></term></item>
    ///     </list></remarks>
    public enum SCardScope
    {
        /// <summary>Scope in user space.</summary>
        [Description("Scope in user space")]
        User = 0x0000,
        /// <summary>Scope in terminal.</summary>
        [Description("Scope in terminal")]
        Terminal = 0x0001,
        /// <summary>Scope in system. Service on the local machine.</summary>
        [Description("Scope in system")]
        System = 0x0002,

        /** PC/SC Lite specific extensions */
        /// <summary>Scope is global. </summary>
        [Description("Scope is global")]
        Global = 0x0003
    }
}