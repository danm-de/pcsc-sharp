using System;

namespace PCSC.Iso7816
{
    /// <summary>A Command Application Protocol Data Unit (APDU), defined by the ISO/IEC 7816 standards</summary>
    /// <remarks>
    ///     <para>A <see cref="CommandApdu" /> can be build using one of the following <see cref="IsoCase" />:</para>
    ///     <list type="table">
    ///         <listheader><term>Case</term><description>APDU structure</description></listheader>
    ///         <item><term>1</term><description>CLA INS P1 P2</description></item>
    ///         <item><term>2</term><description>CLA INS P1 P2 Le</description></item>
    ///         <item><term>3</term><description>CLA INS P1 P2 Lc Data</description></item>
    ///         <item><term>4</term><description>CLA INS P1 P2 Lc Data Le</description></item>
    ///     </list>
    ///     <para>See the documentation for <see cref="IsoCase" /> for more information.</para>
    /// </remarks>
    public class CommandApdu : Apdu, ICloneable
    {
        // see http://www.cardwerk.com/smartcards/smartcard_standard_Iso7816-4_5_basic_organizations.aspx

        private int _le;
        private byte[] _data;

        /// <summary>Initializes a new instance of the <see cref="CommandApdu" /> class.</summary>
        /// <param name="isoCase">The ISO case to use.</param>
        /// <param name="protocol">The protocol.</param>
        public CommandApdu(IsoCase isoCase, SCardProtocol protocol) {
            Case = isoCase;
            Protocol = protocol;
        }

        /// <summary>Gets or sets the CLA byte.</summary>
        /// <remarks>You can use the <see cref="ClassByte" /> class to build a well formed CLA byte.</remarks>
        public byte CLA { get; set; }

        /// <summary>Gets the CLA.</summary>
        /// <returns>The <see cref="CLA" /> as <see cref="ClassByte" /> instance.</returns>
        public ClassByte GetClassByteInfo() {
            return new ClassByte(CLA);
        }

        /// <summary>Gets the instruction byte info.</summary>
        /// <returns>The <see cref="INS" /> as <see cref="InstructionByte" /> instance.</returns>
        public InstructionByte GetInstructionByteInfo() {
            return new InstructionByte(INS);
        }

        /// <summary>Gets or sets the instruction.</summary>
        public byte INS { get; set; }

        /// <summary>Sets the instruction.</summary>
        public InstructionCode Instruction {
            set {
                unchecked {
                    INS = (byte) value;
                }
            }
        }

        /// <summary>The first parameter (P1)</summary>
        public byte P1 { get; set; }

        /// <summary>The second parameter (P2)</summary>
        public byte P2 { get; set; }

        /// <summary>A combination of parameter P1 and P2</summary>
        public int P1P2 {
            get { return (P1 << 8) | P2; }
            set {
                if (value < 0 || value > 0xFFFF) {
                    throw new ArgumentException(
                        "Must be a value between 0x00 and 0xFFFF.",
                        new OverflowException());
                }
                P2 = (byte) (0xFF & value);
                P1 = (byte) ((0xFF00 & value) >> 8);
            }
        }

        /// <summary>Command APDU data to be transmitted.</summary>
        /// <remarks>You can only set data if you created the <see cref="CommandApdu" /> with ISO case 3 or 4.</remarks>
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
                        Lc = _data.Length;
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
                        Lc = _data.Length;
                        break;

                    default:
                        throw new ArgumentException("Iso7816-4 " + Case +
                            " does not expect any data in its APDU command.");
                }
            }
        }

        /// <summary>Length command</summary>
        public int Lc { get; private set; }

        /// <summary>The third parameter (P3 or Le)</summary>
        public int P3 {
            get { return Le; }
            set { Le = value; }
        }

        /// <summary>Length expected.</summary>
        /// <remarks>This is the expected number of response data bytes. Do not take account of the status word (SW1 and SW2) here!</remarks>
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

        /// <summary>The expected response size (Le + SW1SW2)</summary>
        public int ExpectedResponseLength {
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

        /// <summary>Calculates the APDU size in bytes.</summary>
        /// <returns>The APDU size in bytes depending on the currently selected ISO case.</returns>
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
                    size += 1 + Lc;
                    break;

                case IsoCase.Case4Short:
                    if (_data == null) {
                        throw new InvalidOperationException("No data has been set.");
                    }

                    size += Lc; /* Num(Lc) bytes */
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

                    size += Lc; /* Num(Lc) bytes */
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

                    size += Lc; /* Num(Lc) bytes */
                    if (Protocol == SCardProtocol.T0) {
                        size++; /* Regarding to OpenSC: T0 has only 1 byte for Lc
                                     * and no byte for Le */
                    } else {
                        size += 5; /* 3 bytes for Lc AND 2 bytes for Le */
                    }
                    break;

                default:
                    throw new InvalidOperationException(string.Format("IsoCase {0} is not supported.", Case));
            }

            return size;
        }

        /// <summary>Converts the command APDU to a transmittable byte array.</summary>
        /// <returns>The command APDU as byte array.</returns>
        /// <exception cref="InvalidOperationException">If the command APDU is in an invalid state.</exception>
        public override byte[] ToArray() {
            // Inspired by the work from Nils Larsch (OpenSC)

            var size = GetLength(); /* Throws an InvalidOperationException if the ISOCase requires 
                                     * data but nothing has been set yet. */
            var apdu = new byte[size];

            var pos = 0;

            // APDU header
            apdu[pos++] = CLA;
            apdu[pos++] = INS;
            apdu[pos++] = P1;
            apdu[pos++] = P2;

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
                    apdu[pos++] = (byte) Lc;
                    Array.Copy(_data, 0, apdu, pos, Lc);
                    break;

                case IsoCase.Case4Short:
                    /* Body contains Num(Lc) followed by the data
                     * and Num(Le). */
                    apdu[pos++] = (byte) Lc;
                    Array.Copy(_data, 0, apdu, pos, Lc);
                    pos += Lc;
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
                        apdu[pos++] = (byte) Lc;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (Lc >> 8); // B1 = higher bits
                        apdu[pos++] = (byte) (Lc & 0xFF); // B2 = lower bits
                    }
                    Array.Copy(_data, 0, apdu, pos, Lc);
                    break;

                case IsoCase.Case4Extended:
                    /* Body contains Num(Lc) followed by the data and Num(Le).
                     * Regarding to OpenSC: T0 has only 1 byte for Lc and
                     * no Le */
                    if (Protocol == SCardProtocol.T0) {
                        apdu[pos++] = (byte) Lc;
                    } else {
                        apdu[pos++] = 0; // B0 = 0x00
                        apdu[pos++] = (byte) (Lc >> 8); // B1 = higher bits
                        apdu[pos++] = (byte) (Lc & 0xFF); // B2 = lower bits
                    }
                    Array.Copy(_data, 0, apdu, pos, Lc);
                    pos += Lc;

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

        /// <summary> Indicates if the command APDU is valid.</summary>
        /// <value><see langword="true" /> if the APDU is valid.</value>
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

        /// <summary>Creates a clone of the current instance. The data is NOT copied. </summary>
        /// <returns>A clone of the current instance.</returns>
        public virtual object Clone() {
            return new CommandApdu(Case, Protocol) {
                CLA = CLA,
                INS = INS,
                P1 = P1,
                P2 = P2,
                Lc = Lc,
                _le = _le,
                _data = _data
            };
        }
    }
}