using System;
using System.ComponentModel;

namespace PCSC
{
    /// <summary>Error and return codes.</summary>
    [Flags]
    public enum SCardError
    {
        /// <summary>No error. (SCARD_S_SUCCESS)</summary>
        [Description("No error.")]
        Success = 0x00000000,
        /// <summary>An internal consistency check failed. (SCARD_F_INTERNAL_ERROR)</summary>
        [Description("An internal consistency check failed.")]
        InternalError = unchecked((int) 0x80100001),

        /// <summary>The action was cancelled by an <see cref="ISCardContext.Cancel()" /> request. (SCARD_E_CANCELLED)</summary>
        [Description("The action was cancelled by an SCardCancel request.")]
        Cancelled = unchecked((int) 0x80100002),
        /// <summary>The supplied handle was invalid. (SCARD_E_INVALID_HANDLE)</summary>
        [Description("The supplied handle was invalid. ")]
        InvalidHandle = unchecked((int) 0x80100003),
        /// <summary>One or more of the supplied parameters could not be properly interpreted. (SCARD_E_INVALID_PARAMETER)</summary>
        [Description("One or more of the supplied parameters could not be properly interpreted.")]
        InvalidParameter = unchecked((int) 0x80100004),
        /// <summary>Registry startup information is missing or invalid. (SCARD_E_INVALID_TARGET)</summary>
        [Description("Registry startup information is missing or invalid.")]
        InvalidTarget = unchecked((int) 0x80100005),
        /// <summary>Not enough memory available to complete this command. (SCARD_E_NO_MEMORY)</summary>
        [Description("Not enough memory available to complete this command.")]
        NoMemory = unchecked((int) 0x80100006),
        /// <summary>An internal consistency timer has expired. (SCARD_F_WAITED_TOO_LONG)</summary>
        [Description("An internal consistency timer has expired.")]
        WaitedTooLong = unchecked((int) 0x80100007),
        /// <summary>The data buffer to receive returned data is too small for the returned data. (SCARD_E_INSUFFICIENT_BUFFER)</summary>
        [Description("The data buffer to receive returned data is too small for the returned data.")]
        InsufficientBuffer = unchecked((int) 0x80100008),
        /// <summary>Windows error ERROR_INSUFFICIENT_BUFFER: The data area passed to a system call is too small.</summary>
        [Description("The data area passed to a system call is too small.")]
        WinErrorInsufficientBuffer = 122,
        /// <summary>The specified reader name is not recognized. (SCARD_E_UNKNOWN_READER)</summary>
        [Description("The specified reader name is not recognized.")]
        UnknownReader = unchecked((int) 0x80100009),
        /// <summary>The user-specified timeout value has expired. (SCARD_E_TIMEOUT)</summary>
        [Description("The user-specified timeout value has expired.")]
        Timeout = unchecked((int) 0x8010000A),
        /// <summary>The smart card cannot be accessed because of other connections outstanding. (SCARD_E_SHARING_VIOLATION)</summary>
        [Description("The smart card cannot be accessed because of other connections outstanding.")]
        SharingViolation = unchecked((int) 0x8010000B),
        /// <summary>The operation requires a Smart Card, but no Smart Card is currently in the device. (SCARD_E_NO_SMARTCARD)</summary>
        [Description("The operation requires a Smart Card, but no Smart Card is currently in the device.")]
        NoSmartcard = unchecked((int) 0x8010000C),
        /// <summary>The specified smart card name is not recognized. (SCARD_E_UNKNOWN_CARD)</summary>
        [Description("The specified smart card name is not recognized.")]
        UnknownCard = unchecked((int) 0x8010000D),

