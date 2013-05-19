namespace PCSC.Iso7816
{
    /// <summary>Meaning for the first status word SW1 (ISO7816-4).</summary>
    public enum SW1Code : byte
    {
        /// <summary>Normal data response. SW2 indicates the number of remaining response bytes.</summary>
        NormalDataResponse = 0x61,

        /// <summary>Warning. The state of non-volatile memory has not been changed.</summary>
        /// <remarks>SW2 could have one of the following values:
        ///     <list type="table">
        ///         <listheader><term>SW2</term><description>Meaning</description></listheader>
        ///         <item><term>0x00</term><description>No information.</description></item>
        ///         <item><term>0x81</term><description>Returned data may be corrupted.</description></item>
        ///         <item><term>0x82</term><description>End of file or end of record reached. The number of bytes read is less than specified in <see cref="CommandApdu.Le" /></description></item>
        ///         <item><term>0x83</term><description>The selected file invalidated.</description></item>
        ///         <item><term>0x84</term><description>FCI is not correctly formatted.</description></item>
        ///     </list>
        /// </remarks>
        WarningNVDataNotChanged = 0x62,

        /// <summary>Warning. The state of non-volatile memory has been changed.</summary>
        /// <remarks>SW2 could have one of the following values:
        ///     <list type="table">
        ///         <listheader><term>SW2</term><description>Meaning</description></listheader>
        ///         <item><term>0x00</term><description>No information.</description></item>
        ///         <item><term>0x81</term><description>File filled up.</description></item>
        ///     </list>
        /// </remarks>
        WarningNVDataChanged = 0x63,

        /// <summary>An error occurred. The state of non-volatile memory has not been changed.</summary>
        /// <remarks>SW2 should be 0x00.</remarks>
        ErrorNVDataNotChanged = 0x64,

        /// <summary>An error occurred. The state of non-volatile memory has been changed.</summary>
        /// <remarks>SW2 could have one of the following values:
        ///     <list type="table">
        ///         <listheader><term>SW2</term><description>Meaning</description></listheader>
        ///         <item><term>0x00</term><description>No information.</description></item>
        ///         <item><term>0x81</term><description>Memory failure.</description></item>
        ///     </list>
        /// </remarks>
        ErrorNVDataChanged = 0x65,

        /// <summary>An security error occurred.</summary>
        ErrorSecurity = 0x66,

        /// <summary>Error wrong length.</summary>
        ErrorLengthIncorrect = 0x67,

        /// <summary>The function defined in CLA is not supported.</summary>
        /// <remarks>SW2 could have one of the following values:
        ///     <list type="table">
        ///         <listheader><term>SW2</term><description>Meaning</description></listheader>
        ///         <item><term>0x00</term><description>No information.</description></item>
        ///         <item><term>0x81</term><description>Logical channel not supported</description></item>
        ///         <item><term>0x82</term><description>Secure messaging not supported</description></item>
        ///     </list>
        /// </remarks>
        ErrorFunctionNotSupported = 0x68,

        /// <summary>Error command not allowed</summary>
        /// <remarks>SW2 could have one of the following values:
        ///     <list type="table">
        ///         <listheader><term>SW2</term><description>Meaning</description></listheader>
        ///         <item><term>0x00</term><description>No information.</description></item>
        ///         <item><term>0x81</term><description>The command is not compatible with the file structure.</description></item>
        ///         <item><term>0x82</term><description>The security status is not satisfied.</description></item>
        ///         <item><term>0x83</term><description>The authentication method is blocked.</description></item>
        ///         <item><term>0x84</term><description>The referenced data has been invalidated.</description></item>
        ///         <item><term>0x85</term><description>The conditions are not satisfied.</description></item>
        ///         <item><term>0x86</term><description>The command is not allowed. No current elementary file (EF).</description></item>
        ///         <item><term>0x87</term><description>Expected secure messaging data objects are missing.</description></item>
        ///         <item><term>0x88</term><description>Secure messaging data objects are incorrect.</description></item>
        ///     </list>
        /// </remarks>
        ErrorCmdNotAllowed = 0x69,

        /// <summary>Error wrong parameters P1 and P2</summary>
        /// <remarks>SW2 could have one of the following values:
        ///     <list type="table">
        ///         <listheader><term>SW2</term><description>Meaning</description></listheader>
        ///         <item><term>0x00</term><description>No information.</description></item>
        ///         <item><term>0x80</term><description>The parameters in the data field are incorrect.</description></item>
        ///         <item><term>0x81</term><description>The requested function is not supported.</description></item>
        ///         <item><term>0x82</term><description>File not found.</description></item>
        ///         <item><term>0x83</term><description>Record not found.</description></item>
        ///         <item><term>0x84</term><description>The file has not enough free space.</description></item>
        ///         <item><term>0x85</term><description>The <see cref="CommandApdu.Lc"/> parameter is inconsistent with the TLV structure.</description></item>
        ///         <item><term>0x86</term><description>The parameters <see cref="CommandApdu.P1"/> and <see cref="CommandApdu.P2"/> are incorrect.</description></item>
        ///         <item><term>0x87</term><description><see cref="CommandApdu.Lc"/> is not consistent with <see cref="CommandApdu.P1P2"/>.</description></item>
        ///         <item><term>0x88</term><description></description>The referenced data was not found.</item>
        ///     </list>
        /// </remarks>
        ErrorP1P2Incorrect = 0x6A,
        
        /// <summary>Error wrong parameters P1 and P2</summary>
        ErrorParameterIncorrect = 0x6B,
        
        /// <summary><see cref="CommandApdu.Le"/> has the wrong length.</summary>
        /// <remarks>SW2 contains the exact length.</remarks>
        ErrorP3Incorrect = 0x6C,
        
        /// <summary>The instruction code is invalid or not supported.</summary>
        ErrorInsNotSupported = 0x6D,
        
        /// <summary>The Class is not supported.</summary>
        ErrorClassNotSupported = 0x6E,
        
        /// <summary>No precise diagnosis available.</summary>
        ErrorNoPreciseDiagnostic = 0x6F,

        /// <summary>Normal data response.</summary>
        Normal = 0x90,

        /// <summary>Error purse balance.</summary>
        ErrorPurseBalance = 0x91,

        /// <summary>Memory error.</summary>
        ErrorMemory = 0x92,
        
        /// <summary>File error.</summary>
        ErrorFile = 0x94,

        /// <summary>Error authorization.</summary>
        ErrorAuthorization = 0x98,
        
        /// <summary>Normal GET response</summary>
        NormalGetResponse = 0x9F
    }
}