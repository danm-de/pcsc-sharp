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

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol proto) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response != null && response.Length < length)) {
                throw new ArgumentOutOfRangeException("length");
            }

            this.response = response;
            this.length = length;
            this.isocase = isoCase;
            this.proto = proto;
        }

        public ResponseApdu(byte[] response, int length, IsoCase isoCase, SCardProtocol proto, bool copy) {
            if (length < 0 ||
                (response == null && length > 0) ||
                (response.Length < length)) {
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
            this.isocase = isoCase;
            this.proto = proto;
        }

        public bool HasData {
            get {
                if (response != null &&
                    response.Length > 2 &&
                    length > 2) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        public override bool IsValid {
            get {
                if (response == null) {
                    return false;
                }
                if (response.Length < 2 || length < 2) // a valid APDU response contains SW1 and SW2
                {
                    return false;
                }
                return true;
            }
        }

        public byte SW1 {
            get {
                if (response != null &&
                    response.Length > 1 &&
                    length > 1) {
                    return response[length - 2];
                } else {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
            }
        }
        public byte SW2 {
            get {
                if (response != null &&
                    response.Length > 0 &&
                    length > 0) {
                    return response[length - 1];
                } else {
                    throw new InvalidApduException("The response APDU is invalid.");
                }
            }
        }
        public int StatusWord {
            get { return (((int) SW1) << 8) | (int) SW2; }
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

        public int DataSize {
            get {
                if (response == null ||
                    response.Length <= 2 ||
                    length <= 2) {
                    return 0;
                } else {
                    return length - 2;
                }
            }
        }
        public byte[] FullApdu {
            get { return response; }
        }

        public override byte[] ToArray() {
            if (response == null) {
                return null;
            }
            if (response.Length <= length) {
                var tmp = new byte[length];
                Array.Copy(response, tmp, length);
                return tmp;
            } else {
                throw new InvalidApduException("The response APDU is null or invalid.");
            }
        }

        public virtual object Clone() {
            var tmp = new ResponseApdu();
            tmp.response = response;
            tmp.length = length;
            return tmp;
        }
    }
}