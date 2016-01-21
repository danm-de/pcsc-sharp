namespace PCSC.Iso7816
{
    /// <summary>A file descriptor. See ISO/IEC7816-4 File control information (FCI) for more information.</summary>
    public class FileDescriptor
    {
        /// <summary>File type bit mask.</summary>
        public const byte FILE_TYPE_MASK                = (1 << 7) + (1 << 5) + (1 << 4) + (1 << 3);
        /// <summary>Dedicated file (DF) bit.</summary>
        public const byte FILE_TYPE_DF                  = (1 << 5) + (1 << 4) + (1 << 3);
        /// <summary>Working elementary file (EF) bit.</summary>
        public const byte FILE_TYPE_WORKING_EF          = (0 << 5) + (0 << 4) + (0 << 3);
        /// <summary>Internal elementary file (EF) bit.</summary>
        public const byte FILE_TYPE_INTERNAL_EF         = (0 << 5) + (0 << 4) + (1 << 3);
        /// <summary>Sharemode bit mask.</summary>
        public const byte FILE_SHARE_MODE_MASK          = (1 << 7) + (1 << 6);
        /// <summary>Shareable bit.</summary>
        public const byte FILE_SHARE_MODE_SHAREABLE     = (1 << 6);
        /// <summary>Non shareable bit.</summary>
        public const byte FILE_SHARE_MODE_NOTSHAREABLE  = (0 << 6);

        private FileStructureInfo _fileStructureInfoCache;

        /// <summary>The file descriptor as single byte structure.</summary>
        public byte Descriptor { get; }

        /// <summary>Gets the file type.</summary>
        public FileType Type { get; }

        /// <summary>Gets the extended file type.</summary>
        public ExtendedFileType ExtendedType { get; }

        /// <summary>Gets the file sharing mode.</summary>
        public FileShareMode ShareMode { get; }

        /// <summary>Gets file structure information.</summary>
        public FileStructureInfo Structure {
            get {
                if (_fileStructureInfoCache == null) {
                    if (ExtendedType == ExtendedFileType.InternalElementary ||
                        ExtendedType == ExtendedFileType.WorkingElementary) {
                        _fileStructureInfoCache = new FileStructureInfo(Descriptor);
                    }
                }
                return _fileStructureInfoCache;
            }
        }

        /// <summary>Initializes a new instance of the <see cref="FileDescriptor" /> class.</summary>
        /// <param name="fileDescriptorByte">The file descriptor byte that shall be parsed.</param>
        public FileDescriptor(byte fileDescriptorByte) {
            Descriptor = fileDescriptorByte;

            Type = SCardHelper.IsSet(Descriptor, FILE_TYPE_MASK, FILE_TYPE_DF)
                ? FileType.Dedicated
                : FileType.Elementary;

            // ExtendedType
            if (SCardHelper.IsSet(Descriptor, FILE_TYPE_MASK, FILE_TYPE_DF)) {
                ExtendedType = ExtendedFileType.Dedicated;
            } else if (SCardHelper.IsSet(Descriptor, FILE_TYPE_MASK, FILE_TYPE_WORKING_EF)) {
                ExtendedType = ExtendedFileType.WorkingElementary;
            } else if (SCardHelper.IsSet(Descriptor, FILE_TYPE_MASK, FILE_TYPE_INTERNAL_EF)) {
                ExtendedType = ExtendedFileType.InternalElementary;
            } else {
                ExtendedType = ExtendedFileType.Proprietary;
            }

            ShareMode = SCardHelper.IsSet(Descriptor, FILE_SHARE_MODE_MASK, FILE_SHARE_MODE_SHAREABLE)
                ? FileShareMode.Shareable
                : FileShareMode.NotShareable;

            _fileStructureInfoCache = null;
        }

        /// <summary>Determines whether the specified bits are set.</summary>
        /// <param name="mask">The a bit mask.</param>
        /// <param name="bits">The bits to check for.</param>
        /// <returns>
        ///     <c>true</c> if the specified bits are set; otherwise, <c>false</c>.</returns>
        public bool IsSet(byte mask, byte bits) {
            return SCardHelper.IsSet(Descriptor, mask, bits);
        }

        /// <summary>Implicitly converts a <see cref="FileDescriptor" /> to a single byte.</summary>
        /// <param name="fd">The file descriptor.</param>
        /// <returns>A file descriptor as byte.</returns>
        public static implicit operator byte(FileDescriptor fd) {
            return fd.Descriptor;
        }

        /// <summary>Implicitly converts a single byte to a <see cref="FileDescriptor" />.</summary>
        /// <param name="fd">The file descriptor as byte.</param>
        /// <returns>A <see cref="FileDescriptor" /> instance as parsed result.</returns>
        public static implicit operator FileDescriptor(byte fd) {
            return new FileDescriptor(fd);
        }
    }
}