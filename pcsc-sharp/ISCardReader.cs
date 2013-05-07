using System;

namespace PCSC
{
    public interface ISCardReader
    {
        SCardError Connect(string name, SCardShareMode mode, SCardProtocol prefProto);
        SCardError Disconnect(SCardReaderDisposition discntExec);
        SCardError Reconnect(SCardShareMode mode, SCardProtocol prefProto, SCardReaderDisposition initExec);

        SCardError BeginTransaction();
        SCardError EndTransaction(SCardReaderDisposition disposition);

        SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, int sendBufLength, SCardPCI ioRecvPci, byte[] recvBuffer, ref int recvBufLength);
        SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, SCardPCI ioRecvPci, ref byte[] recvBuffer);
        SCardError Transmit(IntPtr pioSendPci, byte[] sendBuffer, ref byte[] recvBuffer);
        SCardError Transmit(SCardPCI ioSendPci, byte[] sendBuffer, SCardPCI ioRecvPci, ref byte[] recvBuffer);
        SCardError Transmit(byte[] sendBuffer, int sendBufferLength, byte[] recvBuffer, ref int recvBufferLength);
        SCardError Transmit(byte[] sendBuffer, byte[] recvBuffer, ref int recvBufferLength);
        SCardError Transmit(byte[] sendBuffer, ref byte[] recvBuffer);

        SCardError Control(IntPtr controlCode, byte[] sendBuffer, ref byte[] recvBuffer);

        SCardError Status(out string[] readerName, out SCardState state, out SCardProtocol protocol, out byte[] atr);

        SCardError GetAttrib(IntPtr dwAttrId, byte[] pbAttr, out int attrLen);
        SCardError GetAttrib(IntPtr attrId, out byte[] pbAttr);
        SCardError GetAttrib(SCardAttr attrId, byte[] pbAttr, out int attrLen);
        SCardError GetAttrib(SCardAttr attrId, out byte[] pbAttr);

        SCardError SetAttrib(IntPtr attr, byte[] pbAttr, int attrBufSize);
        SCardError SetAttrib(IntPtr attr, byte[] pbAttr);
        SCardError SetAttrib(SCardAttr attr, byte[] pbAttr, int attrBufSize);
        SCardError SetAttrib(SCardAttr attr, byte[] pbAttr);

        string ReaderName { get; }
        SCardContext CurrentContext { get; }
        SCardShareMode CurrentShareMode { get; }
        SCardProtocol ActiveProtocol { get; }
        IntPtr CardHandle { get; }
    }
}
