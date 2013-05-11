using System;

namespace PCSC.Iso7816
{
    public class CommandApdu : Apdu, ICloneable
    {
        // see http://www.cardwerk.com/smartcards/smartcard_standard_Iso7816-4_5_basic_organizations.aspx

        private byte _cla, _ins, _p1, _p2;
        private int _lc, _le;
        private byte[] _data;

        public CommandApdu(IsoCase isoCase, SCardProtocol protocol) {
            Case = isoCase;
            Protocol = protocol;
        }

        public byte CLA {
            get { return _cla; }
            set { _cla = value; }
        }

        public ClassByte GetClassByteInfo() {
            return new ClassByte(_cla);
        }

        public InstructionByte GetInstructionByteInfo() {
            return new InstructionByte(_ins);
        }

        public byte INS {
            get { return _ins; }
            set { _ins = value; }
        }

        public InstructionCode Instruction {
            set { _ins = (byte) value; }
        }

        public byte P1 {
            get { return _p1; }
            set { _p1 = value; }
        }

        public byte P2 {
            get { return _p2; }
            set { _p2 = value; }
        }

        public int P1P2 {
            get { return (_p1 << 8) | _p2; }
            set {
                if (value < 0 || value > 0xFFFF) {
                    throw new ArgumentException(
                        "Must be a value between 0x00 and 0xFFFF.",
                        new OverflowException());
                }
                _p2 = (byte) (0xFF & value);
                _p1 = (byte) ((0xFF00 & value) >> 8);
            }
        }

        public byte[] Data {
            get { return _data; }
            set {
                switch (Case) {
                    case IsoCase.Case3Short:
                    case IsoCase.Case4Short:
                        if (value == null) {
                            throw new ArgumentNullException("Iso7816-4 " + Case +
                                " expects 1 to 255 bytes of data.");
                        }
                        if (value.Length > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " expects 1 to 255 bytes of data.");
                        }
                        _data = value;
                        _lc = _data.Length;
                        break;

                    case IsoCase.Case3Extended:
                    case IsoCase.Case4Extended:
                        if (value == null) {
                            throw new ArgumentNullException("Iso7816-4 " + Case +
                                " expects 1 to 65535 bytes of data.");
                        }
                        if (Protocol == SCardProtocol.T0 && value.Length > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " with protocol " + Protocol +
                                " accepts only 255 bytes of data.");
                        }
                        if (value.Length > 65535) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " expects 1 to 65535 bytes of data.");
                        }
                        _data = value;
                        _lc = _data.Length;
                        break;

