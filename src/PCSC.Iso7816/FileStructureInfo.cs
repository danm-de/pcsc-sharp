namespace PCSC.Iso7816
{
    /// <summary>File structure information for elementary file (EF).</summary>
    public class FileStructureInfo
    {
        /// <summary>File structure mask bits.</summary>
        public const byte FILE_STRUCTURE_MASK                   = (1 << 7) + (1 << 2) + (1 << 1) + (1 << 0);
        /// <summary>File has not structure information bit.</summary>
        public const byte FILE_STRUCTURE_NO_INFO                = (0 << 2) + (0 << 1) + (0 << 0);
        /// <summary>Transparent bit.</summary>
        public const byte FILE_STRUCTURE_TRANSPARENT            = (0 << 2) + (0 << 1) + (1 << 0);
        /// <summary>Linear fixed bit.</summary>
        public const byte FILE_STRUCTURE_LINEAR_FIXED           = (0 << 2) + (1 << 1) + (0 << 0);
        /// <summary>Linear fixed TLV bit.</summary>
        public const byte FILE_STRUCTURE_LINEAR_FIXED_TLV       = (0 << 2) + (1 << 1) + (1 << 0);
        /// <summary>Linear variable bit.</summary>
        public const byte FILE_STRUCTURE_LINEAR_VARIABLE        = (1 << 2) + (0 << 1) + (0 << 0);
        /// <summary>Linear variable TLV bit.</summary>
        public const byte FILE_STRUCTURE_LINEAR_VARIABLE_TLV    = (1 << 2) + (0 << 1) + (1 << 0);
        /// <summary>Cyclic bit.</summary>
        public const byte FILE_STRUCTURE_CYCLIC                 = (1 << 2) + (1 << 1) + (0 << 0);
        /// <summary>Cyclic TLV bit.</summary>
        public const byte FILE_STRUCTURE_CYCLIC_TLV             = (1 << 2) + (1 << 1) + (1 << 0);

        private RecordInfo _recordInfoCache;

        /// <summary>Initializes a new instance of the <see cref="FileStructureInfo" /> class.</summary>
        /// <param name="fileDescriptor">The file descriptor containing the file structure information.</param>
        public FileStructureInfo(byte fileDescriptor) {
            FileDescriptor = fileDescriptor;
            IsTransparent = false;
            IsRecord = false;

            // StructureType
            if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_NO_INFO)) {
                Type = FileStructureType.NoInformation;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_TRANSPARENT)) {
                Type = FileStructureType.Transparent;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_FIXED)) {
                Type = FileStructureType.LinearFixed;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_FIXED_TLV)) {
                Type = FileStructureType.LinearFixedSimpleTlv;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_VARIABLE)) {
                Type = FileStructureType.LinearVariable;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_VARIABLE_TLV)) {
                Type = FileStructureType.LinearFixedSimpleTlv;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_CYCLIC)) {
                Type = FileStructureType.Cyclic;
            } else if (_IsSet(FileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_CYCLIC_TLV)) {
                Type = FileStructureType.CyclicSimpleTlv;
            }

            if (Type == FileStructureType.Transparent) {
                IsTransparent = true;
            } else if (Type != FileStructureType.NoInformation) {
                IsRecord = true;
            }
        }

        /// <summary>Gets the file structure type.</summary>
        public FileStructureType Type { get; }

        /// <summary>Gets a value indicating whether the structuring method is a transparent EF.</summary>
        public bool IsTransparent { get; }

        /// <summary>Gets the record information.</summary>
        /// <remarks>Returns a <see cref="RecordInfo" /> instance if the file structuring method is a record EF. Otherwise <see langword="null" />.</remarks>
        public RecordInfo RecordInfo {
            get {
                if (_recordInfoCache == null && IsRecord) {
                    _recordInfoCache = new RecordInfo(FileDescriptor);
                }
                return _recordInfoCache;
            }
        }

        /// <summary>Gets the file descriptor.</summary>
        /// <value>The file descriptor as byte.</value>
        public byte FileDescriptor { get; }

        /// <summary>Gets a value indicating whether the structuring method is a record EF.</summary>
        /// <value>
        ///     <c>true</c> if the EF is record; otherwise, <c>false</c>.</value>
        public bool IsRecord { get; }

        private static bool _IsSet(byte value, byte mask, byte bits) {
            return ((value & mask) == bits);
        }
    }
}