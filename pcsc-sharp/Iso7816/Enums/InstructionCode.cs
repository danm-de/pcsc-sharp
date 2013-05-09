using System.ComponentModel;

namespace PCSC.Iso7816
{
    public enum InstructionCode: byte
    {
        [Description("ERASE BINARY")]
        EraseBinary = 0x0E,
        [Description("VERIFY")]
        Verify = 0x20,
        [Description("MANAGE CHANNEL")]
        ManageChannel = 0x70,
        [Description("EXTERNAL AUTHENTICATE")]
        ExternalAuthenticate = 0x82,
        [Description("GET CHALLENGE")]
        GetChallenge = 0x84,
        [Description("INTERNAL AUTHENTICATE")]
        InternalAuthenticate = 0x88,
        [Description("SELECT FILE")]
        SelectFile = 0xA4,
        [Description("READ BINARY")]
        ReadBinary = 0xB0,
        [Description("READ RECORD(S)")]
        ReadRecord = 0xB2,
        [Description("GET RESPONSE")]
        GetResponse = 0xC0,
        [Description("ENVELOPE")]
        Envelope = 0xC2,
        [Description("GET DATA")]
        GetData = 0xCA,
        [Description("WRITE BINARY")]
        WriteBinary = 0xD0,
        [Description("WRITE RECORD")]
        WriteRecord = 0xD2,
        [Description("UPDATE BINARY")]
        UpdateBinary = 0xD6,
        [Description("PUT DATA")]
        PutData = 0xDA,
        [Description("UPDATE DATA")]
        UpdateData = 0xDC,
        [Description("APPEND RECORD")]
        AppendRecord = 0xE2
    }
}