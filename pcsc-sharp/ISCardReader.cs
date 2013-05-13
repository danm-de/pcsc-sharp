using System;

namespace PCSC
{
    /// <summary>Common functions that are needed to operate on Smart Card readers.</summary>
    /// <remarks>
    ///     See <see cref="T:PCSC.SCardReader" />.
    /// </remarks>
    public interface ISCardReader
    {
        /// <summary>Establishes a connection to the Smart Card reader.</summary>
        /// <param name="readerName">Reader name to connect to.</param>
        /// <param name="mode">Mode of connection type: exclusive or shared.</param>
        /// <param name="prefProto">Desired protocol use.</param>
        /// <returns>
        ///     An error code / return value:
        ///     <para>
        ///         <list type="table">
        ///             <listheader>
        ///                 <term>Error code</term><description>Description</description>
        ///             </listheader>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.Success" />
        ///                 </term>
        ///                 <description>Successful (SCARD_S_SUCCESS)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///                 </term>
        ///                 <description>Invalid context handle (SCARD_E_INVALID_HANDLE)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///                 </term>
        ///                 <description>
        ///                     <paramref name="prefProto" /> is invalid or <see langword="null" />  (SCARD_E_INVALID_PARAMETER)
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.InvalidValue" />
        ///                 </term>
        ///                 <description>Invalid sharing mode, requested protocol, or reader name (SCARD_E_INVALID_VALUE)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.NoService" />
        ///                 </term>
        ///                 <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.NoSmartcard" />
        ///                 </term>
        ///                 <description>No smart card present (SCARD_E_NO_SMARTCARD)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.NotReady" />
        ///                 </term>
        ///                 <description>Could not allocate the desired port (SCARD_E_NOT_READY)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///                 </term>
        ///                 <description>Requested protocol is unknown (SCARD_E_PROTO_MISMATCH)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///                 </term>
        ///                 <description>Could not power up the reader or card (SCARD_E_READER_UNAVAILABLE)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.SharingViolation" />
        ///                 </term>
        ///                 <description>Someone else has exclusive rights (SCARD_E_SHARING_VIOLATION)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.UnknownReader" />
        ///                 </term>
        ///                 <description>
        ///                     The reader name is <see langword="null" /> (SCARD_E_UNKNOWN_READER)
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.UnsupportedFeature" />
        ///                 </term>
        ///                 <description>Protocol not supported (SCARD_E_UNSUPPORTED_FEATURE)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.CommunicationError" />
        ///                 </term>
        ///                 <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.InternalError" />
        ///                 </term>
        ///                 <description>An internal consistency check failed (SCARD_F_INTERNAL_ERROR)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.UnpoweredCard" />
        ///                 </term>
        ///                 <description>Card is not powered (SCARD_W_UNPOWERED_CARD)</description>
        ///             </item>
        ///             <item>
        ///                 <term>
        ///                     <see cref="F:PCSC.SCardError.UnresponsiveCard" />
        ///                 </term>
        ///                 <description>Card is mute (SCARD_W_UNRESPONSIVE_CARD)</description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="prefProto" />  is a bit mask of acceptable protocols for the connection. You can use (
        ///         <see
        ///             cref="F:PCSC.SCardProtocol.T0" />
        ///         |
        ///         <see
        ///             cref="F:PCSC.SCardProtocol.T1" />
        ///         ) if you do not have a preferred protocol.
        ///         The protocol used with this connection will be stored in <see cref="P:PCSC.ISCardReader.ActiveProtocol" />.
        ///     </para>
        ///     <para>
        ///         This method calls the API function SCardConnect().
        ///     </para>
        ///     <example>
        ///         <code lang="C#">
        ///  // Establish PC/SC context.
        /// SCardContext ctx = new SCardContext();
        /// ctx.Establish(SCardScope.System);
        /// 
        ///  // Create a Smart Card reader object and connect to it.
        /// ISCardReader reader = new SCardReader(ctx);
        /// SCardError serr = reader.Connect("OMNIKEY CardMan 5x21 00 00",
        /// 	SCardShareMode.Shared,
        /// 	SCardProtocol.Any);
        ///   </code>
        ///     </example>
        /// </remarks>
        SCardError Connect(string readerName, SCardShareMode mode, SCardProtocol prefProto);

        /// <summary>
        ///     Terminates a connection made through
        ///     <see
        ///         cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />
        ///     .
        /// </summary>
        /// <param name="discntExec">Reader function to execute.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return value</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>
        ///                 Invalid <paramref name="discntExec" /> (SCARD_E_INVALID_VALUE)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoSmartcard" />
        ///             </term>
        ///             <description>No smart card present (SCARD_E_NO_SMARTCARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>This method calls the API function SCardDisconnect().</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Establish PC/SC context.
        /// SCardContext ctx = new SCardContext();
        /// ctx.Establish(SCardScope.System);
        /// 
        /// // Create a Smart Card reader object and connect to it.
        /// ISCardReader reader = new SCardReader(ctx);
        /// SCardError serr = reader.Connect("OMNIKEY", SCardShareMode.Shared, SCardProtocol.Any);
        /// 
        /// // Disconnect the reader and reset the SmartCard.
        /// reader.Disconnect(SCardReaderDisposition.Reset);
        ///   </code>
        ///     </example>
        /// </remarks>
        SCardError Disconnect(SCardReaderDisposition discntExec);

