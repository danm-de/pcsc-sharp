namespace PCSC.Iso7816
{
    public enum SW1Code: byte
    {
        NormalDataResponse          = 0x61,
        WarningNVDataNotChanged     = 0x62,
        WarningNVDataChanged        = 0x63,
        ErrorNVDataNotChanged       = 0x64,
        ErrorNVDataChanged          = 0x65,
        ErrorSecurity               = 0x66,
        ErrorLengthIncorrect        = 0x67,
        ErrorFunctionNotSupported   = 0x68,
        ErrorCmdNotAllowed          = 0x69,
        ErrorP1P2Incorrect          = 0x6A,
        ErrorParameterIncorrect     = 0x6B,
        ErrorP3Incorrect            = 0x6C,
        ErrorInsNotSupported        = 0x6D,
        ErrorClassNotSupported      = 0x6E,
        ErrorNoPreciseDiagnostic    = 0x6F,
        
        Normal                      = 0x90,

        ErrorPurseBalance           = 0x91,
        ErrorMemory                 = 0x92,
        ErrorFile                   = 0x94,
        ErrorAuthorization          = 0x98,
        NormalGetResponse           = 0x9F
    }
}