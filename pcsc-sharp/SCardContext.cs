/*
Copyright (c) 2010 
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
using System.ComponentModel;
using System.Reflection;
using System.Runtime.InteropServices;

using PCSC.Interop;

namespace PCSC
{
    public enum SCardScope : int
    {
        [DescriptionAttribute("Scope in user space")]
        User = 0x0000,
        [DescriptionAttribute("Scope in terminal")]
        Terminal = 0x0001,
        [DescriptionAttribute("Scope in system")]
        System = 0x0002,

        /** PC/SC Lite specific extensions */
        [DescriptionAttribute("Scope is global")]
        Global = 0x0003
    }

    public class SCardContext : IDisposable
    {
        internal bool hasContext = false;
        internal IntPtr contextPtr;
        internal SCardScope lastScope;

        public SCardContext()
        {
        }

        ~SCardContext()
        {
            Dispose();
        }

        public void Establish(SCardScope scope)
        {
            if (hasContext)
                if (IsValid())
                    Release();

            SCardError rc;
            IntPtr hContext = IntPtr.Zero;

            rc = SCardAPI.Lib.EstablishContext(scope,
                IntPtr.Zero,
                IntPtr.Zero,
                ref hContext);

            if (rc == SCardError.Success)
            {
                contextPtr = hContext;
                lastScope = scope;
                hasContext = true;
            }
            else
            {
                if (rc == SCardError.InvalidValue)
                    throw new InvalidScopeTypeException(rc, "Invalid scope type passed");
                else
                    throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }

        }

        public void Release()
        {
            if (!hasContext)
                throw new InvalidContextException(SCardError.UnknownError, "Context was not established");

            SCardError rc;

            rc = SCardAPI.Lib.ReleaseContext(contextPtr);

            if (rc == SCardError.Success)
            {
                contextPtr = IntPtr.Zero;
                hasContext = false;
            }
            else
            {
                if (rc == SCardError.InvalidHandle)
                    throw new InvalidContextException(rc, "Invalid Context handle");
                else
                    throw new PCSCException(rc, SCardHelper.StringifyError(rc));
            }
        }

        public SCardError CheckValidity()
        {
            return SCardAPI.Lib.IsValidContext(contextPtr);
        }

        public bool IsValid()
        {
            SCardError rc = CheckValidity();
            if (rc == SCardError.Success)
                return true;
            else
                return false;
        }

        public void ReEstablish()
        {
            Establish(lastScope);
        }

        public void Dispose()
        {
            if (hasContext)
                Release();

            GC.SuppressFinalize(this);

            return;
        }

        public string[] GetReaders(string[] Groups)
        {
            if (contextPtr.Equals(IntPtr.Zero))
                throw new InvalidContextException(SCardError.InvalidHandle);

            SCardError rc;
            string[] readers;

            rc = SCardAPI.Lib.ListReaders(
                contextPtr,
                Groups,
                out readers);

            if (rc == SCardError.Success)
                return readers;
            if (rc == SCardError.InvalidHandle)
                throw new InvalidContextException(rc, "Invalid Scope Handle");
            else
                throw new PCSCException(rc, SCardHelper.StringifyError(rc));
        }
        public string[] GetReaders()
        {
            return GetReaders(null);
        }

        public string[] GetReaderGroups()
        {
            if (contextPtr.Equals(IntPtr.Zero))
                throw new InvalidContextException(SCardError.InvalidHandle);
            
            string[] groups;
            
            SCardError sc = SCardAPI.Lib.ListReaderGroups(
                contextPtr,
                out groups);

            if (sc == SCardError.Success)
                return groups;
            else if (sc == SCardError.InvalidHandle)
                throw new InvalidContextException(sc, "Invalid Scope Handle");
            else
                throw new PCSCException(sc, SCardHelper.StringifyError(sc));
        }

        public SCardReaderState GetReaderStatus(string readername)
        {
            string[] tmp_readers;
            if (readername != null)
                tmp_readers = new string[] { readername };
            else
                tmp_readers = new string[0];

            return GetReaderStatus(tmp_readers)[0];
        }

        public SCardReaderState[] GetReaderStatus(string[] readernames)
        {
            SCardError rc;

            if (readernames == null)
                throw new ArgumentNullException("readernames");
            if (readernames.Length == 0)
                throw new ArgumentException("You must specify at least one reader.");

            SCardReaderState[] states = new SCardReaderState[readernames.Length];
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = new SCardReaderState();
                states[i].ReaderName = readernames[i];
                states[i].CurrentState = SCRState.Unaware;
            }

            rc = GetStatusChange(IntPtr.Zero, states);
            if (rc != SCardError.Success)
                throw new PCSCException(rc, SCardHelper.StringifyError(rc));

            return states;
        }

        public SCardError GetStatusChange(
            IntPtr timeout,
            SCardReaderState[] readerstates)
        {
            if (contextPtr.Equals(IntPtr.Zero))
                throw new InvalidContextException(SCardError.InvalidHandle);

            SCardError rc = SCardAPI.Lib.GetStatusChange(
                contextPtr,
                timeout,
                readerstates);

            return rc;
        }

        public SCardError Cancel()
        {
            if (contextPtr.Equals(IntPtr.Zero))
                throw new InvalidContextException(SCardError.UnknownError, "Invalid connection context.");

            SCardError rc = SCardAPI.Lib.Cancel(contextPtr);
            return rc;
        }

        public static int MaxATRSize
        {
            get { return SCardAPI.Lib.MaxATRSize; }
        }
        public IntPtr Handle
        {
            get { return contextPtr; }
        }
        public static IntPtr Infinite
        {
            get
            {
                // Hack to avoid Overflow exception on Windows 7 32bit
                if (Marshal.SizeOf(typeof(IntPtr)) == 4)
                    return unchecked((IntPtr)(Int32)0xFFFFFFFF);
                else
                    return unchecked((IntPtr)0xFFFFFFFF);
            }
        }
    }
}
