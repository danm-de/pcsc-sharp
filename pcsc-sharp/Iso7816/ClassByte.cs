using System;

namespace PCSC.Iso7816
{
    public class ClassByte
    {
        private const byte SECURE_MESSAGING_MASK        = 0x4 + 0x8;
        private const byte LOGICAL_CHANNEL_NUMBER_MASK  = 0x1 + 0x2;

        private byte _cla;

        public ClassByte(byte cla) {
            _cla = cla;
        }

        public ClassByte(ClaHighPart highPart, SecureMessagingFormat secMsgFmt, int logChannel) {
            if (logChannel > 3 || logChannel < 0) {
                throw new ArgumentOutOfRangeException(
                    "logChannel",
                    "Logical channels must be in the range between 0 and 3.");
            }
            _cla = (byte) ((int) highPart | (int) secMsgFmt | logChannel);
        }

        public byte Value {
            get { return _cla; }
            set { _cla = value; }
        }

        public ClaHighPart HighPart {
            get {
                // get the high part (b8,b7,b6,b5)
                var h = (byte) (0xF0 & _cla);

                // return the high part
                return (ClaHighPart) h;
            }
            set {
                // save the low part (b4,b3,b2,b1)
                var l = (byte) (0x0F & _cla);

                // combine the user specified high part with the saved low part
                _cla = (byte) ((int) value | l);
            }
        }

        public SecureMessagingFormat Security {
            get {
                byte sec = (byte) (_cla & SECURE_MESSAGING_MASK);
                return (SecureMessagingFormat) sec;
            }
            set {
                byte inversemask;
                unchecked {
                    inversemask = (byte) (~(SECURE_MESSAGING_MASK));
                }
                // save old settings
                var tmp = (byte) (_cla & inversemask);

                // set new Secure Messaging Format
                _cla = (byte) (tmp | (int) value);
            }
        }

        public int LogicalChannel {
            get {
                var logch = (_cla & LOGICAL_CHANNEL_NUMBER_MASK);
                return logch;
            }
            set {
                if (value > 3 || value < 0) {
                    throw new ArgumentException(
                        "Logical channels must be in the range between 0 and 3.",
                        new OverflowException());
                }

                byte inversemask;
                unchecked {
                    inversemask = (byte) (~(LOGICAL_CHANNEL_NUMBER_MASK));
                }
                // save old settings
                var tmp = (byte) (_cla & inversemask);

                // set new logical channel setting
                _cla = (byte) (tmp | value);
            }
        }

        public static implicit operator byte(ClassByte bInfo) {
            return bInfo._cla;
        }

        public static implicit operator ClassByte(byte b) {
            return new ClassByte(b);
        }
    }
}