        /// <summary>The system could not dispose of the media in the requested manner. (SCARD_E_CANT_DISPOSE)</summary>
        [Description("The system could not dispose of the media in the requested manner.")]
        CannotDispose = unchecked((int) 0x8010000E),
        /// <summary>The requested protocols are incompatible with the protocol currently in use with the smart card. (SCARD_E_PROTO_MISMATCH)</summary>
        [Description("The requested protocols are incompatible with the protocol currently in use with the smart card.")]
        ProtocolMismatch = unchecked((int) 0x8010000F),
        /// <summary>The reader or smart card is not ready to accept commands. (SCARD_E_NOT_READY)</summary>
        [Description("The reader or smart card is not ready to accept commands.")]
        NotReady = unchecked((int) 0x80100010),
        /// <summary>One or more of the supplied parameters values could not be properly interpreted. (SCARD_E_INVALID_VALUE)</summary>
        [Description("One or more of the supplied parameters values could not be properly interpreted.")]
        InvalidValue = unchecked((int) 0x80100011),
        /// <summary>The action was cancelled by the system, presumably to log off or shut down. (SCARD_E_SYSTEM_CANCELLED)</summary>
        [Description("The action was cancelled by the system, presumably to log off or shut down.")]
        SystemCancelled = unchecked((int) 0x80100012),
        /// <summary>An internal communications error has been detected. (SCARD_F_COMM_ERROR)</summary>
        [Description("An internal communications error has been detected.")]
        CommunicationError = unchecked((int) 0x80100013),
        /// <summary>An internal error has been detected, but the source is unknown. (SCARD_F_UNKNOWN_ERROR)</summary>
        [Description("An internal error has been detected, but the source is unknown.")]
        UnknownError = unchecked((int) 0x80100014),
        /// <summary>An ATR obtained from the registry is not a valid ATR string. (SCARD_E_INVALID_ATR)</summary>
        [Description("An ATR obtained from the registry is not a valid ATR string.")]
        InvalidAtr = unchecked((int) 0x80100015),
        /// <summary>An attempt was made to end a non-existent transaction. (SCARD_E_NOT_TRANSACTED)</summary>
        [Description("An attempt was made to end a non-existent transaction.")]
        NotTransacted = unchecked((int) 0x80100016),
        /// <summary>The specified reader is not currently available for use. (SCARD_E_READER_UNAVAILABLE)</summary>
        [Description("The specified reader is not currently available for use.")]
        ReaderUnavailable = unchecked((int) 0x80100017),
        /// <summary>The operation has been aborted to allow the server application to exit. (SCARD_P_SHUTDOWN)</summary>
        [Description("The operation has been aborted to allow the server application to exit.")]
        Shutdown = unchecked((int) 0x80100018),
        /// <summary>The reader cannot communicate with the card, due to ATR string configuration conflicts. (SCARD_W_UNSUPPORTED_CARD)</summary>
        [Description("The reader cannot communicate with the card, due to ATR string configuration conflicts.")]
        UnsupportedCard = unchecked((int) 0x80100065),
        /// <summary>The smart card is not responding to a reset. (SCARD_W_UNRESPONSIVE_CARD)</summary>
        [Description("The smart card is not responding to a reset.")]
        UnresponsiveCard = unchecked((int) 0x80100066),
        /// <summary>Power has been removed from the smart card, so that further communication is not possible. (SCARD_W_UNPOWERED_CARD)</summary>
        [Description("Power has been removed from the smart card, so that further communication is not possible.")]
        UnpoweredCard = unchecked((int) 0x80100067),
        /// <summary>The smart card has been reset, so any shared state information is invalid. (SCARD_W_RESET_CARD)</summary>
        [Description("The smart card has been reset, so any shared state information is invalid.")]
        ResetCard = unchecked((int) 0x80100068),
        /// <summary>The smart card has been removed, so further communication is not possible. (SCARD_W_REMOVED_CARD)</summary>
        [Description("The smart card has been removed, so further communication is not possible.")]
        RemovedCard = unchecked((int) 0x80100069),
        /// <summary>The PCI Receive buffer was too small. (SCARD_E_PCI_TOO_SMALL)</summary>
        [Description("The PCI Receive buffer was too small.")]
        PciTooSmall = unchecked((int) 0x80100019),
        /// <summary>The reader driver does not meet minimal requirements for support. (SCARD_E_READER_UNSUPPORTED)</summary>
        [Description("The reader driver does not meet minimal requirements for support.")]
        ReaderUnsupported = unchecked((int) 0x8010001A),

        /// <summary>The reader driver did not produce a unique reader name. (SCARD_E_DUPLICATE_READER)</summary>
        [Description("The reader driver did not produce a unique reader name.")]
        DuplicateReader = unchecked((int) 0x8010001B),
        /// <summary>The smart card does not meet minimal requirements for support. (SCARD_E_CARD_UNSUPPORTED)</summary>
        [Description("The smart card does not meet minimal requirements for support.")]
        CardUnsupported = unchecked((int) 0x8010001C),
        /// <summary>The Smart card resource manager is not running. (SCARD_E_NO_SERVICE)</summary>
        [Description("The Smart card resource manager is not running.")]
        NoService = unchecked((int) 0x8010001D),
        /// <summary>The Smart card resource manager has shut down. (SCARD_E_SERVICE_STOPPED)</summary>
        [Description("The Smart card resource manager has shut down.")]
        ServiceStopped = unchecked((int) 0x8010001E),
        /// <summary>An unexpected card error has occurred. (SCARD_E_UNEXPECTED)</summary>
        [Description("An unexpected card error has occurred.")]
        Unexpected = unchecked((int) 0x8010001F),
        /// <summary>Cannot find a smart card reader. (SCARD_E_NO_READERS_AVAILABLE)</summary>
        [Description("Cannot find a smart card reader.")]
        NoReadersAvailable = unchecked((int) 0x8010002E),

        /** PC/SC Lite specific extensions */
        /// <summary>The smart card has been inserted. (Obsolete)</summary>
        [Description("Card inserted.")]
        [Obsolete("PC/SC Lite specific, value conflicts with SCardError.SecurityViolation")]
        InsertedCard = unchecked((int) 0x8010006A),
        /// <summary>Feature not supported. (SCARD_E_UNSUPPORTED_FEATURE)</summary>
        [Description("Feature not supported")]
        UnsupportedFeature = unchecked((int) 0x8010001F),

        /// <summary>No primary provider can be found for the smart card. (SCARD_E_ICC_INSTALLATION)</summary>
        [Description("No primary provider can be found for the smart card.")]
        ICCInstallation = unchecked((int) 0x80100020),

