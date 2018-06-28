﻿namespace PCSC.Monitoring
{
    /// <summary>A card has been removed.</summary>
    /// <param name="sender">The <see cref="SCardMonitor" /> sender object.</param>
    /// <param name="e">Reader status information.</param>
    /// <remarks>
    ///     <example>
    ///         <code lang="C#">
    /// // Create a monitor object with its own PC/SC context.
    /// SCardMonitor monitor = new SCardMonitor(
    /// 	new SCardContext(),
    /// 	SCardScope.System);
    /// 
    /// // Point the callback function(s) to the pre-defined method MyCardRemovedMethod.
    /// monitor.CardRemoved += new CardRemovedEvent(MyCardRemovedMethod);
    /// 
    /// // Start to monitor the reader
    /// monitor.Start("OMNIKEY CardMan 5x21 00 01");
    ///   </code>
    ///     </example>
    /// </remarks>
    public delegate void CardRemovedEvent(object sender, CardStatusEventArgs e);
}
