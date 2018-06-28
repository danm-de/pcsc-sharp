using System;
using System.Collections.Generic;
using PCSC.Monitoring;

namespace PCSC.Reactive.Events
{
    /// <summary>
    /// Smartcard reader devices have been detached
    /// </summary>
    public class ReadersDetached : DeviceMonitorEvent
    {
        /// <summary>
        /// Recently detached readers
        /// </summary>
        public override IEnumerable<string> Readers { get; }

        /// <summary>
        /// Original event arguments
        /// </summary>
        public override EventArgs EventArgs { get; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="e">Original event arguments</param>
        public ReadersDetached(DeviceChangeEventArgs e) {
            EventArgs = e;
            Readers = e.DetachedReaders;
        }
    }
}
