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
    public class FileDescriptor
    {
        private const byte FILE_TYPE_MASK           = (1 << 7) + (1 << 5) + (1 << 4) + (1 << 3);
        private const byte FILE_TYPE_DF             = (1 << 5) + (1 << 4) + (1 << 3);
        private const byte FILE_TYPE_WORKING_EF     = (0 << 5) + (0 << 4) + (0 << 3);
        private const byte FILE_TYPE_INTERNAL_EF    = (0 << 5) + (0 << 4) + (1 << 3);

       
        private const byte FILE_SHARE_MODE_MASK         = (1 << 7) + (1 << 6);
        private const byte FILE_SHARE_MODE_SHAREABLE    = (1 << 6);
        private const byte FILE_SHARE_MODE_NOTSHAREABLE = (0 << 6);



        private byte filedescriptor;

        public FileDescriptor(byte fileDescriptorByte)
        {
            this.filedescriptor = fileDescriptorByte;
            UpdateFileInfo();
        }

        public byte Descriptor
        {
            get { return filedescriptor; }
        }

        protected void UpdateFileInfo()
        {
   
            // Type
            if (SCardHelper.IsSet(filedescriptor, FILE_TYPE_MASK, FILE_TYPE_DF))
                _type = FileType.Dedicated;
            else
                _type = FileType.Elementary;

            // ExtendedType
            if (SCardHelper.IsSet(filedescriptor, FILE_TYPE_MASK, FILE_TYPE_DF))
                _extendedtype = ExtendedFileType.Dedicated;
            else if (SCardHelper.IsSet(filedescriptor, FILE_TYPE_MASK, FILE_TYPE_WORKING_EF))
                _extendedtype = ExtendedFileType.WorkingElementary;
            else if (SCardHelper.IsSet(filedescriptor, FILE_TYPE_MASK, FILE_TYPE_INTERNAL_EF))
                _extendedtype = ExtendedFileType.InternalElementary;
            else _extendedtype = ExtendedFileType.Proprietary;

            // ShareMode
            if (SCardHelper.IsSet(filedescriptor, FILE_SHARE_MODE_MASK, FILE_SHARE_MODE_SHAREABLE))
                _sharemode = FileShareMode.Shareable;
            else
                _sharemode = FileShareMode.NotShareable;

            _filestructureinfo = null;
        }

        private FileType _type;
        public FileType Type
        {
            get { return _type; }
        }

        private ExtendedFileType _extendedtype;
        public ExtendedFileType ExtendedType
        {
            get { return _extendedtype; }
        }

        private FileShareMode _sharemode;
        public FileShareMode ShareMode
        {
            get { return _sharemode; }
        }

        private FileStructureInfo _filestructureinfo;
        public FileStructureInfo Structure
        {
            get
            {
                if (_filestructureinfo == null)
                {
                    if (_extendedtype == ExtendedFileType.InternalElementary ||
                        _extendedtype == ExtendedFileType.WorkingElementary)
                    {
                        _filestructureinfo = new FileStructureInfo(filedescriptor);
                    }
                }
                return _filestructureinfo;
            }
        }

        public bool IsSet(byte mask, byte bits)
        {
            return SCardHelper.IsSet(filedescriptor, mask, bits);
        }

        public static implicit operator byte(FileDescriptor fd)
        {
            return fd.filedescriptor;
        }
        public static implicit operator FileDescriptor(byte b)
        {
            return new FileDescriptor(b);
        }

    }
}
