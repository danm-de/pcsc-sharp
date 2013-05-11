using System;

namespace PCSC.Iso7816
{
    public class ResponseApdu : Apdu, ICloneable
    {
        private byte[] _response;
        private int _length;

        private ResponseApdu() {}

        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol) {
            _response = response;
            
            if (response != null) {
                _length = response.Length;
            }

            Case = isoCase;
            Protocol = protocol;
        }

        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol, bool copy) {
            Case = isoCase;
            Protocol = protocol;

            if (response == null) {
                return;
            }

            if (copy) {
                _length = response.Length;
                _response = new byte[_length];
                Array.Copy(response, _response, _length);
            } else {
                _response = response;
                _length = response.Length;
            }
        }

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol protocol) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            _response = response;
            _length = length;
            Case = isoCase;
            Protocol = protocol;
        }

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol protocol, bool copy) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            if (copy) {
                if (response != null) {
                    _response = new byte[length];
                    Array.Copy(response, _response, length);
                }
            } else {
                _response = response;
            }
            _length = length;
            Case = isoCase;
            Protocol = protocol;
        }

        public bool HasData {
            get {
                return _response != null && _response.Length > 2 && _length > 2;
            }
        }

        public override bool IsValid {
            get {
                if (_response == null) {
                    return false;
                }
                return _response.Length >= 2 && _length >= 2;
            }
        }

        public byte SW1 {
            get {
                if (_response == null || _response.Length < _length || _length <= 1) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return _response[_length - 2];
            }
        }
        
        public byte SW2 {
            get {
                if (_response == null || _response.Length < _length || _length <= 0) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return _response[_length - 1];
            }
        }
        
        public int StatusWord {
            get { return (SW1 << 8) | SW2; }
        }

        public int Length {
            get { return _length; }
        }

        public int DataSize {
            get {
                if (_response == null || _response.Length <= 2 || _length <= 2) {
                    return 0;
                }
                return _length - 2;
            }
        }
        
        public byte[] FullApdu {
            get { return _response; }
        }

        public byte[] GetData() {
            if (_response == null) {
                throw new InvalidApduException("The response APDU is invalid.");
            }

            if (_response.Length <= 2 ||
                _length <= 2) {
                return null;
            }

            var tmp = new byte[_length - 2];
            Array.Copy(_response, tmp, _length - 2);
            return tmp;
        }

        public override byte[] ToArray() {
            if (_response == null) {
                return null;
            }
            
            if (_response.Length < _length) {
                throw new InvalidApduException("The response APDU is invalid.");
            }
            
            var tmp = new byte[_length];
            Array.Copy(_response, tmp, _length);
            return tmp;
        }

        public virtual object Clone() {
            var tmp = new ResponseApdu {
                _response = _response, 
                _length = _length
            };
            return tmp;
        }
    }
}