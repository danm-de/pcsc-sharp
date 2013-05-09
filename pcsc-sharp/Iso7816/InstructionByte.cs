namespace PCSC.Iso7816
{
    public class InstructionByte
    {
        protected byte ins;

        public InstructionByte(InstructionCode code) {
            ins = (byte) code;
        }

        protected internal InstructionByte(byte ins) {
            this.ins = ins;
        }

        public InstructionCode Code {
            get { return (InstructionCode) ins; }
            set { ins = (byte) value; }
        }

        public byte Value {
            get { return ins; }
            set { ins = value; }
        }

        public static implicit operator byte(InstructionByte insByteInfo) {
            return insByteInfo.Value;
        }

        public static implicit operator InstructionByte(byte b) {
            return new InstructionByte(b);
        }

        public static implicit operator InstructionByte(InstructionCode code) {
            return new InstructionByte(code);
        }
    }
}
