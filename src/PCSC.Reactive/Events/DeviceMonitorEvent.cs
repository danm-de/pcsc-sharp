using System;
using System.Collections.Generic;

namespace PCSC.Reactive.Events
{
    /// <summary>
    /// Device monitor event
    /// </summary>
    public abstract class DeviceMonitorEvent
    {
        /// <summary>
        /// A list of affected smartcard reader names
        /// </summary>
        public abstract IEnumerable<string> Readers { get; }

        /// <summary>
        /// Original event arguments
        /// </summary>
        public abstract EventArgs EventArgs { get; }
    }
}
