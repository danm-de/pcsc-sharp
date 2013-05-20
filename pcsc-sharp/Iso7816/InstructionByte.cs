namespace PCSC.Iso7816
{
    /// <summary>A class that describes the instruction of a command APDU.</summary>
    public class InstructionByte
    {
        private byte _instruction;

        /// <summary>Initializes a new instance of the <see cref="InstructionByte" /> class.</summary>
        /// <param name="code">The instruction code.</param>
        public InstructionByte(InstructionCode code) {
            _instruction = (byte) code;
        }

        /// <summary>Initializes a new instance of the <see cref="InstructionByte" /> class.</summary>
        /// <param name="instruction">The instruction as byte.</param>
        protected internal InstructionByte(byte instruction) {
            this._instruction = instruction;
        }

        /// <summary>Gets or sets the instruction code.</summary>
        public InstructionCode Code {
            get { return (InstructionCode) _instruction; }
            set { _instruction = (byte) value; }
        }

        /// <summary>Gets or sets the instruction as value.</summary>
        public byte Value {
            get { return _instruction; }
            set { _instruction = value; }
        }

        /// <summary>Implicitly converts a <see cref="InstructionByte" /> to a single INS byte.</summary>
        /// <returns>A byte containing INS.</returns>
        public static implicit operator byte(InstructionByte insByteInfo) {
            return insByteInfo.Value;
        }

        /// <summary>Implicitly converts a byte to a <see cref="InstructionByte" /> instance.</summary>
        /// <param name="instruction">The instruction as byte.</param>
        /// <returns>A <see cref="InstructionByte" /> class</returns>
        public static implicit operator InstructionByte(byte instruction) {
            return new InstructionByte(instruction);
        }

        /// <summary>Implicitly converts a <see cref="InstructionCode" /> to a <see cref="InstructionByte" /> instance.</summary>
        /// <param name="code">The instruction code.</param>
        /// <returns>A <see cref="InstructionByte" /> instance</returns>
        public static implicit operator InstructionByte(InstructionCode code) {
            return new InstructionByte(code);
        }
    }
}