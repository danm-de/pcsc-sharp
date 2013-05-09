using System;

namespace PCSC.Iso7816
{
    public class CommandApdu : Apdu, ICloneable
    {
        // see http://www.cardwerk.com/smartcards/smartcard_standard_Iso7816-4_5_basic_organizations.aspx

        protected byte cla, ins, p1, p2;
        protected int lc, le;
        protected byte[] data;

        public CommandApdu(IsoCase isocase, SCardProtocol protocol) {
            this.isocase = isocase;
            proto = protocol;
        }

        public byte CLA {
            get { return cla; }
            set { cla = value; }
        }

        public ClassByte GetClassByteInfo() {
            return new ClassByte(cla);
        }

        public InstructionByte GetInstructionByteInfo() {
            return new InstructionByte(ins);
        }

        public byte INS {
            get { return ins; }
            set { ins = value; }
        }

        public InstructionCode Instruction {
            set { ins = (byte) value; }
        }

        public byte P1 {
            get { return p1; }
            set { p1 = value; }
        }

        public byte P2 {
            get { return p2; }
            set { p2 = value; }
        }

        public int P1P2 {
            get { return (p1 << 8) | p2; }
            set {
                if (value < 0 || value > 0xFFFF) {
                    throw new ArgumentException(
                        "Must be a value between 0x00 and 0xFFFF.",
                        new OverflowException());
                }
                p2 = (byte) (0xFF & value);
                p1 = (byte) ((0xFF00 & value) >> 8);
            }
        }

        public byte[] Data {
            get { return data; }
            set {
                switch (isocase) {
                    case IsoCase.Case3Short:
                    case IsoCase.Case4Short:
                        if (value == null) {
                            throw new ArgumentNullException("Iso7816-4 " + isocase +
                                " expects 1 to 255 bytes of data.");
                        }
                        if (value.Length > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " expects 1 to 255 bytes of data.");
                        }
                        data = value;
                        lc = data.Length;
                        break;

                    case IsoCase.Case3Extended:
                    case IsoCase.Case4Extended:
                        if (value == null) {
                            throw new ArgumentNullException("Iso7816-4 " + isocase +
                                " expects 1 to 65535 bytes of data.");
                        }
                        if (proto == SCardProtocol.T0 && value.Length > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " with protocol " + proto +
                                " accepts only 255 bytes of data.");
                        }
                        if (value.Length > 65535) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " expects 1 to 65535 bytes of data.");
                        }
                        data = value;
                        lc = data.Length;
                        break;

