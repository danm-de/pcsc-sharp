using System;
using System.Collections.Generic;
using PCSC.Monitoring;

namespace PCSC.Reactive.Events
{
    /// <summary>
    /// The device monitor has been initialized
    /// </summary>
    public class DeviceMonitorInitialized : DeviceMonitorEvent
    {
        /// <summary>
        /// Currently attached readers
        /// </summary>
        public override IEnumerable<string> Readers { get; }
        
        /// <summary>
        /// Original event arguments
        /// </summary>
        public override EventArgs EventArgs { get; }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="e">Original event args</param>
        public DeviceMonitorInitialized(DeviceChangeEventArgs e) {
            EventArgs = e;
            Readers = e.AllReaders;
        }
    }
}