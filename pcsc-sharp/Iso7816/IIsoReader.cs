using System;

namespace PCSC.Iso7816
{
    public interface IIsoReader : IDisposable
    {
        ISCardContext CurrentContext { get; }
        ISCardReader Reader { get; }
        string ReaderName { get; }
        SCardProtocol ActiveProtocol { get; }
        SCardShareMode CurrentShareMode { get; }
        int RetransmitWaitTime { get; set; }
        int MaxReceiveSize { get; }
        CommandApdu ConstructCommandApdu(IsoCase isoCase);
        void Connect(string readerName, SCardShareMode mode, SCardProtocol protocol);
        void Disconnect(SCardReaderDisposition disposition);
        Response Transmit(CommandApdu commandApdu);
    }
}