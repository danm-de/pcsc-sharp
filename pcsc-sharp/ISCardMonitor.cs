using System;

namespace PCSC
{
    /// <summary>Monitors a Smart Card reader and triggers events on status changes.</summary>
    public interface ISCardMonitor : IDisposable
    {
        /// <summary>A general reader status change.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// // Point the callback function(s) to the pre-defined method MyStatusChangedMethod.
        /// monitor.StatusChanged += new StatusChangeEvent(MyStatusChangedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        event StatusChangeEvent StatusChanged;
        /// <summary>A new card has been inserted.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.CardInserted += new CardInsertedEvent(MyCardInsertedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        event CardInsertedEvent CardInserted;
        /// <summary>A card has been removed.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.CardRemoved += new CardRemovedEvent(MyCardRemovedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        event CardRemovedEvent CardRemoved;
        /// <summary>The monitor object has been initialized.</summary>
        /// <remarks>
        ///     <para>This event appears only once for each reader after calling <see cref="SCardMonitor.Start(string)" /> or <see cref="SCardMonitor.Start(string[])" />.</para>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.Initialized += new CardInitializedEvent(MyCardInitializedMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        event CardInitializedEvent Initialized;
        /// <summary>An PC/SC error occurred during monitoring.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// monitor.MonitorException += new MonitorExceptionEvent(MyMonitorExceptionMethod);
        /// 
        /// // Start to monitor the reader
        /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
        ///   </code>
        ///     </example>
        /// </remarks>
        event MonitorExceptionEvent MonitorException;
        /// <summary>All readers that are currently being monitored.</summary>
        /// <value>A <see cref="T:System.String" /> array of reader names. <see langword="null" /> if no readers is being monitored.</value>
        string[] ReaderNames { get; }
        /// <summary>Indicates if there are readers currently monitored.</summary>
        /// <value>
        ///     <list type="table">
        ///         <listheader>
        ///             <term>Value</term>
        ///             <description>Description</description>
        ///         </listheader>
        ///         <item>
        ///             <term>
        ///                 <see langword="true" />
        ///             </term>
        ///             <description>Monitoring process ongoing.</description>
        ///         </item>
        ///         <item>
        ///             <term>
        ///                 <see langword="false" />
        ///             </term>
        ///             <description>No monitoring.</description>
        ///         </item>
        ///     </list>
        /// </value>
        bool Monitoring { get; }
        /// <summary>The number of readers that currently being monitored.</summary>
        /// <value>Return 0 if no reader is being monitored.</value>
        int ReaderCount { get; }

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        IntPtr GetCurrentStateValue(int index);

        /// <summary>Returns the current state of a reader that is currently being monitored.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>The current state of reader with index number <paramref name="index" />.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        SCRState GetCurrentState(int index);

        /// <summary>Returns the reader name of a given <paramref name="index" />.</summary>
        /// <param name="index">The number of the desired reader. The index must be between 0 and (<see cref="P:PCSC.SCardMonitor.ReaderCount" /> - 1).</param>
        /// <returns>A reader name.</returns>
        /// <remarks>This method will throw an <see cref="T:System.ArgumentOutOfRangeException" /> if the specified <paramref name="index" /> is invalid. You can enumerate all readers currently monitored with the <see cref="P:PCSC.SCardMonitor.ReaderNames" /> property.</remarks>
        /// <exception cref="ArgumentOutOfRangeException">If the specified <paramref name="index" /> is invalid.</exception>
        string GetReaderName(int index);

        /// <summary>Cancels the monitoring of all readers that are currently being monitored.</summary>
        /// <remarks>This will end the monitoring. The method calls the <see cref="ISCardContext.Cancel()" /> method of its Application Context to the PC/SC Resource Manager.</remarks>
        void Cancel();

        /// <param name="readerName">The Smart Card reader that shall be monitored.</param>
        /// <summary>Starts to monitor a single Smart Card reader for status changes.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// // Create a new monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// // Start to monitor a single reader.
        /// monitor.Start("OMNIKEY CardMan 5x21 00 00");
        ///   </code>
        ///     </example>
        ///     <para>Do not forget to register for at least one event:
        ///         <list type="table">
        ///             <listheader><term>Event</term><description>Description</description></listheader>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardInserted" /></term><description>A new card has been inserted.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardRemoved" /></term><description>A card has been removed.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.Initialized" /></term><description>Initial status.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.StatusChanged" /></term><description>A general status change.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.MonitorException" /></term><description>An error occurred.</description></item>
        ///         </list></para>
        /// </remarks>
        void Start(string readerName);

        /// <param name="readerNames">A <see cref="T:System.String" /> array of reader names that shall be monitored.</param>
        /// <summary>Starts to monitor a range Smart Card readers for status changes.</summary>
        /// <remarks>
        ///     <example>
        ///         <code lang="C#">
        /// string [] readerNames;
        /// using (var ctx = new SCardContext()) {
        ///     ctx.Establish(SCardScope.System);
        ///     // Retrieve the names of all installed readers.
        ///     readerNames = ctx.GetReaders();
        ///     ctx.Release();
        /// }
        /// 
        /// // Create a new monitor object with its own PC/SC context.
        /// var monitor = new SCardMonitor(
        /// 	new SCardContext(),
        /// 	SCardScope.System,
        ///     true);
        /// 
        /// foreach (string reader in readerNames) {
        /// 	Console.WriteLine("Start monitoring for reader {0}.", reader);
        /// }
        ///         
        /// // Start monitoring multiple readers.
        /// monitor.Start(readerNames);
        /// </code>
        ///     </example>
        ///     <para>Do not forget to register for at least one event:
        ///         <list type="table">
        ///             <listheader><term>Event</term><description>Description</description></listheader>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardInserted" /></term><description>A new card has been inserted.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.CardRemoved" /></term><description>A card has been removed.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.Initialized" /></term><description>Initial status.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.StatusChanged" /></term><description>A general status change.</description></item>
        ///             <item><term><see cref="E:PCSC.SCardMonitor.MonitorException" /></term><description>An error occurred.</description></item>
        ///         </list></para>
        /// </remarks>
        void Start(string[] readerNames);
    }
}