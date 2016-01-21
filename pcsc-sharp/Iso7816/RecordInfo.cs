namespace PCSC.Iso7816
{
    /// <summary>Record structure information.</summary>
    /// <remarks>Elementary files (EF) that have a sequence of individually identifiable records should use one of the following methods for structuring:
    ///     <list type="bullet">
    ///         <item><description>Linear elementary file (EF) with records of variable size.</description></item>
    ///         <item><description>Linear elementary file (EF) with records of fixed size.</description></item>
    ///         <item><description>Cyclic elementary file (EF) with records of fixed size.</description></item>
    ///     </list>
    /// </remarks>
    public class RecordInfo
    {
        /// <summary>File structuring mask bits.</summary>
        public const byte FILE_STRUCTURING_MASK             = (1 << 7) + (1 << 2) + (1 << 1);
        /// <summary>Linear fixed bit.</summary>
        public const byte FILE_STRUCTURE_IS_LINEAR_FIXED    = (0 << 2) + (1 << 1);
        /// <summary>Linear variable bit.</summary>
        public const byte FILE_STRUCTURE_IS_LINEAR_VARIABLE = (1 << 2) + (0 << 1);
        /// <summary>Cyclic bit.</summary>
        public const byte FILE_STRUCTURE_IS_CYCLIC          = (1 << 2) + (1 << 1);

        /// <summary>Initializes a new instance of the <see cref="RecordInfo" /> class.</summary>
        /// <param name="fileDescriptor">The file descriptor.</param>
        public RecordInfo(byte fileDescriptor) {
            FileDescriptor = fileDescriptor;

            IsCyclic = false;
            IsLinear = false;
            IsFixed = false;
            IsVariable = false;
            IsSimpleTlv = false;

            // IsCyclic
            if (SCardHelper.IsSet(fileDescriptor, FILE_STRUCTURING_MASK, FILE_STRUCTURE_IS_CYCLIC)) {
                IsCyclic = true;

                // IsFixed
                IsFixed = true;
            }

            // IsLinear
            if (!IsCyclic) {
                IsLinear = true;

                // IsFixed
                if (SCardHelper.IsSet(fileDescriptor, FILE_STRUCTURING_MASK, FILE_STRUCTURE_IS_LINEAR_FIXED)) {
                    IsFixed = true;
                }

                // IsVariable
                if (SCardHelper.IsSet(fileDescriptor, FILE_STRUCTURING_MASK, FILE_STRUCTURE_IS_LINEAR_VARIABLE)) {
                    IsVariable = true;
                }
            }

            // IsSimpleTlv
            if (SCardHelper.IsSet(fileDescriptor, 1, 1)) {
                IsSimpleTlv = true;
            }
        }

        /// <summary>Gets a value indicating whether the EF has cyclic records.</summary>
        /// <value><c>true</c> if the EF is cyclic; otherwise, <c>false</c>.</value>
        public bool IsCyclic { get; }

        /// <summary>Gets a value indicating whether the EF has linear records.</summary>
        /// <value><c>true</c> if the EF is linear; otherwise, <c>false</c>.</value>
        public bool IsLinear { get; }

        /// <summary>Gets a value indicating whether the EF's record size is fixed.</summary>
        /// <value><c>true</c> if the record's size is fixed; otherwise, <c>false</c>.</value>
        public bool IsFixed { get; }

        /// <summary>Gets a value indicating whether the EF's record size is variable.</summary>
        /// <value><c>true</c> if the record's size is variable; otherwise, <c>false</c>.</value>
        public bool IsVariable { get; }

        /// <summary>Gets a value indicating whether the EF contains simple TLV records.</summary>
        /// <value><c>true</c> if the EF contains simple TLV records; otherwise, <c>false</c>.</value>
        public bool IsSimpleTlv { get; }

        /// <summary>Gets the file descriptor.</summary>
        /// <value>The file descriptor as byte.</value>
        public byte FileDescriptor { get; }
    }
}