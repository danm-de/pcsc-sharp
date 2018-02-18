using System;
using PCSC.Context;

namespace PCSC.Monitoring
{
    /// <summary>Monitors for attached and detached smartcard reader devices.</summary>
    public interface IDeviceMonitor : IDisposable
    {
        /// <summary>
        /// The monitor object has been initialized.
        /// </summary>
        event DeviceChangeEvent Initialized;

        /// <summary>
        /// New reader(s) have been attached and/or detached.
        /// </summary>
        event DeviceChangeEvent StatusChanged;

        /// <summary>An PC/SC error occurred during monitoring.</summary>
        event DeviceMonitorExceptionEvent MonitorException;

        /// <summary>
        /// Starts monitoring for device status changes
        /// </summary>
        void Start();

        /// <summary>Cancels the monitoring.</summary>
        /// <remarks>This will end the monitoring. The method calls the <see cref="ISCardContext.Cancel()" /> method of its Application Context to the PC/SC Resource Manager.</remarks>
        void Cancel();
    }
}