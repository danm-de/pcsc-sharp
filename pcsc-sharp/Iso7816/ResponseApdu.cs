using System;

namespace PCSC.Iso7816
{
    /* 
     *      Case    Command data    Expected response data
     *      ==============================================
     *      1       No data         No data
     *      2       No data         Data
     *      3       Data            No data
     *      4       Data            Data 
     */
    public class ResponseApdu : Apdu, ICloneable
    {
        protected byte[] response;
        protected int length;

        private ResponseApdu() {}

        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol) {
            this.response = response;
            
            if (response != null) {
                length = response.Length;
            }

            isocase = isoCase;
            proto = protocol;
        }

        public ResponseApdu(byte[] response, IsoCase isoCase, SCardProtocol protocol, bool copy) {
            isocase = isoCase;
            proto = protocol;

            if (response == null) {
                return;
            }

            if (copy) {
                length = response.Length;
                this.response = new byte[length];
                Array.Copy(response, this.response, length);
            } else {
                this.response = response;
                length = response.Length;
            }
        }

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol protocol) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            this.response = response;
            this.length = length;
            isocase = isoCase;
            proto = protocol;
        }

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol protocol, bool copy) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            if (copy) {
                if (response != null) {
                    this.response = new byte[length];
                    Array.Copy(response, this.response, length);
                }
            } else {
                this.response = response;
            }
            this.length = length;
            isocase = isoCase;
            proto = protocol;
        }

        public bool HasData {
            get {
                return response != null && response.Length > 2 && length > 2;
            }
        }

        public override bool IsValid {
            get {
                if (response == null) {
                    return false;
                }
                return response.Length >= 2 && length >= 2;
            }
        }

        public byte SW1 {
            get {
                if (response == null || response.Length < length || length <= 1) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return response[length - 2];
            }
        }
        
        public byte SW2 {
            get {
                if (response == null || response.Length < length || length <= 0) {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
                return response[length - 1];
            }
        }
        
        public int StatusWord {
            get { return (SW1 << 8) | SW2; }
        }

        public int Length {
            get { return length; }
        }

        public int DataSize {
            get {
                if (response == null || response.Length <= 2 || length <= 2) {
                    return 0;
                }
                return length - 2;
            }
        }
        
        public byte[] FullApdu {
            get { return response; }
        }

        public byte[] GetData() {
            if (response == null) {
                throw new InvalidApduException("The response APDU is invalid.");
            }

            if (response.Length <= 2 ||
                length <= 2) {
                return null;
            }

            var tmp = new byte[length - 2];
            Array.Copy(response, tmp, length - 2);
            return tmp;
        }

        public override byte[] ToArray() {
            if (response == null) {
                return null;
            }
            
            if (response.Length < length) {
                throw new InvalidApduException("The response APDU is invalid.");
            }
            
            var tmp = new byte[length];
            Array.Copy(response, tmp, length);
            return tmp;
        }

        public virtual object Clone() {
            var tmp = new ResponseApdu {
                response = response, 
                length = length
            };
            return tmp;
        }
    }
}