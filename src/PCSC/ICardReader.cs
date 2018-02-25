using System;

namespace PCSC
{
    /// <summary>A reader class that implements the most basic PC/SC functions to operate on smart cards, RFID tags and so on.</summary>
    public interface ICardReader : IDisposable
    {
        /// <summary>The connected reader's friendly name.</summary>
        /// <value>A human readable string of the reader name.</value>
        string ReaderName { get; }

        /// <summary>The current mode of connection type: exclusive, shared or direct.</summary>
        SCardShareMode Mode { get; }

        /// <summary>The currently used protocol to communicate with the card.</summary>
        SCardProtocol Protocol { get; }

        /// <summary>
        /// Card or reader handle. <see cref="ICardHandle.Handle"/> can be used for C API calls
        /// </summary>
        ICardHandle CardHandle { get; }

        /// <summary>Connection state of the reader.</summary>
        /// <value><see langword="true" /> if the reader is connected. Otherwise <see langword="false" />.</value>
        bool IsConnected { get; }

        /// <summary>Reestablishes a connection to a reader that was previously connected to using
        ///     <see
        ///         cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />
        ///     .</summary>
        /// <param name="mode">Mode of connection type: exclusive/shared.
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Value</term><description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="SCardShareMode.Shared" />
        ///             </term>
        ///             <description>This application will allow others to share the reader. (SCARD_SHARE_SHARED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="SCardShareMode.Exclusive" />
        ///             </term>
        ///             <description>This application will NOT allow others to share the reader. (SCARD_SHARE_EXCLUSIVE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="SCardShareMode.Direct" />
        ///             </term>
        ///             <description>Direct connection to the reader. (SCARD_SHARE_DIRECT)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="preferredProtocol">Desired protocol use.</param>
        /// <param name="initialExecution">Desired action taken on the card/reader before reconnect.</param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="preferredProtocol" />  is a bit mask of acceptable protocols for the connection. You can use (<see cref="SCardProtocol.T0" /> | <see cref="SCardProtocol.T1" />) if you do not have a preferred protocol. The protocol used with this connection will be stored in <see cref="ISCardReader.ActiveProtocol" />.</para>
        ///     <para>This method calls the API function SCardReconnect().</para>
        /// </remarks>
        void Reconnect(SCardShareMode mode, SCardProtocol preferredProtocol, SCardReaderDisposition initialExecution);

