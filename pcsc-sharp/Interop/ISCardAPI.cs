using System;
using System.Text;

namespace PCSC.Interop
{
    /// <summary>
    /// Gives access to the system's smart card API
    /// </summary>
    internal interface ISCardAPI
    {
        IntPtr GetSymFromLib(string symName);

        int MaxAtrSize { get; }
        Encoding TextEncoding { get; set; }
        int CharSize { get; }

        SCardError EstablishContext(SCardScope dwScope, IntPtr pvReserved1, IntPtr pvReserved2, out IntPtr phContext);
        SCardError ReleaseContext(IntPtr hContext);
        SCardError IsValidContext(IntPtr hContext);
        
        SCardError ListReaders(IntPtr hContext, string[] groups, out string[] readers);
        SCardError ListReaderGroups(IntPtr hContext, out string[] groups);
        
        SCardError Connect(IntPtr hContext, string szReader, SCardShareMode dwShareMode, SCardProtocol dwPreferredProtocols, out IntPtr phCard, out SCardProtocol pdwActiveProtocol);
        SCardError Disconnect(IntPtr hCard, SCardReaderDisposition dwDisposition);
        SCardError Reconnect(IntPtr hCard, SCardShareMode dwShareMode, SCardProtocol dwPreferredProtocols, SCardReaderDisposition dwInitialization, out SCardProtocol pdwActiveProtocol);
        
        SCardError BeginTransaction(IntPtr hCard);
        SCardError EndTransaction(IntPtr hCard, SCardReaderDisposition dwDisposition);

        SCardError Transmit(IntPtr hCard, IntPtr pioSendPci, byte[] pbSendBuffer, IntPtr pioRecvPci, byte[] pbRecvBuffer, out int pcbRecvLength);
        SCardError Transmit(IntPtr hCard, IntPtr pioSendPci, byte[] pbSendBuffer, int pcbSendLength, IntPtr pioRecvPci, byte[] pbRecvBuffer, ref int pcbRecvLength);

        SCardError Control(IntPtr hCard, IntPtr dwControlCode, byte[] pbSendBuffer, byte[] pbRecvBuffer, out int lpBytesReturned);

        SCardError Status(IntPtr hCard, out string[] szReaderName, out IntPtr pdwState, out IntPtr pdwProtocol, out byte[] pbAtr);
        SCardError GetStatusChange(IntPtr hContext, IntPtr dwTimeout, SCardReaderState[] rgReaderStates);
        SCardError Cancel(IntPtr hContext);

        SCardError GetAttrib(IntPtr hCard, IntPtr dwAttrId, byte[] pbAttr, out int pcbAttrLen);
        SCardError SetAttrib(IntPtr hCard, IntPtr dwAttrId, byte[] pbAttr, int attrSize);
    }
}