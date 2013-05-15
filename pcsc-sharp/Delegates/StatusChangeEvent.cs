namespace PCSC
{
    /// <summary>A general reader status change.</summary>
    /// <param name="sender">The <see cref="T:PCSC.ISCardMonitor" /> sender object.</param>
    /// <param name="e">Reader status information.</param>
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
    public delegate void StatusChangeEvent(object sender, StatusChangeEventArgs e);
}