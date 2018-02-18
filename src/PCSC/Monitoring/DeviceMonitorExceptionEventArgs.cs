using System;

namespace PCSC.Monitoring
{
    /// <summary>
    /// Information about a device monitor exception
    /// </summary>
    public class DeviceMonitorExceptionEventArgs : EventArgs
    {
        /// <summary>
        /// Exception that occurred in device monitor thread
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Creates an instance
        /// </summary>
        /// <param name="exception">Exception that occurred in monitoring thread</param>
        public DeviceMonitorExceptionEventArgs(Exception exception) {
            Exception = exception;
        }
    }
}
