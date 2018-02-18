namespace PCSC
{
    /// <summary>Information about a smart card reader status change.</summary>
    public class StatusChangeEventArgs : CardEventArgs
    {
        /// <summary>The reader's status before this event.</summary>
        /// <remarks>
        ///     <para>Is a bit mask containing one or more of the following values:</para>
        ///     <list type="table">
        ///         <listheader><term>State</term><description>Description</description></listheader>
        ///         <item><term><see cref="F:PCSC.SCRState.Unaware" /></term><description>The application is unaware of the current state, and would like to know. The use of this value results in an immediate return from state transition monitoring services. This is represented by all bits set to zero.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Ignore" /></term><description>This reader should be ignored</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Changed" /></term><description>There is a difference between the state believed by the application, and the state known by the resource manager. When this bit is set, the application may assume a significant state change has occurred on this reader.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Unknown" /></term><description>The given reader name is not recognized by the resource manager. If this bit is set, then <see cref="F:PCSC.SCRState.Changed" /> and <see cref="F:PCSC.SCRState.Ignore" /> will also be set</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Unavailable" /></term><description>The actual state of this reader is not available. If this bit is set, then all the following bits are clear.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Empty" /></term><description>There is no card in the reader. If this bit is set, all the following bits will be clear</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Present" /></term><description>There is a card in the reader</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Exclusive" /></term><description>The card in the reader is allocated for exclusive use by another application. If this bit is set, <see cref="F:PCSC.SCRState.Present" /> will also be set.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.InUse" /></term><description>The card in the reader is in use by one or more other applications, but may be connected to in shared mode. If this bit is set, <see cref="F:PCSC.SCRState.Present" /> will also be set.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Mute" /></term><description>There is an unresponsive card in the reader.</description></item>
        ///     </list>
        /// </remarks>
        public SCRState LastState { get; private set; }

        /// <summary>The new status of this reader.</summary>
        /// <remarks>
        ///     <para>Is a bit mask containing one or more of the following values:</para>
        ///     <list type="table">
        ///         <listheader><term>State</term><description>Description</description></listheader>
        ///         <item><term><see cref="F:PCSC.SCRState.Unaware" /></term><description>The application is unaware of the current state, and would like to know. The use of this value results in an immediate return from state transition monitoring services. This is represented by all bits set to zero.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Ignore" /></term><description>This reader should be ignored</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Changed" /></term><description>There is a difference between the state believed by the application, and the state known by the resource manager. When this bit is set, the application may assume a significant state change has occurred on this reader.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Unknown" /></term><description>The given reader name is not recognized by the resource manager. If this bit is set, then <see cref="F:PCSC.SCRState.Changed" /> and <see cref="F:PCSC.SCRState.Ignore" /> will also be set</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Unavailable" /></term><description>The actual state of this reader is not available. If this bit is set, then all the following bits are clear.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Empty" /></term><description>There is no card in the reader. If this bit is set, all the following bits will be clear</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Present" /></term><description>There is a card in the reader</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Exclusive" /></term><description>The card in the reader is allocated for exclusive use by another application. If this bit is set, <see cref="F:PCSC.SCRState.Present" /> will also be set.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.InUse" /></term><description>The card in the reader is in use by one or more other applications, but may be connected to in shared mode. If this bit is set, <see cref="F:PCSC.SCRState.Present" /> will also be set.</description></item>
        ///         <item><term><see cref="F:PCSC.SCRState.Mute" /></term><description>There is an unresponsive card in the reader.</description></item>
        ///     </list>
        /// </remarks>
        public SCRState NewState { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangeEventArgs"/> class.
        /// </summary>
        public StatusChangeEventArgs() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="StatusChangeEventArgs"/> class.
        /// </summary>
        /// <param name="readerName">Name of the reader.</param>
        /// <param name="lastState">The last state.</param>
        /// <param name="newState">The new state.</param>
        /// <param name="atr">The card's ATR.</param>
        public StatusChangeEventArgs(string readerName, SCRState lastState, SCRState newState, byte[] atr)
            : base(readerName, atr) {
            LastState = lastState;
            NewState = newState;
        }
    }
}