        SCardError Reconnect(SCardShareMode mode, SCardProtocol prefProto, SCardReaderDisposition initExec);

        /// <summary>Establishes a temporary exclusive access mode for doing a serie of commands in a transaction.</summary>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return value</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.SharingViolation" />
        ///             </term>
        ///             <description>Someone else has exclusive rights (SCARD_E_SHARING_VIOLATION)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         You might want to use this when you are selecting a few files and then writing a large file so you can make sure that another application will not change the current file. If another application has a lock on this reader or this application is in
        ///         <see
        ///             cref="F:PCSC.SCardShareMode.Exclusive" />
        ///         there will be no action taken.
        ///     </para>
        ///     <para>
        ///         This method calls the API function SCardBeginTransaction().
        ///     </para>
        /// </remarks>
        SCardError BeginTransaction();

        /// <summary>Ends a previously begun transaction.</summary>
        /// <param name="disposition">Action to be taken on the reader.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return value</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>
        ///                 Invalid value for <paramref name="disposition" /> (SCARD_E_INVALID_VALUE)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.SharingViolation" />
        ///             </term>
        ///             <description>Someone else has exclusive rights (SCARD_E_SHARING_VIOLATION)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The calling application must be the owner of the previously begun transaction or an error will occur.
        ///     </para>
        ///     <para>
        ///         This method calls the API function SCardEndTransaction().
        ///     </para>
        ///     <block subset="none" type="note">
        ///         <para>
        ///             The disposition action is not currently used in PC/SC Lite on UNIX/Linux machines.
        ///         </para>
        ///     </block>
        /// </remarks>
        SCardError EndTransaction(SCardReaderDisposition disposition);

        SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, int sendBufLength, SCardPCI ioRecvPci,
            byte[] recvBuffer, ref int recvBufLength);

        SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, SCardPCI ioRecvPci, ref byte[] recvBuffer);
        SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, ref byte[] recvBuffer);
        SCardError Transmit(SCardPCI ioSendPci, byte[] sendBuffer, SCardPCI ioRecvPci, ref byte[] recvBuffer);
        SCardError Transmit(byte[] sendBuffer, int sendBufferLength, byte[] recvBuffer, ref int recvBufferLength);
        SCardError Transmit(byte[] sendBuffer, byte[] recvBuffer, ref int recvBufferLength);
        SCardError Transmit(byte[] sendBuffer, ref byte[] recvBuffer);

        /// <summary>Sends a command directly to the IFD Handler (reader driver) to be processed by the reader.</summary>
        /// <param name="controlCode">Control code for the operation.</param>
        /// <param name="sendBuffer">Command to send to the reader.</param>
        /// <param name="recvBuffer">Response from the reader.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return code</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="recvBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> is <see langword="null" /> and the IFDHandler is version 2.0 (without
        ///                 <paramref
        ///                     name="controlCode" />
        ///                 ) (SCARD_E_INVALID_PARAMETER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid value was presented (SCARD_E_INVALID_VALUE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NotTransacted" />
        ///             </term>
        ///             <description>Data exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed(SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.UnsupportedFeature" />
        ///             </term>
        ///             <description>Driver does not support (SCARD_E_UNSUPPORTED_FEATURE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader(SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         This method is useful for creating client side reader drivers for functions like PIN pads, biometrics, or other extensions to the normal smart card reader that are not normally handled by PC/SC.
        ///     </para>
        ///     <para>
        ///         This method calls the API function SCardControl().
        ///     </para>
        ///     <block subset="none" type="note">
        ///         <para>
        ///             The API of this function changed. In pcsc-lite 1.2.0 and before the API was not Windows(R) PC/SC compatible. This has been corrected.
        ///         </para>
        ///     </block>
        /// </remarks>
        SCardError Control(IntPtr controlCode, byte[] sendBuffer, ref byte[] recvBuffer);

        SCardError Status(out string[] readerName, out SCardState state, out SCardProtocol protocol, out byte[] atr);

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="dwAttrId">Identifier for the attribute to get.</param>
        /// <param name="pbAttr">A buffer that receives the attribute.</param>
        /// <param name="attrLen">Size of the result contained in pbAttr (in bytes).</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return code</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="pbAttr" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 Reader buffer <paramref name="pbAttr" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoMemory" />
        ///             </term>
        ///             <description>Memory allocation failed (SCARD_E_NO_MEMORY)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NotTransacted" />
        ///             </term>
        ///             <description>Data exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        ///     <para>
        ///         The method will return <see cref="F:PCSC.SCardError.InsufficientBuffer" /> if <paramref name="pbAttr" /> is
        ///         <see langword="null" /> or if the buffer size is not sufficient.
        ///         <paramref name="attrLen" /> contains the required amount of bytes (size).
        ///     </para>
        ///    <para>
        ///     If the method returned with <see cref="F:PCSC.SCardError.Success" /> then <paramref name="attrLen" /> contains the exact size of the result in <paramref name="pbAttr" />.
        ///     </para>
        ///     <para>
        ///         For an example please see <see cref="M:PCSC.ISCardReader.GetAttrib(PCSC.SCardAttr,System.Byte[]@)" />.
        ///     </para>
        /// </returns>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        SCardError GetAttrib(IntPtr dwAttrId, byte[] pbAttr, out int attrLen);

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attrId">Identifier for the attribute to get.</param>
        /// <param name="pbAttr">A buffer that receives the attribute.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return code</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="pbAttr" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 Reader buffer <paramref name="pbAttr" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoMemory" />
        ///             </term>
        ///             <description>Memory allocation failed (SCARD_E_NO_MEMORY)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NotTransacted" />
        ///             </term>
        ///             <description>Data exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        SCardError GetAttrib(IntPtr attrId, out byte[] pbAttr);

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attrId">Identifier for the attribute to get.</param>
        /// <param name="pbAttr">A buffer that receives the attribute.</param>
        /// <param name="attrLen">Size of the result contained in pbAttr (in bytes).</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return code</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="pbAttr" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 Reader buffer <paramref name="pbAttr" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoMemory" />
        ///             </term>
        ///             <description>Memory allocation failed (SCARD_E_NO_MEMORY)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NotTransacted" />
        ///             </term>
        ///             <description>Data exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        ///     <para>
        ///         The method will return <see cref="F:PCSC.SCardError.InsufficientBuffer" /> if <paramref name="pbAttr" /> is
        ///         <see langword="null" /> or if the buffer size is not sufficient.
        ///         <paramref name="attrLen" /> contains the required amount of bytes (size).
        ///     </para>
        ///    <para>
        ///     If the method returned with <see cref="F:PCSC.SCardError.Success" /> then <paramref name="attrLen" /> contains the exact size of the result in <paramref name="pbAttr" />.
        ///     </para>
        ///     <para>
        ///         For an example please see <see cref="M:PCSC.ISCardReader.GetAttrib(PCSC.SCardAttr,System.Byte[]@)" />.
        ///     </para>

        /// </returns>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        SCardError GetAttrib(SCardAttr attrId, byte[] pbAttr, out int attrLen);

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attrId">Identifier for the attribute to get.</param>
        /// <param name="pbAttr">A buffer that receives the attribute.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return code</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Success" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="pbAttr" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 Reader buffer <paramref name="pbAttr" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)
        ///             </description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoMemory" />
        ///             </term>
        ///             <description>Memory allocation failed (SCARD_E_NO_MEMORY)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>The server is not runing (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NotTransacted" />
        ///             </term>
        ///             <description>Data exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader has been removed (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        SCardError GetAttrib(SCardAttr attrId, out byte[] pbAttr);

        SCardError SetAttrib(IntPtr attr, byte[] pbAttr, int attrBufSize);
        SCardError SetAttrib(IntPtr attr, byte[] pbAttr);
        SCardError SetAttrib(SCardAttr attr, byte[] pbAttr, int attrBufSize);
        SCardError SetAttrib(SCardAttr attr, byte[] pbAttr);

        /// <summary>
        /// The connected reader's friendly name.
        /// </summary>
        /// <value>A human readable string of the reader name or <see langword="null" /> if the reader object is currently not connected.</value>
        string ReaderName { get; }

        /// <summary>The Smart Card context that will be used for this connection.</summary>
        /// <value>
        ///     <see langword="null" /> if the reader is not connected.
        /// </value>
        SCardContext CurrentContext { get; }

        /// <summary>The current mode of connection type: exclusive or shared.</summary>
        SCardShareMode CurrentShareMode { get; }

        /// <summary>The currently used protocol to communicate with the card.</summary>
        /// <value>
        ///     <see cref="F:PCSC.SCardProtocol.Unset" /> if not connected.
        /// </value>
        SCardProtocol ActiveProtocol { get; }

        /// <summary>A pointer (Card Handle) that can be used for C API calls.</summary>
        /// <value>0 if not connected.</value>
        /// <remarks>
        ///     <para>
        ///         This is the card handle that is returned when calling the C API function SCardConnect().
        ///     </para>
        /// </remarks>
        IntPtr CardHandle { get; }
        bool IsConnected { get; }
    }
}