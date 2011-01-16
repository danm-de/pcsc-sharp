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
    public class RecordInfo
    {
        private const byte FILE_STRUCTURE_IS_MASK = (1 << 7) + (1 << 2) + (1 << 1);
        private const byte FILE_STRUCTURE_IS_LINEAR_FIXED = (0 << 2) + (1 << 1);
        private const byte FILE_STRUCTURE_IS_LINEAR_VARIABLE = (1 << 2) + (0 << 1);
        private const byte FILE_STRUCTURE_IS_CYCLIC = (1 << 2) + (1 << 1);

        private byte filedescriptor;

        internal RecordInfo(byte fileDesc)
        {
            this.filedescriptor = fileDesc;
            UpdateFileInfo();
        }

        protected void UpdateFileInfo()
        {
            _iscyclic = false;
            _islinear = false;
            _isfixed = false;
            _isvariable = false;
            _issimpletlv = false;

            // IsCyclic
            if (SCardHelper.IsSet(filedescriptor, FILE_STRUCTURE_IS_MASK, FILE_STRUCTURE_IS_CYCLIC))
            {
                _iscyclic = true;

                // IsFixed
                _isfixed = true;
            }

            // IsLinear
            if (!_iscyclic)
            {
                _islinear = true;

                // IsFixed
                if (SCardHelper.IsSet(filedescriptor, FILE_STRUCTURE_IS_MASK, FILE_STRUCTURE_IS_LINEAR_FIXED))
                    _isfixed = true;

                // IsVariable
                if (SCardHelper.IsSet(filedescriptor, FILE_STRUCTURE_IS_MASK, FILE_STRUCTURE_IS_LINEAR_VARIABLE))
                    _isvariable = true;
            }

            // IsSimpleTlv
            if (SCardHelper.IsSet(filedescriptor, 1, 1))
                _issimpletlv = true;
        }

        private bool _iscyclic;
        public bool IsCyclic
        {
            get { return _iscyclic; }
        }

        private bool _islinear;
        public bool IsLinear
        {
            get { return _islinear; }
        }

        private bool _isfixed;
        public bool IsFixed
        {
            get { return _isfixed; }
        }

        private bool _isvariable;
        public bool IsVariable
        {
            get { return _isvariable; }
        }

        private bool _issimpletlv;
        public bool IsSimpleTlv
        {
            get { return _issimpletlv; }

        }
    }
}
