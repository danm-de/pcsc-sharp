namespace PCSC.Iso7816
{
    public class FileDescriptor
    {
        public const byte FILE_TYPE_MASK                = (1 << 7) + (1 << 5) + (1 << 4) + (1 << 3);
        public const byte FILE_TYPE_DF                  = (1 << 5) + (1 << 4) + (1 << 3);
        public const byte FILE_TYPE_WORKING_EF          = (0 << 5) + (0 << 4) + (0 << 3);
        public const byte FILE_TYPE_INTERNAL_EF         = (0 << 5) + (0 << 4) + (1 << 3);

        public const byte FILE_SHARE_MODE_MASK          = (1 << 7) + (1 << 6);
        public const byte FILE_SHARE_MODE_SHAREABLE     = (1 << 6);
        public const byte FILE_SHARE_MODE_NOTSHAREABLE  = (0 << 6);

        private readonly byte _fileDescriptor;

        private readonly FileType _type;
        private readonly ExtendedFileType _extendedType;
        private readonly FileShareMode _shareMode;
        private FileStructureInfo _fileStructureInfoCache;

        public byte Descriptor {
            get { return _fileDescriptor; }
        }

        public FileType Type {
            get { return _type; }
        }

        public ExtendedFileType ExtendedType {
            get { return _extendedType; }
        }

        public FileShareMode ShareMode {
            get { return _shareMode; }
        }

        public FileStructureInfo Structure {
            get {
                if (_fileStructureInfoCache == null) {
                    if (_extendedType == ExtendedFileType.InternalElementary || _extendedType == ExtendedFileType.WorkingElementary) {
                        _fileStructureInfoCache = new FileStructureInfo(_fileDescriptor);
                    }
                }
                return _fileStructureInfoCache;
            }
        }

        public FileDescriptor(byte fileDescriptorByte) {
            _fileDescriptor = fileDescriptorByte;
            
            _type = SCardHelper.IsSet(_fileDescriptor, FILE_TYPE_MASK, FILE_TYPE_DF)
                ? FileType.Dedicated
                : FileType.Elementary;

            // ExtendedType
            if (SCardHelper.IsSet(_fileDescriptor, FILE_TYPE_MASK, FILE_TYPE_DF)) {
                _extendedType = ExtendedFileType.Dedicated;
            } else if (SCardHelper.IsSet(_fileDescriptor, FILE_TYPE_MASK, FILE_TYPE_WORKING_EF)) {
                _extendedType = ExtendedFileType.WorkingElementary;
            } else if (SCardHelper.IsSet(_fileDescriptor, FILE_TYPE_MASK, FILE_TYPE_INTERNAL_EF)) {
                _extendedType = ExtendedFileType.InternalElementary;
            } else {
                _extendedType = ExtendedFileType.Proprietary;
            }

            _shareMode = SCardHelper.IsSet(_fileDescriptor, FILE_SHARE_MODE_MASK, FILE_SHARE_MODE_SHAREABLE)
                ? FileShareMode.Shareable
                : FileShareMode.NotShareable;

            _fileStructureInfoCache = null;
        }

        public bool IsSet(byte mask, byte bits) {
            return SCardHelper.IsSet(_fileDescriptor, mask, bits);
        }

        public static implicit operator byte(FileDescriptor fd) {
            return fd._fileDescriptor;
        }

        public static implicit operator FileDescriptor(byte b) {
            return new FileDescriptor(b);
        }
    }
}
