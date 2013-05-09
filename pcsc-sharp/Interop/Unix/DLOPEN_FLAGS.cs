using System;
using System.ComponentModel;

namespace PCSC.Interop.Unix
{
    [Flags]
    internal enum DLOPEN_FLAGS
    {
        /* The MODE argument to `dlopen' contains one of the following: */
        [Description("Lazy function call binding")]
        RTLD_LAZY = 0x00001,
        [Description("Immediate function call binding")]
        RTLD_NOW = 0x00002,

        [Description("Mask of binding time value")]
        RTLD_BINDING_MASK = 0x3,
        [Description("Do not load the object.")]
        RTLD_NOLOAD = 0x00004,
        [Description("Use deep binding")]
        RTLD_DEEPBIND = 0x00008,
        [Description("The symbols defined by this library will be made available for symbol resolution of subsequently loaded libraries")]
        RTLD_GLOBAL = 0x00100,
        [Description("The converse of RTLD_GLOBAL")]
        RTLD_LOCAL = 0x0,
        [Description("Do not delete object when closed")]
        RTLD_NODELETE = 0x01000
    }
}