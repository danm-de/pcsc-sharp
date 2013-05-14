using System;
using PCSC.Interop;

namespace PCSC
{
    /// <summary>A reader class with common functions that are needed to operate on smart cards. It will use the system's PC/SC API.</summary>
    public class SCardReader : ISCardReader
    {
        private SCardShareMode _sharemode;
        private ISCardContext _context;
        private IntPtr _cardHandle;
        private SCardProtocol _activeprot;
        private string _readername;

        /// <summary>The connected reader's friendly name.</summary>
        /// <value>A human readable string of the reader name or <see langword="null" /> if the reader object is currently not connected.</value>
        public string ReaderName {
            get { return _readername; }
            protected set { _readername = value; }
        }

        /// <summary>The Smart Card context that will be used for this connection.</summary>
        /// <value><see langword="null" /> if the reader is not connected.</value>
        public ISCardContext CurrentContext {
            get { return _context; }
            protected set { _context = value; }
        }

        /// <summary>The current mode of connection type: exclusive or shared.</summary>
        public SCardShareMode CurrentShareMode {
            get { return _sharemode; }
            protected set { _sharemode = value; }
        }

        /// <summary>The currently used protocol to communicate with the card.</summary>
        /// <value>
        ///     <see cref="F:PCSC.SCardProtocol.Unset" /> if not connected.</value>
        public SCardProtocol ActiveProtocol {
            get { return _activeprot; }
            protected set { _activeprot = value; }
        }

        /// <summary>A pointer (Card Handle) that can be used for C API calls.</summary>
        /// <value>0 if not connected.</value>
        /// <remarks>
        ///     <para>This is the card handle that is returned when calling the C API function SCardConnect().</para>
        /// </remarks>
        public IntPtr CardHandle {
            get { return _cardHandle; }
        }

        /// <summary>The current connection state of the reader.</summary>
        /// <value><see langword="true" /> if the reader is connected. Otherwise <see langword="false" />.</value>
        public bool IsConnected {
            get { return _cardHandle != IntPtr.Zero; }
        }

        ~SCardReader() {
            Dispose(false);
        }

        public SCardReader(ISCardContext context) {
            _context = context;
        }

        /// <summary>Establishes a connection to the Smart Card reader.</summary>
        /// <param name="readerName">Reader name to connect to.</param>
        /// <param name="mode">Mode of connection type: exclusive or shared.
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Value</term><description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term><see cref="F:PCSC.SCardShareMode.Shared" /></term>
        ///             <description>This application will allow others to share the reader. (SCARD_SHARE_SHARED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardShareMode.Exclusive" />
        ///             </term>
        ///             <description>This application will NOT allow others to share the reader. (SCARD_SHARE_EXCLUSIVE)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="preferredProtocol">Desired protocol use.</param>
        /// <returns>An error code / return value:
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
        ///                     <paramref name="preferredProtocol" /> is invalid or <see langword="null" />  (SCARD_E_INVALID_PARAMETER)</description>
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
        ///                 <description>The reader name is <see langword="null" /> (SCARD_E_UNKNOWN_READER)</description>
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
        ///         <paramref name="preferredProtocol" />  is a bit mask of acceptable protocols for the connection. You can use (<see cref="F:PCSC.SCardProtocol.T0" /> | <see cref="F:PCSC.SCardProtocol.T1" />) if you do not have a preferred protocol. The protocol used with this connection will be stored in <see cref="P:PCSC.ISCardReader.ActiveProtocol" />.</para>
        ///     <para>This method calls the API function SCardConnect().</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Establish PC/SC context.
        /// SCardContext ctx = new SCardContext();
        /// ctx.Establish(SCardScope.System);
        /// 
        /// // Create a Smart Card reader object and connect to it.
        /// ISCardReader reader = new SCardReader(ctx);
        /// SCardError serr = reader.Connect("OMNIKEY CardMan 5x21 00 00",
        /// 	SCardShareMode.Shared,
        /// 	SCardProtocol.Any);
        ///   </code>
        ///     </example>
        /// </remarks>
        public SCardError Connect(string readerName, SCardShareMode mode, SCardProtocol preferredProtocol) {
            if (readerName == null) {
                throw new ArgumentNullException("readerName");
            }

            if (string.IsNullOrWhiteSpace(readerName)) {
                throw new UnknownReaderException(SCardError.InvalidValue, "Invalid card reader name.");
            }

            if (_context == null || _context.Handle.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle, "Invalid connection context.");
            }

            IntPtr hCard;
            SCardProtocol dwActiveProtocol;

