using System;
using System.Runtime.InteropServices;
using PCSC.Interop;

namespace PCSC
{
    /// <summary>Manages an application context to the PC/SC Resource Manager.</summary>
    /// <remarks>Each thread of an application shall use its own SCardContext.</remarks>
    public class SCardContext : ISCardContext
    {
        private bool _hasContext;
        private SCardScope _lastScope;
        private IntPtr _contextPtr;

        /// <summary>
        /// Destroys application context to the PC/SC Resource Manager.
        /// </summary>
        ~SCardContext() {
            Dispose(false);
        }

		/// <summary>Creates an Application Context to the PC/SC Resource Manager.</summary>
		/// <param name="scope">Scope of the establishment. This can either be a local or remote connection.</param>
		/// <remarks>
		///     <para>Creates an Application Context for a client. This must be the first WinSCard function called in a PC/SC application. Each thread of an application shall use its own <see cref="T:PCSC.SCardContext" />.</para>
		///     <para>This method calls the API function SCardEstablishContext().</para>
		///     <example>
		///         <code lang="C#">
		/// var context = new SCardContext();
		/// context.Establish(SCardScope.System);
		///   </code>
		///     </example>
		/// </remarks>
		/// <exception cref="InvalidScopeTypeException">If an invalid scope type has been passed</exception>
		/// <exception cref="NoServiceException">If the smart card service is not running</exception>
		public void Establish(SCardScope scope) {
            if (_hasContext && IsValid()) {
                Release();
            }

            IntPtr hContext;

            var rc = Platform.Lib.EstablishContext(
                scope,
                IntPtr.Zero,
                IntPtr.Zero,
                out hContext);

            switch (rc) {
                case SCardError.Success:
                    _contextPtr = hContext;
                    _lastScope = scope;
                    _hasContext = true;
                    break;
                case SCardError.InvalidValue:
                    throw new InvalidScopeTypeException(rc, "Invalid scope type passed");
				default:
		            rc.Throw();
		            break;
            }
        }

        /// <summary>Destroys a communication context to the PC/SC Resource Manager and frees unmanaged resources.</summary>
        /// <remarks>
        ///     <para>This must be the last method called in a PC/SC application. <see cref="Dispose()"/> calls this method silently.</para>
        ///     <para>This method calls the API function SCardReleaseContext().</para>
        ///     <example>
        ///         <code lang="C#">
        /// var context = new SCardContext();
        /// 
        /// // establish context
        /// context.Establish(SCardScope.System);
        /// 
        /// // release context
        /// context.Release();
        ///   </code>
        ///     </example>
        /// </remarks>
        public void Release() {
            if (!_hasContext) {
                throw new InvalidContextException(SCardError.UnknownError, "Context was not established");
            }

            var rc = Platform.Lib.ReleaseContext(_contextPtr);

            switch (rc) {
                case SCardError.Success:
                    _contextPtr = IntPtr.Zero;
                    _hasContext = false;
                    break;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(rc, "Invalid Context handle");
                default:
		            rc.Throw();
		            break;
            }
        }

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
        public SCardError CheckValidity() {
            return Platform.Lib.IsValidContext(_contextPtr);
        }

        /// <summary>Checks the validity of the current context.</summary>
        /// <returns><see langword="true" /> if the context is valid.</returns>
        /// <remarks>Call this function to determine whether a smart card context handle is still valid. After a smart card context handle has been set by <see cref="M:PCSC.SCardContext.Establish(PCSC.SCardScope)" />, it may become not valid if the resource manager service has been shut down.
        ///     <example>
        ///         <code lang="C#">
        /// using (var context = new SCardContext()) {
        ///     context.Establish(SCardScope.System);
        ///     Console.WriteLine("Context is valid? {0}", context.IsValid());
        /// }
        ///   </code>
        ///     </example>
        /// </remarks>
        public bool IsValid() {
            return CheckValidity() == SCardError.Success;
        }

        /// <summary>Re-Establishes an Application Context to the PC/SC Resource Manager with the last used <see cref="T:PCSC.SCardScope" />.</summary>
        /// <remarks>This method must not be called before <see cref="Establish(PCSC.SCardScope)" /></remarks>
        public void ReEstablish() {
            Establish(_lastScope);
        }

