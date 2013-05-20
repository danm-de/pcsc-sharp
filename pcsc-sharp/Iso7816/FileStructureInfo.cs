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

        private readonly byte _fileDescriptor;

        private readonly FileStructureType _structureType;
        private readonly bool _isTransparent;
        private readonly bool _isRecord;

        private RecordInfo _recordInfoCache;

        /// <summary>Initializes a new instance of the <see cref="FileStructureInfo" /> class.</summary>
        /// <param name="fileDescriptor">The file descriptor containing the file structure information.</param>
        public FileStructureInfo(byte fileDescriptor) {
            _fileDescriptor = fileDescriptor;
            _isTransparent = false;
            _isRecord = false;

            // StructureType
            if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_NO_INFO)) {
                _structureType = FileStructureType.NoInformation;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_TRANSPARENT)) {
                _structureType = FileStructureType.Transparent;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_FIXED)) {
                _structureType = FileStructureType.LinearFixed;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_FIXED_TLV)) {
                _structureType = FileStructureType.LinearFixedSimpleTlv;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_VARIABLE)) {
                _structureType = FileStructureType.LinearVariable;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_VARIABLE_TLV)) {
                _structureType = FileStructureType.LinearFixedSimpleTlv;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_CYCLIC)) {
                _structureType = FileStructureType.Cyclic;
            } else if (_IsSet(_fileDescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_CYCLIC_TLV)) {
                _structureType = FileStructureType.CyclicSimpleTlv;
            }

            if (_structureType == FileStructureType.Transparent) {
                _isTransparent = true;
            } else if (_structureType != FileStructureType.NoInformation) {
                _isRecord = true;
            }
        }

        /// <summary>Gets the file structure type.</summary>
        public FileStructureType Type {
            get { return _structureType; }
        }

        /// <summary>Gets a value indicating whether the structuring method is a transparent EF.</summary>
        public bool IsTransparent {
            get { return _isTransparent; }
        }

        /// <summary>Gets the record information.</summary>
        /// <remarks>Returns a <see cref="RecordInfo" /> instance if the file structuring method is a record EF. Otherwise <see langword="null" />.</remarks>
        public RecordInfo RecordInfo {
            get {
                if (_recordInfoCache == null && _isRecord) {
                    _recordInfoCache = new RecordInfo(_fileDescriptor);
                }
                return _recordInfoCache;
            }
        }

        /// <summary>Gets the file descriptor.</summary>
        /// <value>The file descriptor as byte.</value>
        public byte FileDescriptor {
            get { return _fileDescriptor; }
        }

        /// <summary>Gets a value indicating whether the structuring method is a record EF.</summary>
        /// <value>
        ///     <c>true</c> if the EF is record; otherwise, <c>false</c>.</value>
        public bool IsRecord {
            get { return _isRecord; }
        }

        private bool _IsSet(byte value, byte mask, byte bits) {
            return ((value & mask) == bits);
        }
    }
}