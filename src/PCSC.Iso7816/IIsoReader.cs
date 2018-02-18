using System;

namespace PCSC.Iso7816
{
    /// <summary>A ISO/IEC 7816 compliant reader.</summary>
    public interface IIsoReader : IDisposable
    {
        /// <summary>Gets the current context.</summary>
        ISCardContext CurrentContext { get; }
        /// <summary>Gets the current reader.</summary>
        ISCardReader Reader { get; }
        /// <summary>Gets the name of the reader.</summary>
        string ReaderName { get; }
        /// <summary>Gets the active protocol.</summary>
        SCardProtocol ActiveProtocol { get; }
        /// <summary>Gets the current share mode.</summary>
        SCardShareMode CurrentShareMode { get; }
        /// <summary>Gets or sets the wait time in milliseconds that is used if an APDU needs to be retransmitted.</summary>
        int RetransmitWaitTime { get; set; }
        /// <summary>Gets the maximum number of bytes that can be received.</summary>
        int MaxReceiveSize { get; }

        /// <summary>Constructs a command APDU using the active protocol of the reader.</summary>
        /// <param name="isoCase">The ISO case that shall be used for this command.</param>
        /// <returns>An empty command APDU.</returns>
        CommandApdu ConstructCommandApdu(IsoCase isoCase);

        /// <summary>Connects the specified reader.</summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="mode">The share mode.</param>
        /// <param name="protocol">The communication protocol. <seealso cref="ISCardReader.Connect(string,SCardShareMode,SCardProtocol)"/></param>
        void Connect(string readerName, SCardShareMode mode, SCardProtocol protocol);

        /// <summary>Disconnects the currently connected reader.</summary>
        /// <param name="disposition">The action that shall be executed after disconnect.</param>
        void Disconnect(SCardReaderDisposition disposition);

        /// <summary>Transmits the specified command APDU.</summary>
        /// <param name="commandApdu">The command APDU.</param>
        /// <returns>A response containing one ore more <see cref="ResponseApdu" />.</returns>
        Response Transmit(CommandApdu commandApdu);
    }
}