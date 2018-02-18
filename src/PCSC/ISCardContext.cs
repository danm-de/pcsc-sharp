using System;

namespace PCSC
{
    /// <summary>An application context to the PC/SC Resource Manager.</summary>
    /// <remarks>Each thread of an application shall use its own context.</remarks>
    public interface ISCardContext : IDisposable
    {
        /// <summary>Creates an Application Context to the PC/SC Resource Manager.</summary>
        /// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
        /// <remarks>
        ///     <para>Creates an Application Context for a client. This must be the first WinSCard function called in a PC/SC application. Each thread of an application shall use its own <see cref="T:PCSC.SCardContext" />.</para>
        ///     <para>This method calls the API function SCardEstablishContext().</para>
        /// </remarks>
        void Establish(SCardScope scope);

        /// <summary>Destroys a communication context to the PC/SC Resource Manager and frees unmanaged resources.</summary>
        /// <remarks>
        ///     <para>This must be the last method called in a PC/SC application. <see cref="SCardContext.Dispose()"/> calls this method silently.</para>
        ///     <para>This method calls the API function SCardReleaseContext().</para>
        /// </remarks>
        void Release();

        /// <summary>Checks the validity of the current context.</summary>
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
        ///             <description>The context is valid. (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
        ///             </term>
        ///             <description>The context is invalid. (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>Call this function to determine whether a smart card context handle is still valid. After a smart card context handle has been set by <see cref="M:PCSC.SCardContext.Establish(PCSC.SCardScope)" />, it may become not valid if the resource manager service has been shut down.</para>
        ///     <para>This method calls the API function SCardIsValidContext().</para>
        /// </remarks>
        SCardError CheckValidity();

        /// <summary>Checks the validity of the current context.</summary>
        /// <returns><see langword="true" /> if the context is valid.</returns>
        /// <remarks>Call this function to determine whether a smart card context handle is still valid. After a smart card context handle has been set by <see cref="M:PCSC.SCardContext.Establish(PCSC.SCardScope)" />, it may become not valid if the resource manager service has been shut down.
        /// </remarks>
        bool IsValid();

        /// <summary>Re-Establishes an Application Context to the PC/SC Resource Manager with the last used <see cref="T:PCSC.SCardScope" />.</summary>
        /// <remarks>This method must not be called before <see cref="SCardContext.Establish" /></remarks>
        void ReEstablish();

        /// <summary>Returns a list of currently available readers on the system.</summary>
        /// <param name="groups">List of groups to list readers.</param>
        /// <returns>An array of <see cref="T:System.String" />s containing all Smart Card readers found by the PC/SC Resource Manager.</returns>
        /// <remarks>
        ///     <para>Groups are not used on Linux/UNIX machines using the PC/SC Lite daemon.</para>
        ///     <para>This method calls the API function SCardListReaders().</para>
        /// </remarks>
        string[] GetReaders(string[] groups);

        /// <summary>Returns a list of currently available readers on the system.</summary>
        /// <returns>An array of <see cref="T:System.String" />s containing all Smart Card readers found by the PC/SC Resource Manager.</returns>
        /// <remarks>
        ///     <para>This method calls the API function SCardListReaders().</para>
        /// </remarks>
        string[] GetReaders();

        /// <summary>Returns a list of currently available reader groups on the system. </summary>
        /// <returns>An array of <see cref="T:System.String" />s containing all Smart Card reader groups found by the PC/SC Resource Manager.</returns>
        /// <remarks>
        ///     <para>This method calls the API function SCardListReaderGroups().</para>
        /// </remarks>
        string[] GetReaderGroups();

        /// <summary>Returns the current reader status.</summary>
        /// <param name="readerName">The requested reader.</param>
        /// <returns>A <see cref="T:PCSC.SCardReaderState" /> that contains the current reader status.</returns>
        /// <remarks>
        ///     <para>This method uses the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> method with a timeout of zero.</para>
        /// </remarks>
        SCardReaderState GetReaderStatus(string readerName);

        /// <summary>Returns the current reader status of all requested readers.</summary>
        /// <param name="readerNames">Requested reader names.</param>
        /// <returns>An array of <see cref="T:PCSC.SCardReaderState" />s that contains the current reader status of each requested reader.</returns>
        /// <remarks>
        ///     <para>This method uses the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> method with a timeout of zero.</para>
        /// </remarks>
        SCardReaderState[] GetReaderStatus(string[] readerNames);