        /// <summary>Disposes a PC/SC application context.</summary>
        /// <remarks>If a context to the PC/SC Resource Manager is established, Dispose() will call the <see cref="Release()" /> method silently.</remarks>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Disposes a PC/SC application context.</summary>
        /// <param name="disposing">Ignored. If an application context to the PC/SC Resource Manager has been established it will call the <see cref="Release()" /> method.</param>
        protected virtual void Dispose(bool disposing) {
            // we must free unmanaged resources in order to avoid memleeks.
            if (_hasContext) {
                Release();
            }
        }

		/// <summary>Returns a list of currently available readers on the system.</summary>
		/// <param name="groups">List of groups to list readers.</param>
		/// <returns>An array of <see cref="T:System.String" />s containing all Smart Card readers found by the PC/SC Resource Manager.</returns>
		/// <remarks>
		///     <para>Groups are not used on Linux/UNIX machines using the PC/SC Lite daemon.</para>
		///     <para>This method calls the API function SCardListReaders().</para>
		///     <example>
		///         <code lang="C#">
		/// using (var context = new SCardContext()) {
		///     context.Establish(SCardScope.System);
		/// 
		///     // list all configured reader groups
		///     Console.WriteLine("\nCurrently configured readers groups: ");
		///     var groups = context.GetReaderGroups();
		///     foreach (string group in groups) {
		/// 	    Console.WriteLine("\t" + group);
		///     }
		/// 
		///     // list readers for each group
		///     foreach (string group in groups) {
		/// 	    Console.WriteLine("\nGroup " + group + " contains ");
		/// 	    foreach (string reader in context.GetReaders(new string[] {group})) {
		/// 		    Console.WriteLine("\t" + reader);
		///         }
		///     }
		/// }
		///   </code>
		///     </example>
		/// </remarks>
		/// <exception cref="ReaderUnavailableException">Specified reader is not currently available for use</exception>
		public string[] GetReaders(string[] groups) {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }

            string[] readers;
            var rc = Platform.Lib.ListReaders(
                _contextPtr,
                groups,
                out readers);

            switch (rc) {
                case SCardError.Success:
                    return readers;
				case SCardError.NoReadersAvailable:
		            return new string[0]; // Service running, no reader connected
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(rc, "Invalid Scope Handle");
                default:
		            rc.Throw();
		            return null;
            }
        }

		/// <summary>Returns a list of currently available readers on the system.</summary>
		/// <returns>An array of <see cref="T:System.String" />s containing all Smart Card readers found by the PC/SC Resource Manager.</returns>
		/// <remarks>
		///     <para>This method calls the API function SCardListReaders().</para>
		///     <example>
		///         <code lang="C#">
		/// using (var context = new SCardContext()) {
		///     context.Establish(SCardScope.System);
		/// 
		///     // list all (smart card) readers
		///     Console.WriteLine("Currently connected readers: ");
		///     var readers = context.GetReaders();
		///     foreach (string reader in readers) {
		/// 	    Console.WriteLine("\t" + reader);
		///     }
		/// }
		///   </code>
		///     </example>
		/// </remarks>
		/// <exception cref="ReaderUnavailableException">Specified reader is not currently available for use</exception>
		public string[] GetReaders() {
            return GetReaders(null);
        }

        /// <summary>Returns a list of currently available reader groups on the system. </summary>
        /// <returns>An array of <see cref="T:System.String" />s containing all Smart Card reader groups found by the PC/SC Resource Manager.</returns>
        /// <remarks>
        ///     <para>This method calls the API function SCardListReaderGroups().</para>
        ///     <example>
        ///         <code lang="C#">
        /// using (var context = new SCardContext()) {
        ///     context.Establish(SCardScope.System);
        /// 
        ///     // list all configured reader groups
        ///     Console.WriteLine("Currently configured readers groups: ");
        ///     var groups = context.GetReaderGroups();
        ///     foreach (string group in groups) {
        /// 	    Console.WriteLine("\t" + group);
        ///     }
        /// }
        ///   </code>
        ///     </example>
        /// </remarks>
        public string[] GetReaderGroups() {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }

            string[] groups;

            var sc = Platform.Lib.ListReaderGroups(
                _contextPtr,
                out groups);

            switch (sc) {
                case SCardError.Success:
                    return groups;
                case SCardError.InvalidHandle:
                    throw new InvalidContextException(sc, "Invalid Scope Handle");
				case SCardError.NoReadersAvailable:
		            return new string[0]; // Service running, no reader connected
				default:
		            sc.Throw();
		            return null;
            }
        }

        /// <summary>Returns the current reader status.</summary>
        /// <param name="readerName">The requested reader.</param>
        /// <returns>A <see cref="T:PCSC.SCardReaderState" /> that contains the current reader status.</returns>
        /// <remarks>
        ///     <para>This method uses the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> method with a timeout of zero.</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Retrieve the names of all installed readers.
        /// using (var ctx = new SCardContext()) {
        ///     ctx.Establish(SCardScope.System);
        ///     var readerNames = ctx.GetReaders();
        /// 
        ///     // Get the current status of the first reader.
        ///     var state = ctx.GetReaderStatus(readerNames[0]);
        /// 
        ///     Console.WriteLine("Reader: " + state.ReaderName);
        ///     Console.WriteLine("CurrentState: " + state.CurrentState 
        /// 	    + " EventState: " + state.EventState + "\n"
        /// 	    + "CurrentStateValue: " + state.CurrentStateValue 
        /// 	    + " EventStateValue: " + state.EventStateValue);
        ///     Console.WriteLine("UserData: " + state.UserData.ToString()
        /// 	    + " CardChangeEventCnt: " + state.CardChangeEventCnt);
        ///     Console.WriteLine("ATR: " + BitConverter.ToString(state.Atr));
        /// }
        ///   </code>
        ///     </example>
        /// </remarks>
        public SCardReaderState GetReaderStatus(string readerName) {
            var tmp = (readerName != null)
                ? new[] {readerName}
                : new string[0];

            return GetReaderStatus(tmp)[0];
        }

        /// <summary>Returns the current reader status of all requested readers.</summary>
        /// <param name="readerNames">Requested reader names.</param>
        /// <returns>An array of <see cref="T:PCSC.SCardReaderState" />s that contains the current reader status of each requested reader.</returns>
        /// <remarks>
        ///     <para>This method uses the <see cref="M:PCSC.SCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" /> method with a timeout of zero.</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Retrieve the names of all installed readers.
        /// using (var ctx = new SCardContext()) {
        ///     ctx.Establish(SCardScope.System);
        ///     var readerNames = ctx.GetReaders();
        /// 
        ///     // Get the current status for all readers.
        ///     var states = ctx.GetReaderStatus(readerNames);
        /// 
        ///     foreach (var state in states) {
        /// 	    Console.WriteLine("------------------------------");
        /// 	    Console.WriteLine("Reader: " + state.ReaderName);
        /// 	    Console.WriteLine("CurrentState: " + state.CurrentState 
        /// 		    + " EventState: " + state.EventState + "\n"
        /// 		    + "CurrentStateValue: " + state.CurrentStateValue 
        /// 		    + " EventStateValue: " + state.EventStateValue);
        /// 	    Console.WriteLine("UserData: " + state.UserData.ToString()
        /// 		    + " CardChangeEventCnt: " + state.CardChangeEventCnt);
        /// 	    Console.WriteLine("ATR: " + BitConverter.ToString(state.Atr));
        ///     }
        /// }
        ///   </code>
        ///     </example>
        /// </remarks>
        public SCardReaderState[] GetReaderStatus(string[] readerNames) {
            if (readerNames == null) {
                throw new ArgumentNullException(nameof(readerNames));
            }
            if (readerNames.Length == 0) {
                throw new ArgumentException("You must specify at least one reader.");
            }

            var states = new SCardReaderState[readerNames.Length];
            for (var i = 0; i < states.Length; i++) {
                states[i] = new SCardReaderState {
                    ReaderName = readerNames[i],
                    CurrentState = SCRState.Unaware
                };
            }

            GetStatusChange(IntPtr.Zero, states)
				.ThrowIfNotSuccess();

            return states;
        }

        /// <summary>Blocks execution until the current availability of the cards in a specific set of readers changes.</summary>
        /// <param name="timeout">Maximum waiting time (in milliseconds) for status change, zero or <see cref="INFINITE" /> for infinite.</param>
        /// <param name="readerStates">Structures of readers with current states.</param>
        /// <returns>
        ///     <para>A <see cref="T:PCSC.SCardError" /> indicating an error or the success.The caller receives status changes through the <see cref="T:PCSC.SCardReaderState" /> array.</para>
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
        ///                 <see cref="F:PCSC.SCardError.NoService" />
        ///             </term>
        ///             <description>Server is not running (SCARD_E_NO_SERVICE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidParameter" />
        ///             </term>
        ///             <description>
        ///                 <paramref name="readerStates" /> is invalid or <see langword="null" /> (SCARD_E_INVALID_PARAMETER)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidValue" />
        ///             </term>
        ///             <description>Invalid States, reader name, etc (SCARD_E_INVALID_VALUE)</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see cref="F:PCSC.SCardError.InvalidHandle" />
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
        ///                 <see cref="F:PCSC.SCardError.Timeout" />
        ///             </term>
        ///             <description>The user-specified timeout value has expired (SCARD_E_TIMEOUT)</description>
        ///         </item>
        ///     </list>
        /// </returns>
        /// <remarks>
        ///     <para>This method receives a structure or list of structures containing reader names. It then blocks for a change in state to occur for a maximum blocking time of <paramref name="timeout" /> or forever if <see cref="INFINITE" /> is used.</para>
        ///     <para>The new event state will be contained in <see cref="P:PCSC.SCardReaderState.EventState" />. A status change might be a card insertion or removal event, a change in ATR, etc.</para>
        ///     <para>To wait for a reader event (reader added or removed) you may use the special reader name "\\?PnP?\Notification". If a reader event occurs the state of this reader will change and the bit <see cref="F:PCSC.SCRState.Changed" /> will be set.</para>
        ///     <para>This method calls the API function SCardGetStatusChange().</para>
        /// </remarks>
        public SCardError GetStatusChange(IntPtr timeout, SCardReaderState[] readerStates) {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.InvalidHandle);
            }

            return Platform.Lib.GetStatusChange(_contextPtr, timeout, readerStates);
        }

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
        public SCardError Cancel() {
            if (_contextPtr.Equals(IntPtr.Zero)) {
                throw new InvalidContextException(SCardError.UnknownError, "Invalid connection context.");
            }

            var rc = Platform.Lib.Cancel(_contextPtr);
            return rc;
        }

        /// <summary>A pointer (Application Context) that can be used for C API calls.</summary>
        /// <value>The returned Application Context handle. Is <see cref="IntPtr.Zero" /> if not context has been established.</value>
        /// <remarks>This is the Application Context handle that is returned when calling the C API function SCardEstablishContext().</remarks>
        public IntPtr Handle => _contextPtr;

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
        public int MaxAtrSize => MAX_ATR_SIZE;

        /// <summary>Infinite timeout.</summary>
        /// <value>0xFFFFFFFF</value>
        public IntPtr Infinite => INFINITE;

        //// ReSharper disable InconsistentNaming
        
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
        public static int MAX_ATR_SIZE => Platform.Lib.MaxAtrSize;

        /// <summary>Infinite timeout.</summary>
        /// <value>0xFFFFFFFF</value>
        public static IntPtr INFINITE {
            get {
                // Hack to avoid Overflow exception on Windows 7 32bit
                if (Marshal.SizeOf(typeof(IntPtr)) == 4) {
                    return unchecked((IntPtr) (int) 0xFFFFFFFF);
                }
                return unchecked((IntPtr) 0xFFFFFFFF);
            }
        }

        //// ReSharper restore InconsistentNaming
    }
}