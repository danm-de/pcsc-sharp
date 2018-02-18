namespace PCSC
{
    /// <summary>
    /// A smartcard device status change
    /// </summary>
    /// <param name="sender">The <see cref="T:PCSC.ISCardMonitor" /> sender object.</param>
    /// <param name="e">Reader device changes.</param>
    public delegate void DeviceChangeEvent(object sender, DeviceChangeEventArgs e);
}