namespace PCSC.Iso7816
{
    /// <summary>
    /// Coding and meaning of the class byte
    /// </summary>
    public enum ClaHighPart : byte
    {
        /// <summary>The structure and coding of command and response are as described in ISO/IEC 7816. 'x' contains the options for secure messaging <see cref="SecureMessagingFormat"/> and the logical channel.</summary>
        Iso0x           = 0x00,
        /// <summary>Reserved for future use.</summary>
        Rfu1x           = 0x10,
        /// <summary>Reserved for future use.</summary>
        Rfu2x           = 0x20,
        /// <summary>Reserved for future use.</summary>
        Rfu3x           = 0x30,
        /// <summary>Reserved for future use.</summary>
        Rfu4x           = 0x40,
        /// <summary>Reserved for future use.</summary>
        Rfu5x           = 0x50,
        /// <summary>Reserved for future use.</summary>
        Rfu6x           = 0x60,
        /// <summary>Reserved for future use.</summary>
        Rfu7x           = 0x70,
        /// <summary>The structure of command and response are as described in ISO/IEC 7816. 'x' contains the options for secure messaging <see cref="SecureMessagingFormat"/> and the logical channel.
        /// The coding (and meaning) of command and response are proprietary.</summary>
        Iso8x           = 0x80,
        /// <summary>The structure of command and response are as described in ISO/IEC 7816. 'x' contains the options for secure messaging <see cref="SecureMessagingFormat"/> and the logical channel.
        /// The coding (and meaning) of command and response are proprietary.</summary>
        Iso9x           = 0x90,
        /// <summary>If not specified by the application context, structure and coding of command and response are as described in ISO/IEC 7816. 'x' contains the options for secure messaging <see cref="SecureMessagingFormat"/> and the logical channel.</summary>
        IsoAx           = 0xA0,
        /// <summary>The structure of command and response are as described in ISO/IEC 7816.</summary>
        IsoBx           = 0xB0,
        /// <summary>The structure of command and response are as described in ISO/IEC 7816.</summary>
        IsoCx           = 0xC0,
        /// <summary>The structure of command and response are proprietary.</summary>
        ProprietaryDx   = 0xD0,
        /// <summary>The structure of command and response are proprietary.</summary>
        ProprietaryEx   = 0xE0,
        /// <summary>The structure of command and response are proprietary.</summary>
        ProprietaryFx   = 0xF0
    }
}