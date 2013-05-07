using System;
using System.ComponentModel;

namespace PCSC
{
    [Flags] // Needed for CONST casts
    public enum SCardError
    {
        [Description("No error.")]
        Success = 0x00000000,
        [Description("An internal consistency check failed.")]
        InternalError = unchecked((int)0x80100001),
        [Description("The action was cancelled by an SCardCancel request.")]
        Cancelled = unchecked((int)0x80100002),
        [Description("The supplied handle was invalid. ")]
        InvalidHandle = unchecked((int)0x80100003),
        [Description("One or more of the supplied parameters could not be properly interpreted.")]
        InvalidParameter = unchecked((int)0x80100004),
        [Description("Registry startup information is missing or invalid.")]
        InvalidTarget = unchecked((int)0x80100005),
        [Description("Not enough memory available to complete this command.")]
        NoMemory = unchecked((int)0x80100006),
        [Description("An internal consistency timer has expired.")]
        WaitedTooLong =unchecked((int)0x80100007),
        [Description("The data buffer to receive returned data is too small for the returned data.")]
        InsufficientBuffer = unchecked((int)0x80100008),
        [Description("The specified reader name is not recognized.")]
        UnknownReader = unchecked((int)0x80100009),
        [Description("The user-specified timeout value has expired.")]
        Timeout = unchecked((int)0x8010000A),
        [Description("The smart card cannot be accessed because of other connections outstanding.")]
        SharingViolation = unchecked((int)0x8010000B),
        [Description("The operation requires a Smart Card, but no Smart Card is currently in the device.")]
        NoSmartcard = unchecked((int)0x8010000C),
        [Description("The specified smart card name is not recognized.")]
        UnknownCard = unchecked((int)0x8010000D),
        [Description("The system could not dispose of the media in the requested manner.")]
        CantDispose = unchecked((int)0x8010000E),
        [Description("The requested protocols are incompatible with the protocol currently in use with the smart card.")]
        ProtocolMismatch = unchecked((int)0x8010000F),
        [Description("The reader or smart card is not ready to accept commands.")]
        NotReady = unchecked((int)0x80100010),
        [Description("One or more of the supplied parameters values could not be properly interpreted.")]
        InvalidValue = unchecked((int)0x80100011),
        [Description("The action was cancelled by the system, presumably to log off or shut down.")]
        SystemCancelled = unchecked((int)0x80100012),
        [Description("An internal communications error has been detected.")]
        CommunicationError = unchecked((int)0x80100013),
        [Description("An internal error has been detected, but the source is unknown.")]
        UnknownError = unchecked((int)0x80100014),
        [Description("An ATR obtained from the registry is not a valid ATR string.")]
        InvalidAtr = unchecked((int)0x80100015),
        [Description("An attempt was made to end a non-existent transaction.")]
        NotTransacted = unchecked((int)0x80100016),
        [Description("The specified reader is not currently available for use.")]
        ReaderUnavailable = unchecked((int)0x80100017),

        [Description("The operation has been aborted to allow the server application to exit.")]
        Shutdown = unchecked((int)0x80100018),

        [Description("The reader cannot communicate with the card, due to ATR string configuration conflicts.")]
        UnsupportedCard = unchecked((int)0x80100065),
        [Description("The smart card is not responding to a reset.")]
        UnresponsiveCard = unchecked((int)0x80100066),
        [Description("Power has been removed from the smart card, so that further communication is not possible.")]
        UnpoweredCard = unchecked((int)0x80100067),
        [Description("The smart card has been reset, so any shared state information is invalid.")]
        ResetCard = unchecked((int)0x80100068),
        [Description("The smart card has been removed, so further communication is not possible.")]
        RemovedCard = unchecked((int)0x80100069),

        [Description("The PCI Receive buffer was too small.")]
        PciTooSmall = unchecked((int)0x80100019),
        [Description("The reader driver does not meet minimal requirements for support.")]
        ReaderUnsupported = unchecked((int)0x8010001A),
        [Description("The reader driver did not produce a unique reader name.")]
        DuplicateReader = unchecked((int)0x8010001B),
        [Description("The smart card does not meet minimal requirements for support.")]
        CardUnsupported = unchecked((int)0x8010001C),
        [Description("The Smart card resource manager is not running.")]
        NoService = unchecked((int)0x8010001D),
        [Description("The Smart card resource manager has shut down.")]
        ServiceStopped = unchecked((int)0x8010001E),

        [Description("An unexpected card error has occurred.")]
        Unexpected = unchecked((int)0x8010001F),

        [Description("Cannot find a smart card reader.")]
        NoReadersAvailable = unchecked((int)0x8010002E),

        /** PC/SC Lite specific extensions */
        [Description("Card inserted.")]
        [Obsolete("PC/SC Lite specific, value conflicts with SCardError.SecurityViolation")]
        InsertedCard = unchecked((int)0x8010006A),
        [Description("Feature not supported")]
        UnsupportedFeature = unchecked((int)0x8010001F),

        [Description("No primary provider can be found for the smart card.")]
        ICCInstallation = unchecked((int)0x80100020),
        [Description("The requested order of object creation is not supported.")]
        ICCCreateOrder = unchecked((int)0x80100021),

        [Description("The identified directory does not exist in the smart card.")]
        DirectoryNotFound = unchecked((int)0x80100023),
        [Description("The identified file does not exist in the smart card.")]
        FileNotFound = unchecked((int)0x80100024),
        [Description("The supplied path does not represent a smart card directory. ")]
        NoDir = unchecked((int)0x80100025),
        [Description("The supplied path does not represent a smart card file.")]
        NoFile = unchecked((int)0x80100026),
        [Description("Access is denied to this file.")]
        NoAccess = unchecked((int)0x80100027),

        [Description("The smart card does not have enough memory to store the information.")]
        WriteTooMany = unchecked((int)0x80100028),
        [Description("There was an error trying to set the smart card file object pointer.")]
        BadSeek = unchecked((int)0x80100029),

        [Description("The supplied PIN is incorrect.")]
        InvalidCHV = unchecked((int)0x8010002A),
        [Description("An unrecognized error code was returned from a layered component.")]
        UnknownResMng = unchecked((int)0x8010002B),

        [Description("The requested certificate does not exist.")]
        NoSuchCertificate = unchecked((int)0x8010002C),
        [Description("The requested certificate could not be obtained.")]
        CertificateUnavailable = unchecked((int)0x8010002D),
        [Description("A communications error with the smart card has been detected.")]
        CommunicationDataLost = unchecked((int)0x8010002F),
        [Description("The requested key container does not exist on the smart card.")]
        NoKeyContainer = unchecked((int)0x80100030),
        [Description("The Smart Card Resource Manager is too busy to complete this operation.")]
        ServerTooBusy = unchecked((int)0x80100031),
        [Description("Access was denied because of a security violation.")]
        SecurityViolation = unchecked((int)0x8010006A),
        [Description("The card cannot be accessed because the wrong PIN was presented.")]
        WrongCHV = unchecked((int)0x8010006B),
        [Description("The card cannot be accessed because the maximum number of PIN entry attempts has been reached.")]
        CHVBlocked = unchecked((int)0x8010006C),
        [Description("The end of the smart card file has been reached.")]
        Eof = unchecked((int)0x8010006D),
        [Description("The user pressed \"Cancel\" on a Smart Card Selection Dialog.")]
        CancelledByUser = unchecked((int)0x8010006E),
        [Description("No PIN was presented to the smart card.")]
        CardNotAuthenticated = unchecked((int)0x8010006F)

    }
}