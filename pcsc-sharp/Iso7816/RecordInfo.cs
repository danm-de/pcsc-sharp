namespace PCSC.Iso7816
{
    public class RecordInfo
    {
        public const byte FILE_STRUCTURE_IS_MASK            = (1 << 7) + (1 << 2) + (1 << 1);
        public const byte FILE_STRUCTURE_IS_LINEAR_FIXED    = (0 << 2) + (1 << 1);
        public const byte FILE_STRUCTURE_IS_LINEAR_VARIABLE = (1 << 2) + (0 << 1);
        public const byte FILE_STRUCTURE_IS_CYCLIC          = (1 << 2) + (1 << 1);

        private readonly bool _iscyclic;
        private readonly byte _fileDescriptor;
        private readonly bool _islinear;
        private readonly bool _isfixed;
        private readonly bool _isvariable;
        private readonly bool _issimpletlv;

        public RecordInfo(byte fileDescriptor) {
            _fileDescriptor = fileDescriptor;

            _iscyclic = false;
            _islinear = false;
            _isfixed = false;
            _isvariable = false;
            _issimpletlv = false;

            // IsCyclic
            if (SCardHelper.IsSet(fileDescriptor, FILE_STRUCTURE_IS_MASK, FILE_STRUCTURE_IS_CYCLIC)) {
                _iscyclic = true;

                // IsFixed
                _isfixed = true;
            }

            // IsLinear
            if (!_iscyclic) {
                _islinear = true;

                // IsFixed
                if (SCardHelper.IsSet(fileDescriptor, FILE_STRUCTURE_IS_MASK, FILE_STRUCTURE_IS_LINEAR_FIXED)) {
                    _isfixed = true;
                }

                // IsVariable
                if (SCardHelper.IsSet(fileDescriptor, FILE_STRUCTURE_IS_MASK, FILE_STRUCTURE_IS_LINEAR_VARIABLE)) {
                    _isvariable = true;
                }
            }

            // IsSimpleTlv
            if (SCardHelper.IsSet(fileDescriptor, 1, 1)) {
                _issimpletlv = true;
            }
        }

        public bool IsCyclic {
            get { return _iscyclic; }
        }

        public bool IsLinear {
            get { return _islinear; }
        }

        public bool IsFixed {
            get { return _isfixed; }
        }
        public bool IsVariable {
            get { return _isvariable; }
        }

        public bool IsSimpleTlv {
            get { return _issimpletlv; }
        }
        public byte FileDescriptor {
            get { return _fileDescriptor; }
        }
    }
}