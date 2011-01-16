/*
Copyright (C) 2010
    Daniel Mueller <daniel@danm.de>

All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.
2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.
3. The name of the author may not be used to endorse or promote products
   derived from this software without specific prior written permission.

Changes to this license can be made only by the copyright author with
explicit written consent.

THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES
OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT
NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace PCSC.Iso7816
{
    public class FileStructureInfo
    {
        private const byte FILE_STRUCTURE_MASK = (1 << 7) + (1 << 2) + (1 << 1) + (1 << 0);
        private const byte FILE_STRUCTURE_NO_INFO = (0 << 2) + (0 << 1) + (0 << 0);
        private const byte FILE_STRUCTURE_TRANSPARENT = (0 << 2) + (0 << 1) + (1 << 0);
        private const byte FILE_STRUCTURE_LINEAR_FIXED = (0 << 2) + (1 << 1) + (0 << 0);
        private const byte FILE_STRUCTURE_LINEAR_FIXED_TLV = (0 << 2) + (1 << 1) + (1 << 0);
        private const byte FILE_STRUCTURE_LINEAR_VARIABLE = (1 << 2) + (0 << 1) + (0 << 0);
        private const byte FILE_STRUCTURE_LINEAR_VARIABLE_TLV = (1 << 2) + (0 << 1) + (1 << 0);
        private const byte FILE_STRUCTURE_CYCLIC = (1 << 2) + (1 << 1) + (0 << 0);
        private const byte FILE_STRUCTURE_CYCLIC_TLV = (1 << 2) + (1 << 1) + (1 << 0);

        private byte filedescriptor;

        internal FileStructureInfo(byte fileDescriptor)
        {
            this.filedescriptor = fileDescriptor;
            UpdateFileInfo();
        }

        protected void UpdateFileInfo()
        {
            _istransparent = false;
            _isrecord = false;

            // StructureType
            if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_NO_INFO))
                _structuretype = FileStructureType.NoInformation;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_TRANSPARENT))
                _structuretype = FileStructureType.Transparent;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_FIXED))
                _structuretype = FileStructureType.LinearFixed;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_FIXED_TLV))
                _structuretype = FileStructureType.LinearFixedSimpleTlv;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_VARIABLE))
                _structuretype = FileStructureType.LinearVariable;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_LINEAR_VARIABLE_TLV))
                _structuretype = FileStructureType.LinearFixedSimpleTlv;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_CYCLIC))
                _structuretype = FileStructureType.Cyclic;
            else if (_IsSet(filedescriptor, FILE_STRUCTURE_MASK, FILE_STRUCTURE_CYCLIC_TLV))
                _structuretype = FileStructureType.CyclicSimpleTlv;

            // IsTransparent
            if (_structuretype == FileStructureType.Transparent)
                _istransparent = true;
            else
                if (!(_structuretype == FileStructureType.NoInformation))
                    // IsRecord
                    _isrecord = true;
        }

        private FileStructureType _structuretype;
        public FileStructureType Type
        {
            get { return _structuretype; }
        }

        private bool _istransparent;
        public bool IsTransparent
        {
            get { return _istransparent; }
        }

        private RecordInfo _recordinfo;
        public RecordInfo RecordInfo
        {
            get
            {
                if (_recordinfo == null)
                {
                    if (_isrecord)
                        _recordinfo = new RecordInfo(filedescriptor);
                }
                return _recordinfo;
            }
        }

        private bool _isrecord;
        public bool IsRecord
        {
            get { return _isrecord; }
        }

        private bool _IsSet(byte value, byte mask, byte bits)
        {
            return ((value & mask) == bits) ? true : false;
        }
    }
}