        /// <summary>Blocks execution until the current availability of the cards in a specific set of readers changes.</summary>
        /// <param name="timeout">Maximum waiting time (in milliseconds) for status change, zero or <see cref="SCardContext.INFINITE" /> for infinite.</param>
        /// <param name="readerStates">Structures of readers with current states.</param>
        /// <returns>
        ///     <para>A <see cref="F:PCSC.SCardError.Success" /> indicating an error or the success.The caller receives status changes through the <see cref="F:PCSC.SCardError.NoService" /> array.</para>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Return value</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>Successful (SCARD_S_SUCCESS)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>Server is not running (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="readerStates" /> is invalid or <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>Invalid States, reader name, etc (SCARD_E_INVALID_VALUE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.Timeout" />
        ///             </term>
        ///             <description>Invalid context (SCARD_E_INVALID_HANDLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.ReaderUnavailable" />
        ///             </term>
        ///             <description>The reader is unavailable (SCARD_E_READER_UNAVAILABLE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="P:PCSC.SCardReaderState.EventState" />
        ///             </term>
        ///             <description>The user-specified timeout value has expired (SCARD_E_TIMEOUT)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>This method receives a structure or list of structures containing reader names. It then blocks for a change in state to occur for a maximum blocking time of <paramref name="timeout" /> or forever if <see cref="SCardContext.INFINITE" /> is used.</para>
        ///     <para>The new event state will be contained in <see cref="P:PCSC.SCardContext.Infinite" />. A status change might be a card insertion or removal event, a change in ATR, etc.</para>
        ///     <para>To wait for a reader event (reader added or removed) you may use the special reader name "\\?PnP?\Notification". If a reader event occurs the state of this reader will change and the bit <see cref="T:PCSC.SCardContext" /> will be set.</para>
        ///     <para>This method calls the API function SCardGetStatusChange().</para>
        /// </remarks>
        SCardError GetStatusChange(IntPtr timeout, SCardReaderState[] readerStates);

        /// <summary>Cancels all pending blocking requests on the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> method.</summary>
        /// <returns>
        ///     <list type="table">
        ///         <listheader><term>Return value</term><description>Description</description></listheader>
        ///         <item><term><see cref="F:PCSC.SCardError.Success" /></term><description>Successful (SCARD_S_SUCCESS)</description></item>
        ///         <item><term><see cref="F:PCSC.SCardError.InvalidHandle" /></term><description>Invalid context (SCARD_E_INVALID_HANDLE)</description></item>
        ///         <item><term><see cref="F:PCSC.SCardError.NoService" /></term><description>Server is not running (SCARD_E_NO_SERVICE)</description></item>
        ///         <item><term><see cref="F:PCSC.SCardError.CommunicationError" /></term><description>An internal communications error has been detected (SCARD_F_COMM_ERROR)</description></item>
        ///     </list>
        /// </returns>
        /// <remarks>This method calls the API function SCardCancel().</remarks>
        SCardError Cancel();

        /// <summary>A pointer (Application Context) that can be used for C API calls.</summary>
        /// <value>The returned Application Context handle. Is <see cref="IntPtr.Zero" /> if not context has been established.</value>
        /// <remarks>This is the Application Context handle that is returned when calling the C API function SCardEstablishContext().</remarks>
        IntPtr Handle { get; }

        /// <summary>Maximum ATR size.</summary>
        /// <value>
        ///     <list type="table">
        ///         <listheader><term>Platform</term><description>Maximum ATR size</description></listheader>
        ///         <item>
        ///             <term>Windows (Winscard.dll)</term>
        ///             <description>36</description>
        ///         </item>
        ///         <item>
        ///             <term>UNIX/Linux (PC/SClite)</term>
        ///             <description>33</description>
        ///         </item>
        ///     </list>
        /// </value>
        /// <remarks>Attention: Size depends on platform.</remarks>
        int MaxAtrSize { get; }

        /// <summary>Infinite timeout.</summary>
        /// <value>0xFFFFFFFF</value>
        IntPtr Infinite { get; }
    }
}
