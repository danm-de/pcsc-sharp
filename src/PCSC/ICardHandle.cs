using System;

namespace PCSC
{
    /// <summary>
    /// Encapsulates a handle to a connected card / or a directly connected reader
    /// </summary>
    public interface ICardHandle : IDisposable
    {
        /// <summary>
        /// The card handle
        /// </summary>
        IntPtr Handle { get; }

        /// <summary>The connected reader's friendly name.</summary>
        /// <value>A human readable string of the reader name or <see langword="null" /> if the reader object is currently not connected.</value>
        string ReaderName { get; }

        /// <summary>The current mode of connection type: exclusive, shared or direct.</summary>
        SCardShareMode Mode { get; }

        /// <summary>The currently used protocol to communicate with the card.</summary>
        /// <value>
        ///     <see cref="SCardProtocol.Unset" /> if not connected.</value>
        SCardProtocol Protocol { get; }

        /// <summary>The current connection state of the reader.</summary>
        /// <value><see langword="true" /> if the reader is connected. Otherwise <see langword="false" />.</value>
        bool IsConnected { get; }

        /// <summary>Establishes a connection to the Smart Card reader.</summary>
        /// <param name="readerName">Reader name to connect to.</param>
        /// <param name="mode">Mode of connection type: exclusive or shared.
        ///     <list type="table">
        ///         <listheader><term>Value</term><description>Description</description></listheader>
        ///         <item><term><see cref="SCardShareMode.Shared" /></term><description>This application will allow others to share the reader. (SCARD_SHARE_SHARED)</description></item>
        ///         <item><term><see cref="SCardShareMode.Exclusive" /></term><description>This application will NOT allow others to share the reader. (SCARD_SHARE_EXCLUSIVE)</description></item>
        ///     </list>
        /// </param>
        /// <param name="preferredProtocol">Desired protocol use.</param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="preferredProtocol" />  is a bit mask of acceptable protocols for the connection. You can use (<see cref="SCardProtocol.T0" /> | <see cref="SCardProtocol.T1" />) if you do not have a preferred protocol.</para>
        ///     <para>This method calls the API function SCardConnect().</para>
        /// </remarks>
        void Connect(string readerName, SCardShareMode mode, SCardProtocol preferredProtocol);

        /// <summary>Terminates a connection made through <see cref="Connect(string,SCardShareMode,SCardProtocol)" />.</summary>
        /// <param name="disconnectExecution">Reader function to execute.</param>
        /// <remarks>
        ///     <para>This method calls the API function SCardDisconnect().</para>
        /// </remarks>
        void Disconnect(SCardReaderDisposition disconnectExecution);

        /// <summary>Reestablishes a connection to a reader that was previously connected to using
        ///     <see
        ///         cref="Connect(string,SCardShareMode,SCardProtocol)" />
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
    }
}