            var rc = Platform.Lib.Connect(_context.Handle,
                readerName,
                mode,
                preferredProtocol,
                out hCard,
                out dwActiveProtocol);

            if (rc != SCardError.Success) {
                return rc;
            }

            _cardHandle = hCard;
            _activeprot = dwActiveProtocol;
            _readername = readerName;
            _sharemode = mode;

            return rc;
        }

        private void ThrowOnInvalidCardHandle() {
            if (_cardHandle.Equals(IntPtr.Zero)) {
                throw new InvalidOperationException(
                    "Reader is currently not connected or no card handle has been returned.");
            }
        }

        /// <summary>Terminates a connection made through <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />.</summary>
        /// <param name="disconnectExecution">Reader function to execute.</param>
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
        ///             <description>Invalid <paramref name="disconnectExecution" /> (SCARD_E_INVALID_VALUE)</description>
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
        public SCardError Disconnect(SCardReaderDisposition disconnectExecution) {
            ThrowOnInvalidCardHandle();

            var rc = Platform.Lib.Disconnect(_cardHandle, disconnectExecution);

            if (rc != SCardError.Success) {
                return rc;
            }

            // reset local variables
            _readername = null;
            _cardHandle = IntPtr.Zero;
            _activeprot = SCardProtocol.Unset;
            _sharemode = SCardShareMode.Shared;

            return rc;
        }

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
        ///                 <see cref="F:PCSC.SCardShareMode.Shared" />
        ///             </term>
        ///             <description>This application will allow others to share the reader. (SCARD_SHARE_SHARED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardShareMode.Exclusive" />
        ///             </term>
        ///             <description>This application will NOT allow others to share the reader. (SCARD_SHARE_EXCLUSIVE)</description>
        ///         </item>
        ///     </list>
        /// </param>
        /// <param name="preferredProtocol">Desired protocol use.</param>
        /// <param name="initialExecution">Desired action taken on the card/reader before reconnect.</param>
        /// <returns>An error code / return value:
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Error code</term><description>Description</description>
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
        ///             <description>Invalid context handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="preferredProtocol" /> is invalid or <see langword="null" />  (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid sharing mode, requested protocol, or reader name (SCARD_E_INVALID_VALUE)</description>
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
        ///                 <see cref="F:PCSC.SCardError.NotReady" />
        ///             </term>
        ///             <description>Could not allocate the desired port (SCARD_E_NOT_READY)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Requested protocol is unknown (SCARD_E_PROTO_MISMATCH)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>Could not power up the reader or card (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.SharingViolation" />
        ///             </term>
        ///             <description>Someone else has exclusive rights (SCARD_E_SHARING_VIOLATION)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.UnsupportedFeature" />
        ///             </term>
        ///             <description>Protocol not supported (SCARD_E_UNSUPPORTED_FEATURE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.CommunicationError" />
        ///             </term>
        ///             <description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InternalError" />
        ///             </term>
        ///             <description>An internal consistency check failed (SCARD_F_INTERNAL_ERROR)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The smart card has been removed (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.UnresponsiveCard" />
        ///             </term>
        ///             <description>Card is mute (SCARD_W_UNRESPONSIVE_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="preferredProtocol" />  is a bit mask of acceptable protocols for the connection. You can use (<see cref="F:PCSC.SCardProtocol.T0" /> | <see cref="F:PCSC.SCardProtocol.T1" />) if you do not have a preferred protocol. The protocol used with this connection will be stored in <see cref="P:PCSC.ISCardReader.ActiveProtocol" />.</para>
        ///     <para>This method calls the API function SCardReconnect().</para>
        /// </remarks>
        public SCardError Reconnect(SCardShareMode mode, SCardProtocol preferredProtocol, SCardReaderDisposition initialExecution) {
            ThrowOnInvalidCardHandle();

            SCardProtocol dwActiveProtocol;
            var rc = Platform.Lib.Reconnect(_cardHandle,
                mode,
                preferredProtocol,
                initialExecution,
                out dwActiveProtocol);

            if (rc != SCardError.Success) {
                return rc;
            }

            _activeprot = dwActiveProtocol;
            _sharemode = mode;

            return rc;
        }

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
        ///     <para>You might want to use this when you are selecting a few files and then writing a large file so you can make sure that another application will not change the current file. If another application has a lock on this reader or this application is in
        ///         <see cref="F:PCSC.SCardShareMode.Exclusive" /> there will be no action taken.</para>
        ///     <para>This method calls the API function SCardBeginTransaction().</para>
        /// </remarks>
        public SCardError BeginTransaction() {
            ThrowOnInvalidCardHandle(); 
            
            return Platform.Lib.BeginTransaction(_cardHandle);
        }

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
        ///             <description>Invalid value for <paramref name="disposition" /> (SCARD_E_INVALID_VALUE)</description>
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
        ///     <para>The calling application must be the owner of the previously begun transaction or an error will occur.</para>
        ///     <para>This method calls the API function SCardEndTransaction().</para>
        ///     <block subset="none" type="note">
        ///         <para>The disposition action is not currently used in PC/SC Lite on UNIX/Linux machines.</para>
        ///     </block>
        /// </remarks>
        public SCardError EndTransaction(SCardReaderDisposition disposition) {
            ThrowOnInvalidCardHandle();

            return Platform.Lib.EndTransaction(_cardHandle, disposition);
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />.</summary>
        /// <param name="sendPci">Structure of Protocol Control Information.</param>
        /// <param name="sendBuffer">APDU to send to the card.</param>
        /// <param name="receivePci">Structure of protocol information.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
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
        ///             <description> 	Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> or <paramref name="sendPci" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />. <paramref name="sendPci" /> and <paramref name="receivePci" /> are structures containing the following (implemented in <see cref="T:PCSC.SCardPCI" />):
        ///         <example><code lang="C">
        /// typedef struct {
        /// 	DWORD dwProtocol;    // SCARD_PROTOCOL_T0 or SCARD_PROTOCOL_T1
        /// 	DWORD cbPciLength;   // Length of this structure - not used
        /// } SCARD_IO_REQUEST;
        ///   </code></example>
        ///     </para>
        ///     <para>It is recommended to use pre-defined / built-in PCI structures by calling one of the following methods:
        ///         <list type="bullet">
        ///             <item><term><see cref="M:PCSC.ISCardReader.Transmit(System.IntPtr,System.Byte[],PCSC.SCardPCI,System.Byte[]@)" /></term></item>
        ///             <item><term><see cref="M:PCSC.ISCardReader.Transmit(System.IntPtr,System.Byte[],System.Byte[]@)" /></term></item></list></para>
        ///     <para>This method calls the API function SCardTransmit(). The pointers to the pre-defined / built-in PCI structures are determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</para>
        /// </remarks>
        public SCardError Transmit(SCardPCI sendPci, byte[] sendBuffer, SCardPCI receivePci, ref byte[] receiveBuffer) {
            if (sendPci == null) {
                throw new ArgumentNullException("sendPci");
            }
            if (sendPci.MemoryPtr == IntPtr.Zero) {
                throw new ArgumentException("sendPci");
            }

            return Transmit(
                sendPci.MemoryPtr, 
                sendBuffer, 
                receivePci, 
                ref receiveBuffer);
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />. </summary>
        /// <param name="sendPci">A pointer to a pre-defined Structure of Protocol Control Information. You can use one of the following:
        ///     <list type="table">
        ///         <listheader><term>Protocol Control Information</term><description>Description</description></listheader>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item>
        ///         <item><term><see cref="P:PCSC.SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item>
        ///     </list></param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="receiveBuffer">Response from the card.</param>
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
        ///             <description> 	Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> or <paramref name="sendPci" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />.</para>
        ///     <para>This method calls the API function SCardTransmit(). The pointers to the pre-defined / built-in PCI structures are determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</para>
        /// </remarks>
        public SCardError Transmit(IntPtr sendPci, byte[] sendBuffer, ref byte[] receiveBuffer) {
            return Transmit(
                sendPci, 
                sendBuffer, 
                null, 
                ref receiveBuffer);
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />. </summary>
        /// <param name="sendPci">A pointer to a pre-defined Structure of Protocol Control Information. You can use one of the following:
        ///     <list type="table"><listheader><term>Protocol Control Information</term><description>Description</description></listheader><item><term><see cref="P:PCSC.SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item><item><term><see cref="P:PCSC.SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item><item><term><see cref="P:PCSC.SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item></list></param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receivePci">Structure of protocol information. </param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
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
        ///             <description> 	Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> or <paramref name="sendPci" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />. The size of the returned data will be stored in <paramref name="receiveBufferLength" />. This method will return with error code <see cref="F:PCSC.SCardError.InsufficientBuffer" /> if the buffer size of <paramref name="receiveBuffer" /> is too small for the result. If one of the parameters <paramref name="sendBufferLength" /> or <paramref name="receiveBufferLength" /> is invalid, the method will throw an <see cref="T:System.ArgumentOutOfRangeException" />.</para>
        ///     <para>This method calls the API function SCardTransmit(). The pointers to the pre-defined / built-in PCI structures are determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</para>
        /// </remarks>
        public SCardError Transmit(IntPtr sendPci, byte[] sendBuffer, int sendBufferLength, SCardPCI receivePci, byte[] receiveBuffer, ref int receiveBufferLength) {
            ThrowOnInvalidCardHandle();

            var ioRecvPciPtr = IntPtr.Zero;
            if (receivePci != null) {
                ioRecvPciPtr = receivePci.MemoryPtr;
            }

            return Platform.Lib.Transmit(
                _cardHandle,
                sendPci,
                sendBuffer,
                sendBufferLength,
                ioRecvPciPtr,
                receiveBuffer,
                ref receiveBufferLength);
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />. </summary>
        /// <param name="sendPci">A pointer to a pre-defined Structure of Protocol Control Information. You can use one of the following:
        ///     <list type="table"><listheader><term>Protocol Control Information</term><description>Description</description></listheader><item><term><see cref="P:PCSC.SCardPCI.T0" /></term><description>Pre-defined T=0 PCI structure. (SCARD_PCI_T0)</description></item><item><term><see cref="P:PCSC.SCardPCI.T1" /></term><description>Pre-defined T=1 PCI structure. (SCARD_PCI_T1)</description></item><item><term><see cref="P:PCSC.SCardPCI.Raw" /></term><description>Pre-defined RAW PCI structure. (SCARD_PCI_RAW)</description></item></list></param>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="receivePci">Structure of protocol information. </param>
        /// <param name="receiveBuffer">Response from the card. </param>
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
        ///             <description> 	Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> or <paramref name="sendPci" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />. <paramref name="receivePci" /> is a structure containing the following (implemented in <see cref="T:PCSC.SCardPCI" />):
        ///         <example><code lang="C">
        /// typedef struct {
        /// 	DWORD dwProtocol;    // SCARD_PROTOCOL_T0 or SCARD_PROTOCOL_T1
        /// 	DWORD cbPciLength;   // Length of this structure - not used
        /// } SCARD_IO_REQUEST;
        ///   </code></example>
        ///     </para>
        ///     <para>This method calls the API function SCardTransmit(). The pointers to the pre-defined / built-in PCI structures are determinated with dlsym() on UNIX/Linux hosts and GetProcAddress() on Windows hosts.</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a new PC/SC context.
        /// var ctx = new SCardContext();
        /// ctx.Establish(SCardScope.System);
        /// 
        /// // Connect to the reader
        /// var rfidReader = new SCardReader(ctx);
        /// SCardError rc = rfidReader.Connect(
        /// 	"OMNIKEY CardMan 5x21 00 01", 
        /// 	SCardShareMode.Shared, 
        /// 	SCardProtocol.T1);
        /// 
        /// if (rc != SCardError.Success) {
        /// 	Console.WriteLine("Unable to connect to RFID card / chip. Error: " + SCardHelper.StringifyError(rc));
        /// 	return;
        /// }
        /// 
        /// // prepare APDU
        /// byte[] ucByteSend = new byte[] 
        /// 	{
        /// 		0xFF,   // the instruction class
        /// 		0xCA,   // the instruction code 
        /// 		0x00,   // parameter to the instruction
        /// 		0x00,   // parameter to the instruction
        /// 		0x00    // size of I/O transfer
        /// 	};
        /// 
        /// rc = rfidReader.BeginTransaction();
        /// if (rc != SCardError.Success) {
        /// 	throw new Exception("Could not begin transaction.");
        /// }
        /// 
        /// Console.Out.WriteLine("Retrieving the UID .... ");
        /// 
        /// SCardPCI ioreq = new SCardPCI();   // IO returned protocol control information.
        /// byte[] ucByteReceive = new byte[10];
        /// rc = rfidReader.Transmit(
        /// 	SCardPCI.T1,// Protocol control information, T0, T1 and Raw
        /// 	            // are global defined protocol header structures. 
        /// 	ucByteSend, // the actual data to be written to the card 
        /// 	ioreq,      // The returned protocol control information 
        /// 	ref ucByteReceive);
        /// 
        /// if (rc == SCardError.Success) {
        /// 	Console.WriteLine("Uid: {0}", BitConverter.ToString(ucByteReceive));
        /// } else {
        /// 	Console.WriteLine("Error: " + SCardHelper.StringifyError(rc));
        /// }
        ///             
        /// rfidReader.EndTransaction(SCardReaderDisposition.Leave);
        /// rfidReader.Disconnect(SCardReaderDisposition.Reset);
        /// ctx.Release()
        ///   </code>
        ///     </example>
        /// </remarks>
        public SCardError Transmit(IntPtr sendPci, byte[] sendBuffer, SCardPCI receivePci, ref byte[] receiveBuffer) {
            ThrowOnInvalidCardHandle();

            int pcbRecvLength;
            var ioRecvPciPtr = IntPtr.Zero;
            
            if (receivePci != null) {
                ioRecvPciPtr = receivePci.MemoryPtr;
            }

            var rc = Platform.Lib.Transmit(
                _cardHandle,
                sendPci,
                sendBuffer,
                ioRecvPciPtr,
                receiveBuffer,
                out pcbRecvLength);

            if (rc != SCardError.Success) {
                return rc;
            }

            if (receiveBuffer != null && pcbRecvLength < receiveBuffer.Length) {
                Array.Resize(ref receiveBuffer, pcbRecvLength);
            }

            return rc;
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />. </summary>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="sendBufferLength">The buffer size of <paramref name="sendBuffer" /> in bytes.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
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
        ///             <description> 	Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />.  The buffer <paramref name="receiveBuffer" /> must be initialized. The size of the returned data will be stored in <paramref name="receiveBufferLength" />. This method will return with error code <see cref="F:PCSC.SCardError.InsufficientBuffer" /> if the buffer size of <paramref name="receiveBuffer" /> is too small for the result. If one of the parameters <paramref name="sendBufferLength" /> or <paramref name="receiveBufferLength" /> is invalid, the method will throw an <see cref="T:System.ArgumentOutOfRangeException" />.</para>
        ///     <para>This method calls the API function SCardTransmit().</para>
        ///     <block subset="none" type="note">
        ///         <para>This method will only work if the reader has been connected with one of the following protocols:
        ///             <list type="table">
        ///                 <listheader><term>Protocol</term><description>Description</description></listheader>
        ///                 <item><term><see cref="F:PCSC.SCardProtocol.T0" /></term><description>T=0 active protocol.</description></item>
        ///                 <item><term><see cref="F:PCSC.SCardProtocol.T1" /></term><description>T=1 active protocol.</description></item>
        ///                 <item><term><see cref="F:PCSC.SCardProtocol.Raw" /></term><description>Raw active protocol.</description></item>
        ///             </list></para>
        ///     </block>
        /// </remarks>
        public SCardError Transmit(byte[] sendBuffer, int sendBufferLength, byte[] receiveBuffer, ref int receiveBufferLength) {
            SCardPCI iorecvpci = new SCardPCI(); // will be discarded
            return Transmit(
                SCardPCI.GetPci(_activeprot),
                sendBuffer,
                sendBufferLength,
                iorecvpci,
                receiveBuffer,
                ref receiveBufferLength);
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />.</summary>
        /// <param name="sendBuffer">APDU to send to the card. </param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <param name="receiveBufferLength">The buffer size of <paramref name="receiveBuffer" /> in bytes.</param>
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
        ///             <description> 	Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />.  The buffer <paramref name="receiveBuffer" /> must be initialized. The size of the returned data will be stored in <paramref name="receiveBufferLength" />. This method will return with error code <see cref="SCardError.InsufficientBuffer" /> if the buffer size of <paramref name="receiveBuffer" /> is too small for the result. If the parameter <paramref name="receiveBufferLength" /> is invalid, the method will throw an <see cref="T:System.ArgumentOutOfRangeException" />.</para>
        ///     <para>This method calls the API function SCardTransmit().</para>
        ///     <block subset="none" type="note">
        ///         <para>This method will only work if the reader has been connected with one of the following protocols:
        ///             <list type="table"><listheader><term>Protocol</term><description>Description</description></listheader><item><term><see cref="F:PCSC.SCardProtocol.T0" /></term><description>T=0 active protocol.</description></item><item><term><see cref="F:PCSC.SCardProtocol.T1" /></term><description>T=1 active protocol.</description></item><item><term><see cref="F:PCSC.SCardProtocol.Raw" /></term><description>Raw active protocol.</description></item></list></para>
        ///     </block>
        /// </remarks>
        public SCardError Transmit(byte[] sendBuffer, byte[] receiveBuffer, ref int receiveBufferLength) {
            var sendbufsize = 0;
            
            if (sendBuffer != null) {
                sendbufsize = sendBuffer.Length;
            }

            return Transmit(
                sendBuffer,
                sendbufsize,
                receiveBuffer,
                ref receiveBufferLength);
        }

        /// <summary>Sends an APDU to the smart card that was previously connected by <see cref="M:PCSC.ISCardReader.Connect(System.String,PCSC.SCardShareMode,PCSC.SCardProtocol)" />.</summary>
        /// <param name="sendBuffer">APDU to send to the card.</param>
        /// <param name="receiveBuffer">Response from the card.</param>
        /// <returns><list type="table">
        ///         <listheader><term>Return value</term><description>Description</description></listheader>
        ///         <item><term><see cref="F:PCSC.SCardError.Success" /></term><description>Successful (SCARD_S_SUCCESS)</description></item>
        ///         <item><term><see cref="F:PCSC.SCardError.InsufficientBuffer" /></term><description><paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description></item>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> is <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid Protocol, reader name, etc (SCARD_E_INVALID_VALUE)</description>
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
        ///             <description>APDU exchange not successful (SCARD_E_NOT_TRANSACTED)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ProtocolMismatch" />
        ///             </term>
        ///             <description>Connect protocol is different than desired (SCARD_E_PROTO_MISMATCH)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The card has been reset by another application (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The card has been removed from the reader (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The card responds from the APDU and stores this response in <paramref name="receiveBuffer" />. The buffer <paramref name="receiveBuffer" /> must be initialized and will be resized, if the buffer was too big.</para>
        ///     <para>This method calls the API function SCardTransmit().</para>
        ///     <block subset="none" type="note">
        ///         <para>This method will only work if the reader has been connected with one of the following protocols:
        ///             <list type="table">
        ///                 <listheader><term>Protocol</term><description>Description</description></listheader>
        ///                 <item><term><see cref="F:PCSC.SCardProtocol.T0" /></term><description>T=0 active protocol.</description></item>
        ///                 <item><term><see cref="F:PCSC.SCardProtocol.T1" /></term><description>T=1 active protocol.</description></item>
        ///                 <item><term><see cref="F:PCSC.SCardProtocol.Raw" /></term><description>Raw active protocol.</description></item>
        ///             </list></para>
        ///     </block>
        /// </remarks>
        public SCardError Transmit(byte[] sendBuffer, ref byte[] receiveBuffer) {
            var recvbufsize = 0;
            
            if (receiveBuffer != null) {
                recvbufsize = receiveBuffer.Length;
            }

            var sc = Transmit(
                sendBuffer,
                receiveBuffer,
                ref recvbufsize);

            if (sc != SCardError.Success) {
                return sc;
            }
            
            if (receiveBuffer != null && (recvbufsize < receiveBuffer.Length)) {
                Array.Resize(ref receiveBuffer, recvbufsize);
            }

            return sc;
        }

        /// <summary>Sends a command directly to the IFD Handler (reader driver) to be processed by the reader.</summary>
        /// <param name="controlCode">Control code for the operation.</param>
        /// <param name="sendBuffer">Command to send to the reader.</param>
        /// <param name="receiveBuffer">Response from the reader.</param>
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
        ///                 <paramref name="sendBuffer" /> or <paramref name="receiveBuffer" /> are too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///                 ) (SCARD_E_INVALID_PARAMETER)</description>
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
        ///     <para>This method is useful for creating client side reader drivers for functions like PIN pads, biometrics, or other extensions to the normal smart card reader that are not normally handled by PC/SC.</para>
        ///     <para>This method calls the API function SCardControl().</para>
        ///     <block subset="none" type="note">
        ///         <para>The API of this function changed. In pcsc-lite 1.2.0 and before the API was not Windows(R) PC/SC compatible. This has been corrected.</para>
        ///     </block>
        /// </remarks>
        public SCardError Control(IntPtr controlCode, byte[] sendBuffer, ref byte[] receiveBuffer) {
            ThrowOnInvalidCardHandle();

            int lpBytesReturned;

            var rc = Platform.Lib.Control(
                _cardHandle,
                controlCode,
                sendBuffer,
                receiveBuffer,
                out lpBytesReturned);

            if (rc != SCardError.Success || receiveBuffer == null) {
                return rc;
            }

            if (lpBytesReturned < receiveBuffer.Length) {
                Array.Resize(ref receiveBuffer, lpBytesReturned);
            }

            return rc;
        }

        /// <summary>Returns the current status of the reader and the connected card.</summary>
        /// <param name="readerName">The connected readers's friendly name.</param>
        /// <param name="state">The current state.</param>
        /// <param name="protocol">The card's currently used protocol.</param>
        /// <param name="atr">The card's ATR.</param>
        /// <returns><list type="table">
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
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>The reader object did not allocate enough memory for <paramref name="readerName" /> or for <paramref name="atr" /> (SCARD_E_INSUFFICIENT_BUFFER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>The reader object got invalid. Invalid card handle (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>The reader object passed a size of null for <paramref name="readerName" /> or <paramref name="atr" />  (SCARD_E_INVALID_PARAMETER)</description>
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
        ///             <description> The server is not runing (SCARD_E_NO_SERVICE)</description>
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
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InternalError" />
        ///             </term>
        ///             <description>An internal consistency check failed (SCARD_F_INTERNAL_ERROR)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.RemovedCard" />
        ///             </term>
        ///             <description>The smart card has been removed (SCARD_W_REMOVED_CARD)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ResetCard" />
        ///             </term>
        ///             <description>The smart card has been reset (SCARD_W_RESET_CARD)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>The connected readers's friendly name will be stored in <paramref name="readerName" />. The card's ATR will be stored in <paramref name="atr" />. The current state, and protocol will be stored in <paramref name="state" /> and <paramref name="protocol" /> respectively.</para>
        ///     <para>This method calls the API function SCardStatus().</para>
        /// </remarks>
        public SCardError Status(out string[] readerName, out SCardState state, out SCardProtocol protocol, out byte[] atr) {
            IntPtr dwState;
            IntPtr dwProtocol;

            var rc = Platform.Lib.Status(
                _cardHandle,
                out readerName,
                out dwState,
                out dwProtocol,
                out atr);

            if (rc == SCardError.Success) {
                protocol = SCardHelper.ToProto(dwProtocol);
                state = SCardHelper.ToState(dwState);

                // update local copies 
                _activeprot = protocol;
                if (readerName.Length >= 1) {
                    _readername = readerName[0];
                }
            } else {
                protocol = SCardProtocol.Unset;
                state = SCardState.Unknown;
            }

            return rc;
        }

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attributeId">Identifier for the attribute to get.</param>
        /// <param name="attribute">A buffer that receives the attribute.</param>
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
        ///                 <paramref name="attribute" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>Reader buffer <paramref name="attribute" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        public SCardError GetAttrib(SCardAttr attributeId, out byte[] attribute) {
            return GetAttrib((IntPtr) attributeId, out attribute);
        }

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attributeId">Identifier for the attribute to get.</param>
        /// <param name="attribute">A buffer that receives the attribute.</param>
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
        ///                 <paramref name="attribute" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>Reader buffer <paramref name="attribute" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        public SCardError GetAttrib(IntPtr attributeId, out byte[] attribute) {
            int attrlen;

            // receive needed size for attribute
            var rc = GetAttrib(attributeId, null, out attrlen);

            if (rc != SCardError.Success) {
                throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }

            attribute = new byte[attrlen];
            return GetAttrib(attributeId, attribute, out attrlen);
        }

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attributeId">Identifier for the attribute to get.</param>
        /// <param name="attribute">A buffer that receives the attribute.</param>
        /// <param name="attributeBufferLength">Size of the result contained in attribute (in bytes).</param>
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
        ///                 <paramref name="attribute" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>Reader buffer <paramref name="attribute" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        ///     <para>The method will return <see cref="F:PCSC.SCardError.InsufficientBuffer" /> if <paramref name="attribute" /> is
        ///         <see langword="null" /> or if the buffer size is not sufficient.
        ///         <paramref name="attributeBufferLength" /> contains the required amount of bytes (size).</para>
        ///     <para>If the method returned with <see cref="F:PCSC.SCardError.Success" /> then <paramref name="attributeBufferLength" /> contains the exact size of the result in
        ///         <paramref
        ///             name="attribute" />
        ///         .</para>
        ///     <para>For an example please see <see cref="M:PCSC.ISCardReader.GetAttrib(PCSC.SCardAttr,System.Byte[]@)" />.</para>
        /// </returns>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        public SCardError GetAttrib(SCardAttr attributeId, byte[] attribute, out int attributeBufferLength) {
            return GetAttrib((IntPtr) attributeId, attribute, out attributeBufferLength);
        }

        /// <summary>Gets an attribute from the IFD Handler (reader driver).</summary>
        /// <param name="attributeId">Identifier for the attribute to get.</param>
        /// <param name="attribute">A buffer that receives the attribute.</param>
        /// <param name="attributeBufferLength">Size of the result contained in attribute (in bytes).</param>
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
        ///                 <paramref name="attribute" /> is too big  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InsufficientBuffer" />
        ///             </term>
        ///             <description>Reader buffer <paramref name="attribute" /> not large enough  - indicates an error in the PC/SC class library. (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        ///     <para>The method will return <see cref="F:PCSC.SCardError.InsufficientBuffer" /> if <paramref name="attribute" /> is
        ///         <see langword="null" /> or if the buffer size is not sufficient.
        ///         <paramref name="attributeBufferLength" /> contains the required amount of bytes (size).</para>
        ///     <para>If the method returned with <see cref="F:PCSC.SCardError.Success" /> then <paramref name="attributeBufferLength" /> contains the exact size of the result in
        ///         <paramref
        ///             name="attribute" />
        ///         .</para>
        ///     <para>For an example please see <see cref="M:PCSC.ISCardReader.GetAttrib(PCSC.SCardAttr,System.Byte[]@)" />.</para>
        /// </returns>
        /// <remarks>This method calls the API function SCardGetAttrib().</remarks>
        public SCardError GetAttrib(IntPtr attributeId, byte[] attribute, out int attributeBufferLength) {
            return Platform.Lib.GetAttrib(
                _cardHandle,
                attributeId,
                attribute,
                out attributeBufferLength);
        }

        /// <summary>Set an attribute of the IFD Handler.</summary>
        /// <param name="attributeId">Identifier for the attribute to set.</param>
        /// <param name="attribute">Buffer that contains the new value of the attribute.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Value</term>
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
        ///             <description>attribute is too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        /// <remarks>
        ///     <para>The list of attributes you can set depends on the IFD handler you are using.</para>
        ///     <para>This method calls the API function SCardSetAttrib().</para>
        /// </remarks>
        public SCardError SetAttrib(SCardAttr attributeId, byte[] attribute) {
            return SetAttrib((IntPtr) attributeId, attribute, attribute.Length);
        }

        /// <summary>Set an attribute of the IFD Handler.</summary>
        /// <param name="attributeId">Identifier for the attribute to set.</param>
        /// <param name="attribute">Buffer that contains the new value of the attribute.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Column</term>
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
        ///             <description>attribute is too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        /// <remarks>
        ///     <para>The list of attributes you can set depends on the IFD handler you are using.</para>
        ///     <para>This method calls the API function SCardSetAttrib().</para>
        /// </remarks>
        public SCardError SetAttrib(IntPtr attributeId, byte[] attribute) {
            return SetAttrib(attributeId, attribute, attribute.Length);
        }

        /// <summary>Set an attribute of the IFD Handler.</summary>
        /// <param name="attributeId">Identifier for the attribute to set.</param>
        /// <param name="attribute">Buffer that contains the new value of the attribute.</param>
        /// <param name="attributeBufferLength">Length of the <paramref name="attribute" /> buffer in bytes.</param>
        /// <returns><list type="table">
        ///         <listheader>
        ///             <term>Column</term>
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
        ///             <description>attribute is too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        /// <remarks>
        ///     <para>The list of attributes you can set depends on the IFD handler you are using.</para>
        ///     <para>This method calls the API function SCardSetAttrib().</para>
        /// </remarks>
        public SCardError SetAttrib(SCardAttr attributeId, byte[] attribute, int attributeBufferLength) {
            return SetAttrib((IntPtr) attributeId, attribute, attributeBufferLength);
        }

        /// <summary>Set an attribute of the IFD Handler.</summary>
        /// <param name="attributeId">Identifier for the attribute to set.</param>
        /// <param name="attribute">Buffer that contains the new value of the attribute.</param>
        /// <param name="attributeBufferLength">Length of the <paramref name="attribute" /> buffer in bytes.</param>
        /// <returns>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Column</term>
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
        ///             <description>attribute is too big (SCARD_E_INSUFFICIENT_BUFFER)</description>
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
        ///             <description>A parameter is <see langword="null" /> and should not (SCARD_E_INVALID_PARAMETER)</description>
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
        /// <remarks>
        ///     <para>The list of attributes you can set depends on the IFD handler you are using.</para>
        ///     <para>This method calls the API function SCardSetAttrib().</para>
        /// </remarks>
        public SCardError SetAttrib(IntPtr attributeId, byte[] attribute, int attributeBufferLength) {
            return Platform.Lib.SetAttrib(
                _cardHandle,
                attributeId,
                attribute,
                attributeBufferLength);
        }

        /// <summary>
        /// This will disconnect the reader if it is currently connected using <see cref="SCardReaderDisposition.Leave"/>.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This will disconnect the reader if it is currently connected and <paramref name="disposing"/> is <see langword="true" /> using <see cref="SCardReaderDisposition.Leave"/>.
        /// </summary>
        /// <param name="disposing"><see langword="true" /> the reader will be disconnected. <see langword="false"/> the reader will not be disconnected and <see cref="CardHandle"/> remains valid.</param>
        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (_cardHandle != IntPtr.Zero) {
                Disconnect(SCardReaderDisposition.Leave);
            }
        }
    }
}
