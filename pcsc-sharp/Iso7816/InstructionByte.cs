namespace PCSC.Iso7816
{
    /// <summary>A class that describes the instruction of a command APDU.</summary>
    public class InstructionByte
    {
        /// <summary>Initializes a new instance of the <see cref="InstructionByte" /> class.</summary>
        /// <param name="code">The instruction code.</param>
        public InstructionByte(InstructionCode code) {
            Value = (byte) code;
        }

        /// <summary>Initializes a new instance of the <see cref="InstructionByte" /> class.</summary>
        /// <param name="instruction">The instruction as byte.</param>
        protected internal InstructionByte(byte instruction) {
            Value = instruction;
        }

        /// <summary>Gets or sets the instruction code.</summary>
        public InstructionCode Code {
            get { return (InstructionCode) Value; }
            set { Value = (byte) value; }
        }

        /// <summary>Gets or sets the instruction as value.</summary>
        public byte Value { get; set; }

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