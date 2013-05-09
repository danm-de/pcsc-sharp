namespace PCSC.Iso7816
{
    public enum ClaHighPart : byte
    {
        Iso0x           = 0x00,
        Rfu1x           = 0x10,
        Rfu2x           = 0x20,
        Rfu3x           = 0x30,
        Rfu4x           = 0x40,
        Rfu5x           = 0x50,
        Rfu6x           = 0x60,
        Rfu7x           = 0x70,
        Iso8x           = 0x80,
        Iso9x           = 0x90,
        IsoAx           = 0xA0,
        IsoBx           = 0xB0,
        IsoCx           = 0xC0,
        ProprietaryDx   = 0xD0,
        ProprietaryEx   = 0xE0,
        ProprietaryFx   = 0xF0
    }
}