                    default:
                        throw new ArgumentException("Iso7816-4 " + isocase +
                            " does not expect any data in its APDU command.");
                }
            }
        }

        public int Lc {
            get { return lc; }
        }

        public int P3 {
            get { return Le; }
            set { Le = value; }
        }

        public int Le {
            get { return le; }
            set {
                // Inspired by the work from Nils Larsch (OpenSC)
                switch (isocase) {
                    case IsoCase.Case2Short:
                        if (value < 0 || value > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 0 - 255 in Le.");
                        }
                        le = value;
                        break;

                    case IsoCase.Case4Short:
                        if (proto == SCardProtocol.T0) {
                            throw new ArgumentException("Iso7816-4 " + isocase +
                                " with protocol " + proto +
                                " requires data to be transferred by using GET RESPONSE.");
                        }
                        if (value < 0 || value > 255) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 0 - 255 in Le.");
                        }
                        le = value;
                        break;

                    case IsoCase.Case2Extended:
                        if (proto == SCardProtocol.T0) {
                            if (value < 0 || value > 255) {
                                throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                    " with protocol " + proto +
                                    " accepts only values from 0 - 255 in Le.");
                            }
                        }
                        if (value < 0 || value > 65535) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 0 - 65535 in Le.");
                        }
                        le = value;
                        break;

                    case IsoCase.Case4Extended:
                        if (proto == SCardProtocol.T0) {
                            throw new ArgumentException("Iso7816-4 " + isocase +
                                " with protocol " + proto +
                                " requires data to be transferred by using GET RESPONSE.");
                        }
                        if (value < 0 || value > 65535) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 0 - 65535 in Le.");
                        }
                        le = value;
                        break;

                    default:
                        throw new ArgumentException("Iso7816-4 " + isocase +
                            " does not expect any data fields in its return value and" +
                            " therefore has no bytes for Le.");
                }
            }
        }
        public int ExpectedResponseLength {
            /* ExpectedResponseLength = Le + SW1SW2.
             */
            get {
                switch (isocase) {
                    case IsoCase.Case2Short:
                        if (le == 0) {
                            return 256 + 2;
                        }

                        return le + 2;

                    case IsoCase.Case4Short:
                        if (proto == SCardProtocol.T0) {
                            return 0 + 2;
                        }

                        if (le == 0) {
                            return 256 + 2;
                        }

                        return le + 2;

                    case IsoCase.Case2Extended:
                        if (proto == SCardProtocol.T0) {
                            if (le == 0) {
                                return 256 + 2;
                            }
                            return le + 2;
                        }

                        if (le == 0) {
                            return 65536 + 2;
                        }

                        return le + 2;

                    case IsoCase.Case4Extended:
                        if (proto == SCardProtocol.T0) {
                            return 0 + 2;
                        }

                        if (le == 0) {
                            return 65536 + 2;
                        }

                        return le + 2;

                    default:
                        return 0 + 2;
                }
            }
            set {
                /* SW1SW2 = 2 bytes
                 * Therefore we remove 2 bytes for SW1SW2.
                 */
                var datavalue = value - 2;
                switch (isocase) {
                    case IsoCase.Case2Short:
                        if (datavalue < 1 || datavalue > 256) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 1(+2) - 256(+2).");
                        }
                        le = (datavalue == 256)
                            ? 0
                            : datavalue;

                        break;

                    case IsoCase.Case4Short:
                        if (proto == SCardProtocol.T0 && value != 2) {
                            throw new ArgumentException("Iso7816-4 " + isocase +
                                " with protocol " + proto +
                                " requires data to be transferred by using GET RESPONSE.");
                        }
                        
                        if (datavalue < 1 || datavalue > 256) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 1(+2) - 256(+2).");
                        }

                        le = (datavalue == 256)
                            ? 0
                            : datavalue;

                        break;

                    case IsoCase.Case2Extended:
                        if (proto == SCardProtocol.T0) {
                            if (datavalue < 1 || datavalue > 256) {
                                throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                    " with protocol " + proto +
                                    " accepts only values from 1(+2) - 256(+2).");
                            }
                        }

                        if (datavalue < 1 || datavalue > 65536) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 1(+2)- 65536(+2).");
                        }

                        if (proto == SCardProtocol.T0) {
                            le = (datavalue == 256)
                                ? 0
                                : datavalue;
                        } else {
                            le = (datavalue == 65536)
                                ? 0
                                : datavalue;
                        }

                        break;

                    case IsoCase.Case4Extended:
                        if (proto == SCardProtocol.T0 && value != 2) {
                            throw new ArgumentException("Iso7816-4 " + isocase +
                                " with protocol " + proto +
                                " requires data to be transferred by using GET RESPONSE.");
                        }

                        if (datavalue < 1 || datavalue > 65536) {
                            throw new ArgumentOutOfRangeException("Iso7816-4 " + isocase +
                                " accepts only values from 1(+2) - 65536(+2).");
                        }

                        le = (datavalue == 65536)
                            ? 0
                            : datavalue;

                        break;

                    default:
                        if (value != 2) {
                            throw new ArgumentException("Iso7816-4 " + isocase +
                                " does not expect any data fields in its return value and" +
                                " therefore has no bytes for Le.");
                        }

                        le = 0;
                        
                        break;
                }
            }
        }

        public int GetLength() {
            var size = 4; /* 4 bytes: CLA, INS, P1, P2 */
            switch (isocase) {
                case IsoCase.Case1:
                    /* Regarding to OpenSC: T0 needs one extra byte */
                    if (proto == SCardProtocol.T0) {
                        size++;
                    }
                    break;

                case IsoCase.Case2Short:
                    /* 1 byte for Le */
                    size++;
                    break;

                case IsoCase.Case3Short:
                    if (data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    /* 1 byte for Lc + Num(Lc) bytes */
                    size += 1 + lc;
                    break;

                case IsoCase.Case4Short:
                    if (data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += lc; /* Num(Lc) bytes */
                    if (proto == SCardProtocol.T0) {
                        size += 1; /* 1 byte for Lc.
                                     * Regarding to OpenSC: T0 has no byte for Le */
                    } else {
                        size += 2; /* 1 byte for Lc AND 1 byte for Le */
                    }
                    break;

                case IsoCase.Case2Extended:
                    if (proto == SCardProtocol.T0) {
                        size++; /* Regarding to OpenSC: T0 needs only one byte for Le */
                    } else {
                        size += 3; /* 3 bytes for Le */
                    }
                    break;

                case IsoCase.Case3Extended:
                    if (data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += lc; /* Num(Lc) bytes */
                    if (proto == SCardProtocol.T0) {
                        size++; /* Regarding to OpenSC: T0 needs only one byte for Lc */
                    } else {
                        size += 3; /* 3 bytes for Lc */
                    }

                    break;

                case IsoCase.Case4Extended:
                    if (data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += lc; /* Num(Lc) bytes */
                    if (proto == SCardProtocol.T0) {
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
            apdu[pos++] = cla;
            apdu[pos++] = ins;
            apdu[pos++] = p1;
            apdu[pos++] = p2;

            switch (isocase) {
                case IsoCase.Case1:
                    /* Regarding to OpenSC: T0 needs one additional 
                     * byte containing 0x00. */
                    if (proto == SCardProtocol.T0) {
                        apdu[pos] = 0;
                    }
                    break;

                case IsoCase.Case2Short:
                    /* Body contains only Le. */
                    apdu[pos] = (byte) le;
                    break;

                case IsoCase.Case3Short:
                    /* Body contains Num(Lc) followed by the data. */
                    apdu[pos++] = (byte) lc;
                    Array.Copy(data, 0, apdu, pos, lc);
                    break;

                case IsoCase.Case4Short:
                    /* Body contains Num(Lc) followed by the data
                     * and Num(Le). */
                    apdu[pos++] = (byte) lc;
                    Array.Copy(data, 0, apdu, pos, lc);
                    pos += lc;
                    /* Regarding to OpenSC: T0 has no Le */
                    if (proto != SCardProtocol.T0) {
                        apdu[pos] = (byte) le;
                    }
                    break;

                case IsoCase.Case2Extended:
                    /* Body contains only Le. Regarding to OpenSC: T0 has only
                     * a short Le (1 byte instead of 3 bytes).*/
                    if (proto == SCardProtocol.T0) {
                        apdu[pos] = (byte) le;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (le >> 8); // B1 = higher bits
                        apdu[pos] = (byte) (le & 0xFF); // B2 = lower bits
                    }
                    break;

                case IsoCase.Case3Extended:
                    /* Body contains Num(Lc) followed by the data. 
                     * Regarding to OpenSC: T0 has only 1 byte for Lc and
                     * therefore Num(Lc) cannot be greater then 255. */
                    if (proto == SCardProtocol.T0) {
                        apdu[pos++] = (byte) lc;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (lc >> 8); // B1 = higher bits
                        apdu[pos++] = (byte) (lc & 0xFF); // B2 = lower bits
                    }
                    Array.Copy(data, 0, apdu, pos, lc);
                    break;

                case IsoCase.Case4Extended:
                    /* Body contains Num(Lc) followed by the data and Num(Le).
                     * Regarding to OpenSC: T0 has only 1 byte for Lc and
                     * no Le */
                    if (proto == SCardProtocol.T0) {
                        apdu[pos++] = (byte) lc;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (lc >> 8); // B1 = higher bits
                        apdu[pos++] = (byte) (lc & 0xFF); // B2 = lower bits
                    }
                    Array.Copy(data, 0, apdu, pos, 255);
                    pos += lc;

                    if (proto != SCardProtocol.T0) {
                        /* Case4Extended uses two bytes to "encode"
                         * the Le value. */
                        apdu[pos++] = (byte) (le >> 8); // Bl-1 = higher bits
                        apdu[pos] = (byte) (le & 0xff); // Bl = lower bits
                    }
                    break;

                default:
                    throw new NotSupportedException(string.Format("IsoCase {0} is not supported.", isocase));
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
            return new CommandApdu(isocase, proto) {
                cla = cla,
                ins = ins,
                p1 = p1,
                p2 = p2,
                lc = lc,
                le = le,
                data = data
            };
        }
    }
}
