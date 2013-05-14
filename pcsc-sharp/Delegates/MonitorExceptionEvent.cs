namespace PCSC
{
    /// <summary>An PC/SC error occurred during monitoring.</summary>
    /// <param name="sender">The <see cref="T:PCSC.SCardMonitor" /> sender object.</param>
    /// <param name="exception">An exception containting the PC/SC error code returned from the native library.</param>
    public delegate void MonitorExceptionEvent(object sender, PCSCException exception);
}