namespace PCSC.Iso7816
{
    public class FileStructureInfo
    {
        public const byte FILE_STRUCTURE_MASK                   = (1 << 7) + (1 << 2) + (1 << 1) + (1 << 0);
        public const byte FILE_STRUCTURE_NO_INFO                = (0 << 2) + (0 << 1) + (0 << 0);
        public const byte FILE_STRUCTURE_TRANSPARENT            = (0 << 2) + (0 << 1) + (1 << 0);
        public const byte FILE_STRUCTURE_LINEAR_FIXED           = (0 << 2) + (1 << 1) + (0 << 0);
        public const byte FILE_STRUCTURE_LINEAR_FIXED_TLV       = (0 << 2) + (1 << 1) + (1 << 0);
        public const byte FILE_STRUCTURE_LINEAR_VARIABLE        = (1 << 2) + (0 << 1) + (0 << 0);
        public const byte FILE_STRUCTURE_LINEAR_VARIABLE_TLV    = (1 << 2) + (0 << 1) + (1 << 0);
        public const byte FILE_STRUCTURE_CYCLIC                 = (1 << 2) + (1 << 1) + (0 << 0);
        public const byte FILE_STRUCTURE_CYCLIC_TLV             = (1 << 2) + (1 << 1) + (1 << 0);

        private readonly byte _fileDescriptor;

        private readonly FileStructureType _structureType;
        private readonly bool _isTransparent;
        private readonly bool _isRecord;
        
        private RecordInfo _recordInfoCache;

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

        public FileStructureType Type {
            get { return _structureType; }
        }

        public bool IsTransparent {
            get { return _isTransparent; }
        }

        public RecordInfo RecordInfo {
            get {
                if (_recordInfoCache == null && _isRecord) {
                    _recordInfoCache = new RecordInfo(_fileDescriptor);
                }
                return _recordInfoCache;
            }
        }

        public byte FileDescriptor {
            get { return _fileDescriptor; }
        }

        public bool IsRecord {
            get { return _isRecord; }
        }

        private bool _IsSet(byte value, byte mask, byte bits) {
            return ((value & mask) == bits);
        }
    }
}