        /// <summary>Establishes a temporary exclusive access mode for doing a serie of commands in a transaction.</summary>
        /// <param name="disposition">Action to be taken on the reader if the user ends the transaction.</param>
        /// <remarks>
        ///     <para>You might want to use this when you are selecting a few files and then writing a large file so you can make sure that another application will not change the current file. If another application has a lock on this reader or this application is in
        ///         <see cref="SCardShareMode.Exclusive" /> there will be no action taken.</para>
        ///     <para>This method calls the API function SCardBeginTransaction(). SCardEndTransaction() will be called when you dispose the returned value</para>
        /// </remarks>
        /// <returns>An anonymous instance implementing <see cref="IDisposable"/> that must be disposed to end the transaction.</returns>
        IDisposable Transaction(SCardReaderDisposition disposition);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <remarks>
        ///     <block subset="none" type="note">
        ///         <para>This method will only work if the reader has been connected with one of the following protocols:
        ///             <list type="table">
        ///                 <listheader><term>Protocol</term><description>Description</description></listheader>
        ///                 <item><term><see cref="SCardProtocol.T0" /></term><description>T=0 active protocol.</description></item>
        ///                 <item><term><see cref="SCardProtocol.T1" /></term><description>T=1 active protocol.</description></item>
        ///                 <item><term><see cref="SCardProtocol.Raw" /></term><description>Raw active protocol.</description></item>
        ///             </list>
        ///         </para>
        ///     </block>
        /// </remarks>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(byte[] sendBuffer, byte[] receiveBuffer);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">A pointer to the protocol header structure for the instruction. This buffer is in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI). You can use one of the following:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Protocol Control Information</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(IntPtr sendPci, byte[] sendBuffer, byte[] receiveBuffer);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">Protocol Control Information in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI)</param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(SCardPCI sendPci, byte[] sendBuffer, byte[] receiveBuffer);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">A pointer to the protocol header structure for the instruction. This buffer is in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI). You can use one of the following:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Protocol Control Information</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receivePci">Pointer to the protocol header structure for the instruction, followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use. This parameter can be NULL if no PCI is returned.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, IntPtr receivePci, byte[] receiveBuffer,
            int receiveBufferLength);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">A pointer to the protocol header structure for the instruction. This buffer is in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI). You can use one of the following:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Protocol Control Information</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer,
            int receiveBufferLength);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">A pointer to the protocol header structure for the instruction. This buffer is in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI). You can use one of the following:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Protocol Control Information</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description>
        ///         </item>
        ///         <item>
        ///             <term><see cref="SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receivePci">Structure of Protocol Header Information followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use. This parameter can be <c>null</c> if no PCI is returned.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci, byte[] receiveBuffer,
            int receiveBufferLength);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">Protocol Control Information in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI)</param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receivePci">Structure of Protocol Header Information followed by a buffer in which to receive any returned protocol control information (PCI) specific to the protocol in use. This parameter can be <c>null</c> if no PCI is returned.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(SCardPCI sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci, byte[] receiveBuffer,
            int receiveBufferLength);

        /// <summary>Sends an APDU to the smart card. </summary>
        /// <param name="sendPci">Protocol Control Information in the format of an SCARD_IO_REQUEST structure, followed by the specific protocol control information (PCI)</param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Transmit(SCardPCI sendPci, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer,
            int receiveBufferLength);

        /// <summary>Sends a command directly to the IFD Handler (reader driver) to be processed by the reader.</summary>
        /// <param name="controlCode">Control code for the operation.</param>
        /// <param name="sendBuffer">Command to send to the reader.</param>
        /// <param name="sendBufferLength"><paramref name="sendBuffer"/> size</param>
        /// <param name="receiveBuffer">Response from the reader.</param>
        /// <param name="receiveBufferLength"><paramref name="receiveBuffer"/> size</param>
        /// <remarks>
        ///     <para>This method is useful for creating client side reader drivers for functions like PIN pads, biometrics, or other extensions to the normal smart card reader that are not normally handled by PC/SC.</para>
        ///     <para>This method calls the API function SCardControl().</para>
        /// </remarks>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Control(IntPtr controlCode, byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer, int receiveBufferLength);

        /// <summary>Sends a command directly to the IFD Handler (reader driver) to be processed by the reader.</summary>
        /// <param name="controlCode">Control code for the operation.</param>
        /// <param name="sendBuffer">Command to send to the reader.</param>
        /// <param name="receiveBuffer">Response from the reader.</param>
        /// <remarks>
        ///     <para>This method is useful for creating client side reader drivers for functions like PIN pads, biometrics, or other extensions to the normal smart card reader that are not normally handled by PC/SC.</para>
        ///     <para>This method calls the API function SCardControl().</para>
        /// </remarks>
        /// <returns>The number of bytes written to the <paramref name="receiveBuffer"/></returns>
        int Control(IntPtr controlCode, byte[] sendBuffer, byte[] receiveBuffer);

        /// <summary>Returns the current status of the reader and the connected card.</summary>
        /// <returns>A reader status instance</returns>
        ReaderStatus GetStatus();

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attributeId">Identifier for the attribute to get.</param>
        /// <param name="receiveBuffer">A buffer that receives the attribute.</param>
        /// <param name="receiveBufferSize"><paramref name="receiveBuffer"/> size.</param>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        /// <returns>The number of bytes written to the attributebuffer</returns>
        int GetAttrib(IntPtr attributeId, byte[] receiveBuffer, int receiveBufferSize);

        /// <summary>Set an attribute of the IFD Handler.</summary>
        /// <param name="attributeId">Identifier for the attribute to set.</param>
        /// <param name="sendBuffer">Buffer that contains the new value of the attribute.</param>
        /// <param name="sendBufferLength">Length of the <paramref name="sendBuffer" /> buffer in bytes.</param>
        /// <remarks>
        ///     <para>The list of attributes you can set depends on the IFD handler you are using.</para>
        ///     <para>This method calls the API function SCardSetAttrib().</para>
        /// </remarks>
        void SetAttrib(IntPtr attributeId, byte[] sendBuffer, int sendBufferLength);
    }
}
