using System;
using System.Collections.Generic;
using PCSC.Monitoring;

namespace PCSC.Reactive.Events
{
    /// <summary>
    /// Smartcard reader devices have been attached
    /// </summary>
    public class ReadersAttached : DeviceMonitorEvent
    {
        /// <summary>
        /// Recently attached readers
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
        public ReadersAttached(DeviceChangeEventArgs e) {
            EventArgs = e;
            Readers = e.AttachedReaders;
        }
    }
}