        /// <summary>The requested order of object creation is not supported. (SCARD_E_ICC_CREATEORDER)</summary>
        [Description("The requested order of object creation is not supported.")]
        ICCCreateOrder = unchecked((int) 0x80100021),

        /// <summary>The identified directory does not exist in the smart card. (SCARD_E_DIR_NOT_FOUND)</summary>
        [Description("The identified directory does not exist in the smart card.")]
        DirectoryNotFound = unchecked((int) 0x80100023),

        /// <summary>The identified file does not exist in the smart card. (SCARD_E_FILE_NOT_FOUND)</summary>
        [Description("The identified file does not exist in the smart card.")]
        FileNotFound = unchecked((int) 0x80100024),
        /// <summary>The supplied path does not represent a smart card directory. (SCARD_E_NO_DIR)</summary>
        [Description("The supplied path does not represent a smart card directory. ")]
        NoDir = unchecked((int) 0x80100025),
        /// <summary>The supplied path does not represent a smart card file. (SCARD_E_NO_FILE)</summary>
        [Description("The supplied path does not represent a smart card file.")]
        NoFile = unchecked((int) 0x80100026),
        /// <summary>Access is denied to this file. (SCARD_E_NO_ACCESS)</summary>
        [Description("Access is denied to this file.")]
        NoAccess = unchecked((int) 0x80100027),
        /// <summary>The smart card does not have enough memory to store the information. (SCARD_E_WRITE_TOO_MANY)</summary>
        [Description("The smart card does not have enough memory to store the information.")]
        WriteTooMany = unchecked((int) 0x80100028),

        /// <summary>There was an error trying to set the smart card file object pointer. (SCARD_E_BAD_SEEK)</summary>
        [Description("There was an error trying to set the smart card file object pointer.")]
        BadSeek = unchecked((int) 0x80100029),

        /// <summary>The supplied PIN is incorrect. (SCARD_E_INVALID_CHV)</summary>
        [Description("The supplied PIN is incorrect.")]
        InvalidCHV = unchecked((int) 0x8010002A),
        /// <summary>An unrecognized error code was returned from a layered component. (SCARD_E_UNKNOWN_RES_MNG)</summary>
        [Description("An unrecognized error code was returned from a layered component.")]
        UnknownResMng = unchecked((int) 0x8010002B),
        /// <summary>The requested certificate does not exist. (SCARD_E_NO_SUCH_CERTIFICATE)</summary>
        [Description("The requested certificate does not exist.")]
        NoSuchCertificate = unchecked((int) 0x8010002C),
        /// <summary>The requested certificate could not be obtained. (SCARD_E_CERTIFICATE_UNAVAILABLE)</summary>
        [Description("The requested certificate could not be obtained.")]
        CertificateUnavailable = unchecked((int) 0x8010002D),
        /// <summary>A communications error with the smart card has been detected. (SCARD_E_COMM_DATA_LOST)</summary>
        [Description("A communications error with the smart card has been detected.")]
        CommunicationDataLost = unchecked((int) 0x8010002F),
        /// <summary>The requested key container does not exist on the smart card. (SCARD_E_NO_KEY_CONTAINER)</summary>
        [Description("The requested key container does not exist on the smart card.")]
        NoKeyContainer = unchecked((int) 0x80100030),
        /// <summary>The Smart Card Resource Manager is too busy to complete this operation. (SCARD_E_SERVER_TOO_BUSY)</summary>
        [Description("The Smart Card Resource Manager is too busy to complete this operation.")]
        ServerTooBusy = unchecked((int) 0x80100031),
        /// <summary>Access was denied because of a security violation. (SCARD_W_SECURITY_VIOLATION)</summary>
        [Description("Access was denied because of a security violation.")]
        SecurityViolation = unchecked((int) 0x8010006A),
        /// <summary>The card cannot be accessed because the wrong PIN was presented. (SCARD_W_WRONG_CHV)</summary>
        [Description("The card cannot be accessed because the wrong PIN was presented.")]
        WrongCHV = unchecked((int) 0x8010006B),
        /// <summary>The card cannot be accessed because the maximum number of PIN entry attempts has been reached. (SCARD_W_CHV_BLOCKED)</summary>
        [Description("The card cannot be accessed because the maximum number of PIN entry attempts has been reached.")]
        CHVBlocked = unchecked((int) 0x8010006C),

        /// <summary>The end of the smart card file has been reached. (SCARD_W_EOF)</summary>
        [Description("The end of the smart card file has been reached.")]
        Eof = unchecked((int) 0x8010006D),

        /// <summary>The user pressed "Cancel" on a Smart Card Selection Dialog. (SCARD_W_CANCELLED_BY_USER)</summary>
        [Description("The user pressed \"Cancel\" on a Smart Card Selection Dialog.")]
        CancelledByUser = unchecked((int) 0x8010006E),

        /// <summary>No PIN was presented to the smart card. (SCARD_W_CARD_NOT_AUTHENTICATED)</summary>
        [Description("No PIN was presented to the smart card.")]
        CardNotAuthenticated = unchecked((int) 0x8010006F)
    }
}