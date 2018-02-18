namespace PCSC
{
    /// <summary>An PC/SC error occurred during device monitoring.</summary>
    /// <param name="sender">The <see cref="T:PCSC.SCardMonitor" /> sender object.</param>
    /// <param name="args">Argument that contains the exception.</param>
    public delegate void DeviceMonitorExceptionEvent(object sender, DeviceMonitorExceptionEventArgs args);
}