                    default:
                        throw new ArgumentException("Iso7816-4 " + Case +
                            " does not expect any data in its APDU command.");
                }
            }
        }

        public int Lc {
            get { return _lc; }
        }

        public int P3 {
            get { return Le; }
            set { Le = value; }
        }

        public int Le {
            get { return _le; }
            set {
                // Inspired by the work from Nils Larsch (OpenSC)
                switch (Case) {
                    case IsoCase.Case2Short:
                        if (value < 0 || value > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 0 - 255 in Le.");
                        }
                        _le = value;
                        break;

                    case IsoCase.Case4Short:
                        if (Protocol == SCardProtocol.T0) {
                            throw new ArgumentException("Iso7816-4 " + Case +
                                " with protocol " + Protocol +
                                " requires data to be transferred by using GET RESPONSE.");
                        }
                        if (value < 0 || value > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 0 - 255 in Le.");
                        }
                        _le = value;
                        break;

                    case IsoCase.Case2Extended:
                        if (Protocol == SCardProtocol.T0) {
                            if (value < 0 || value > 255) {
                                throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                    " with protocol " + Protocol +
                                    " accepts only values from 0 - 255 in Le.");
                            }
                        }
                        if (value < 0 || value > 65535) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 0 - 65535 in Le.");
                        }
                        _le = value;
                        break;

                    case IsoCase.Case4Extended:
                        if (Protocol == SCardProtocol.T0) {
                            throw new ArgumentException("Iso7816-4 " + Case +
                                " with protocol " + Protocol +
                                " requires data to be transferred by using GET RESPONSE.");
                        }
                        if (value < 0 || value > 65535) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 0 - 65535 in Le.");
                        }
                        _le = value;
                        break;

                    default:
                        throw new ArgumentException("Iso7816-4 " + Case +
                            " does not expect any data fields in its return value and" +
                            " therefore has no bytes for Le.");
                }
            }
        }

        public int ExpectedResponseLength {
            /* ExpectedResponseLength = Le + SW1SW2.
             */
            get {
                switch (Case) {
                    case IsoCase.Case2Short:
                        if (_le == 0) {
                            return 256 + 2;
                        }

                        return _le + 2;

                    case IsoCase.Case4Short:
                        if (Protocol == SCardProtocol.T0) {
                            return 0 + 2;
                        }

                        if (_le == 0) {
                            return 256 + 2;
                        }

                        return _le + 2;

                    case IsoCase.Case2Extended:
                        if (Protocol == SCardProtocol.T0) {
                            if (_le == 0) {
                                return 256 + 2;
                            }
                            return _le + 2;
                        }

                        if (_le == 0) {
                            return 65536 + 2;
                        }

                        return _le + 2;

                    case IsoCase.Case4Extended:
                        if (Protocol == SCardProtocol.T0) {
                            return 0 + 2;
                        }

                        if (_le == 0) {
                            return 65536 + 2;
                        }

                        return _le + 2;

                    default:
                        return 0 + 2;
                }
            }
            set {
                /* SW1SW2 = 2 bytes
                 * Therefore we remove 2 bytes for SW1SW2.
                 */
                var datavalue = value - 2;
                switch (Case) {
                    case IsoCase.Case2Short:
                        if (datavalue < 1 || datavalue > 256) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 1(+2) - 256(+2).");
                        }
                        _le = (datavalue == 256)
                            ? 0
                            : datavalue;

                        break;

                    case IsoCase.Case4Short:
                        if (Protocol == SCardProtocol.T0 && value != 2) {
                            throw new ArgumentException("Iso7816-4 " + Case +
                                " with protocol " + Protocol +
                                " requires data to be transferred by using GET RESPONSE.");
                        }
                        
                        if (datavalue < 1 || datavalue > 256) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 1(+2) - 256(+2).");
                        }

                        _le = (datavalue == 256)
                            ? 0
                            : datavalue;

                        break;

                    case IsoCase.Case2Extended:
                        if (Protocol == SCardProtocol.T0) {
                            if (datavalue < 1 || datavalue > 256) {
                                throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                    " with protocol " + Protocol +
                                    " accepts only values from 1(+2) - 256(+2).");
                            }
                        }

                        if (datavalue < 1 || datavalue > 65536) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 1(+2)- 65536(+2).");
                        }

                        if (Protocol == SCardProtocol.T0) {
                            _le = (datavalue == 256)
                                ? 0
                                : datavalue;
                        } else {
                            _le = (datavalue == 65536)
                                ? 0
                                : datavalue;
                        }

                        break;

                    case IsoCase.Case4Extended:
                        if (Protocol == SCardProtocol.T0 && value != 2) {
                            throw new ArgumentException("Iso7816-4 " + Case +
                                " with protocol " + Protocol +
                                " requires data to be transferred by using GET RESPONSE.");
                        }

                        if (datavalue < 1 || datavalue > 65536) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + Case +
                                " accepts only values from 1(+2) - 65536(+2).");
                        }

                        _le = (datavalue == 65536)
                            ? 0
                            : datavalue;

                        break;

                    default:
                        if (value != 2) {
                            throw new ArgumentException("Iso7816-4 " + Case +
                                " does not expect any data fields in its return value and" +
                                " therefore has no bytes for Le.");
                        }

                        _le = 0;
                        
                        break;
                }
            }
        }

        public int GetLength() {
            var size = 4; /* 4 bytes: CLA, INS, P1, P2 */
            switch (Case) {
                case IsoCase.Case1:
                    /* Regarding to OpenSC: T0 needs one extra byte */
                    if (Protocol == SCardProtocol.T0) {
                        size++;
                    }
                    break;

                case IsoCase.Case2Short:
                    /* 1 byte for Le */
                    size++;
                    break;

                case IsoCase.Case3Short:
                    if (_data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    /* 1 byte for Lc + Num(Lc) bytes */
                    size += 1 + _lc;
                    break;

                case IsoCase.Case4Short:
                    if (_data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += _lc; /* Num(Lc) bytes */
                    if (Protocol == SCardProtocol.T0) {
                        size += 1; /* 1 byte for Lc.
                                     * Regarding to OpenSC: T0 has no byte for Le */
                    } else {
                        size += 2; /* 1 byte for Lc AND 1 byte for Le */
                    }
                    break;

                case IsoCase.Case2Extended:
                    if (Protocol == SCardProtocol.T0) {
                        size++; /* Regarding to OpenSC: T0 needs only one byte for Le */
                    } else {
                        size += 3; /* 3 bytes for Le */
                    }
                    break;

                case IsoCase.Case3Extended:
                    if (_data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += _lc; /* Num(Lc) bytes */
                    if (Protocol == SCardProtocol.T0) {
                        size++; /* Regarding to OpenSC: T0 needs only one byte for Lc */
                    } else {
                        size += 3; /* 3 bytes for Lc */
                    }

                    break;

                case IsoCase.Case4Extended:
                    if (_data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += _lc; /* Num(Lc) bytes */
                    if (Protocol == SCardProtocol.T0) {
                        size++; /* Regarding to OpenSC: T0 has only 1 byte for Lc
                                     * and no byte for Le */
                    } else {
                        size += 5; /* 3 bytes for Lc AND 2 bytes for Le */
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }

            return size;
        }

        public override byte[] ToArray() {
            // Inspired by the work from Nils Larsch (OpenSC)

            var size = GetLength(); /* Throws an InvalidOperationException if the ISOCase requires 
                                     * data but nothing has been set yet. */
            var apdu = new byte[size];

            var pos = 0;

            // APDU header
            apdu[pos++] = _cla;
            apdu[pos++] = _ins;
            apdu[pos++] = _p1;
            apdu[pos++] = _p2;

            switch (Case) {
                case IsoCase.Case1:
                    /* Regarding to OpenSC: T0 needs one additional 
                     * byte containing 0x00. */
                    if (Protocol == SCardProtocol.T0) {
                        apdu[pos] = 0;
                    }
                    break;

                case IsoCase.Case2Short:
                    /* Body contains only Le. */
                    apdu[pos] = (byte) _le;
                    break;

                case IsoCase.Case3Short:
                    /* Body contains Num(Lc) followed by the data. */
                    apdu[pos++] = (byte) _lc;
                    Array.Copy(_data, 0, apdu, pos, _lc);
                    break;

                case IsoCase.Case4Short:
                    /* Body contains Num(Lc) followed by the data
                     * and Num(Le). */
                    apdu[pos++] = (byte) _lc;
                    Array.Copy(_data, 0, apdu, pos, _lc);
                    pos += _lc;
                    /* Regarding to OpenSC: T0 has no Le */
                    if (Protocol != SCardProtocol.T0) {
                        apdu[pos] = (byte) _le;
                    }
                    break;

                case IsoCase.Case2Extended:
                    /* Body contains only Le. Regarding to OpenSC: T0 has only
                     * a short Le (1 byte instead of 3 bytes).*/
                    if (Protocol == SCardProtocol.T0) {
                        apdu[pos] = (byte) _le;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (_le >> 8); // B1 = higher bits
                        apdu[pos] = (byte) (_le & 0xFF); // B2 = lower bits
                    }
                    break;

                case IsoCase.Case3Extended:
                    /* Body contains Num(Lc) followed by the data. 
                     * Regarding to OpenSC: T0 has only 1 byte for Lc and
                     * therefore Num(Lc) cannot be greater then 255. */
                    if (Protocol == SCardProtocol.T0) {
                        apdu[pos++] = (byte) _lc;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (_lc >> 8); // B1 = higher bits
                        apdu[pos++] = (byte) (_lc & 0xFF); // B2 = lower bits
                    }
                    Array.Copy(_data, 0, apdu, pos, _lc);
                    break;

                case IsoCase.Case4Extended:
                    /* Body contains Num(Lc) followed by the data and Num(Le).
                     * Regarding to OpenSC: T0 has only 1 byte for Lc and
                     * no Le */
                    if (Protocol == SCardProtocol.T0) {
                        apdu[pos++] = (byte) _lc;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (_lc >> 8); // B1 = higher bits
                        apdu[pos++] = (byte) (_lc & 0xFF); // B2 = lower bits
                    }
                    Array.Copy(_data, 0, apdu, pos, 255);
                    pos += _lc;

                    if (Protocol != SCardProtocol.T0) {
                        /* Case4Extended uses two bytes to "encode"
                         * the Le value. */
                        apdu[pos++] = (byte) (_le >> 8); // Bl-1 = higher bits
                        apdu[pos] = (byte) (_le & 0xff); // Bl = lower bits
                    }
                    break;

                default:
                    throw new NotSupportedException(string.Format("IsoCase {0} is not supported.", Case));
            }

            return apdu;
        }

        public override bool IsValid {
            get {
                try {
                    /* Throws an InvalidOperationException if the ISOCase requires 
                     * data but nothing has been set yet. */
                    GetLength();
                    return true;
                } catch {
                    return false;
                }
            }
        }
        
        public virtual object Clone() {
            return new CommandApdu(Case, Protocol) {
                _cla = _cla,
                _ins = _ins,
                _p1 = _p1,
                _p2 = _p2,
                _lc = _lc,
                _le = _le,
                _data = _data
            };
        }
    }
}
