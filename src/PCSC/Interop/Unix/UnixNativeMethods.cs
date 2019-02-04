using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using PCSC.Interop.Unix.ExtensionMethods;

namespace PCSC.Interop.Unix
{


    internal static class UnixNativeMethods
    {
        enum OSUnixPlatform
        {
            Linux,
            OSX
        }

        private const string C_LIB = "libc";
        private const string OS_NAME_OSX = "Darwin";

        private readonly static OSUnixPlatform _osUnixPlatform = GetUnameSysName() == OS_NAME_OSX ? OSUnixPlatform.OSX : OSUnixPlatform.Linux;

        private static IntPtr _libHandle = IntPtr.Zero;

        public static string PCSC_LIB
        {
            get
            {
                if (_osUnixPlatform == OSUnixPlatform.Linux)
                {
                    return LinuxNativeMethods.PCSC_LIB;
                }
                else if (_osUnixPlatform == OSUnixPlatform.OSX)
                {
                    return OSXNativeMethods.PCSC_LIB;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
        }

        #region Common API
        internal static IntPtr SCardEstablishContext(
                [In] IntPtr dwScope,
                [In] IntPtr pvReserved1,
                [In] IntPtr pvReserved2,
                [In, Out] ref IntPtr phContext)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardEstablishContext(
                    dwScope,
                    pvReserved1,
                    pvReserved2,
                    ref phContext);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardEstablishContext(
                    dwScope,
                    pvReserved1,
                    pvReserved2,
                    ref phContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardReleaseContext(
                [In] IntPtr hContext)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardReleaseContext(hContext);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardReleaseContext(hContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardIsValidContext(
              [In] IntPtr hContext)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardIsValidContext(hContext);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardIsValidContext(hContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardListReaders(
                [In] IntPtr hContext,
                [In] byte[] mszGroups,
                [Out] byte[] mszReaders,
                [In, Out] ref IntPtr pcchReaders)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardListReaders(hContext, mszGroups, mszReaders, ref pcchReaders);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardListReaders(hContext, mszGroups, mszReaders, ref pcchReaders);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardListReaderGroups(
                [In] IntPtr hContext,
                [Out] byte[] mszGroups,
                [In, Out] ref IntPtr pcchGroups)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardListReaderGroups(hContext, mszGroups, ref pcchGroups);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardListReaderGroups(hContext, mszGroups, ref pcchGroups); ;
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        internal static IntPtr SCardConnect(
                [In] IntPtr hContext,
                [In] byte[] szReader,
                [In] IntPtr dwShareMode,
                [In] IntPtr dwPreferredProtocols,
                [Out] out IntPtr phCard,
                [Out] out IntPtr pdwActiveProtocol)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardConnect(hContext, szReader, dwShareMode, dwPreferredProtocols, out phCard, out pdwActiveProtocol);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardConnect(hContext, szReader, dwShareMode, dwPreferredProtocols, out phCard, out pdwActiveProtocol);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardReconnect(
               [In] IntPtr hCard,
               [In] IntPtr dwShareMode,
               [In] IntPtr dwPreferredProtocols,
               [In] IntPtr dwInitialization,
               [Out] out IntPtr pdwActiveProtocol)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardReconnect(hCard, dwShareMode, dwPreferredProtocols, dwInitialization, out pdwActiveProtocol);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardReconnect(hCard, dwShareMode, dwPreferredProtocols, dwInitialization, out pdwActiveProtocol);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardDisconnect(
               [In] IntPtr hCard,
               [In] IntPtr dwDisposition)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardDisconnect(hCard, dwDisposition);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardDisconnect(hCard, dwDisposition);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        internal static IntPtr SCardBeginTransaction(
                [In] IntPtr hCard)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardBeginTransaction(hCard);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardBeginTransaction(hCard);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardEndTransaction(
               [In] IntPtr hCard,
               [In] IntPtr dwDisposition)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardEndTransaction(hCard, dwDisposition);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardEndTransaction(hCard, dwDisposition);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        internal static IntPtr SCardTransmit(
                [In] IntPtr hCard,
                [In] IntPtr pioSendPci,
                [In] byte[] pbSendBuffer,
                [In] IntPtr cbSendLength,
                [In, Out] IntPtr pioRecvPci,
                [Out] byte[] pbRecvBuffer,
                [In, Out] ref IntPtr pcbRecvLength)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardTransmit(hCard, pioSendPci, pbSendBuffer, cbSendLength, pioRecvPci, pbRecvBuffer, ref pcbRecvLength);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardTransmit(hCard, pioSendPci, pbSendBuffer, cbSendLength, pioRecvPci, pbRecvBuffer, ref pcbRecvLength); ;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardControl(
                [In] IntPtr hCard,
                [In] IntPtr dwControlCode,
                [In] byte[] pbSendBuffer,
                [In] IntPtr cbSendLength,
                [Out] byte[] pbRecvBuffer,
                [In] IntPtr pcbRecvLength,
                [Out] out IntPtr lpBytesReturned)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardControl(hCard, dwControlCode, pbSendBuffer, cbSendLength, pbRecvBuffer, pcbRecvLength, out lpBytesReturned);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardControl(hCard, dwControlCode, pbSendBuffer, cbSendLength, pbRecvBuffer, pcbRecvLength, out lpBytesReturned);
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        internal static IntPtr SCardStatus(
               [In] IntPtr hCard,
               [Out] byte[] szReaderName,
               [In, Out] ref IntPtr pcchReaderLen,
               [Out] out IntPtr pdwState,
               [Out] out IntPtr pdwProtocol,
               [Out] byte[] pbAtr,
               [In, Out] ref IntPtr pcbAtrLen)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardStatus(hCard, szReaderName, ref pcchReaderLen, out pdwState, out pdwProtocol, pbAtr, ref pcbAtrLen);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardStatus(hCard, szReaderName, ref pcchReaderLen, out pdwState, out pdwProtocol, pbAtr, ref pcbAtrLen);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardGetStatusChange(
                       [In] IntPtr hContext,
                       [In] IntPtr dwTimeout,
                       [In, Out] SCARD_READERSTATE[] rgReaderStates,
                       [In] IntPtr cReaders)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardGetStatusChange(hContext, dwTimeout, rgReaderStates, cReaders);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardGetStatusChange(hContext, dwTimeout, rgReaderStates, cReaders);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardCancel(
                [In] IntPtr hContext)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardCancel(hContext);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardCancel(hContext);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardGetAttrib(
               [In] IntPtr hCard,
               [In] IntPtr dwAttrId,
               [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
               [In, Out] ref IntPtr pcbAttrLen)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardGetAttrib(hCard, dwAttrId, pbAttr, ref pcbAttrLen);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardGetAttrib(hCard, dwAttrId, pbAttr, ref pcbAttrLen);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardSetAttrib(
                [In] IntPtr hCard,
                [In] IntPtr dwAttrId,
                [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
                [In] IntPtr cbAttrLen)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardSetAttrib(hCard, dwAttrId, pbAttr, cbAttrLen);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardSetAttrib(hCard, dwAttrId, pbAttr, cbAttrLen);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr SCardFreeMemory(
             [In] IntPtr hContext,
             [In] IntPtr pvMem)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.SCardFreeMemory(hContext, pvMem);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.SCardFreeMemory(hContext, pvMem);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr dlopen(
                       [In] string szFilename,
                       [In] int flag)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.dlopen(szFilename, flag);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.dlopen(szFilename, flag);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static IntPtr dlsym(
                       [In] IntPtr handle,
                       [In] string szSymbol)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.dlsym(handle, szSymbol);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.dlsym(handle, szSymbol);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        internal static int dlclose(
                       [In] IntPtr handle)
        {
            if (_osUnixPlatform == OSUnixPlatform.Linux)
            {
                return LinuxNativeMethods.dlclose(handle);
            }
            else if (_osUnixPlatform == OSUnixPlatform.OSX)
            {
                return OSXNativeMethods.dlclose(handle);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public static IntPtr GetSymFromLib(string symName)
        {
            // Step 1. load dynamic link library
            if (_libHandle == IntPtr.Zero)
            {
                _libHandle = dlopen(PCSC_LIB, (int)DLOPEN_FLAGS.RTLD_LAZY);
                if (_libHandle.Equals(IntPtr.Zero))
                {
                    throw new Exception("PInvoke call dlopen() failed");
                }
            }

            // Step 2. search symbol name in memory
            var symPtr = dlsym(_libHandle, symName);

            if (symPtr.Equals(IntPtr.Zero))
            {
                throw new Exception("PInvoke call dlsym() failed");
            }

            return symPtr;
        }

        #endregion


        #region Unix Native Methods

        private static string GetUnameSysName()
        {
            var utsNameBuffer = new byte[1000];

            if (uname(utsNameBuffer) == 0)
            {
                int terminator;

                // Find the null terminator of the first string in struct utsname.
                for (terminator = 0;
                    terminator < utsNameBuffer.Length && utsNameBuffer[terminator] != 0;
                    terminator++) ;

                return Encoding.ASCII.GetString(utsNameBuffer, 0, terminator);
            }

            return null;
        }

        [DllImport(C_LIB, CharSet = CharSet.Ansi)]
        private static extern int uname(
            [Out] byte[] buffer);

        #endregion

        #region Linux Specific
        static class LinuxNativeMethods
        {
            internal const string PCSC_LIB = "libpcsclite.so.1";
            private const string DL_LIB = "libdl.so.2";

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardEstablishContext(
                [In] IntPtr dwScope,
                [In] IntPtr pvReserved1,
                [In] IntPtr pvReserved2,
                [In, Out] ref IntPtr phContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardReleaseContext(
                [In] IntPtr hContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardIsValidContext(
              [In] IntPtr hContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardListReaders(
                [In] IntPtr hContext,
                [In] byte[] mszGroups,
                [Out] byte[] mszReaders,
                [In, Out] ref IntPtr pcchReaders);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardListReaderGroups(
                [In] IntPtr hContext,
                [Out] byte[] mszGroups,
                [In, Out] ref IntPtr pcchGroups);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardConnect(
                [In] IntPtr hContext,
                [In] byte[] szReader,
                [In] IntPtr dwShareMode,
                [In] IntPtr dwPreferredProtocols,
                [Out] out IntPtr phCard,
                [Out] out IntPtr pdwActiveProtocol);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardReconnect(
               [In] IntPtr hCard,
               [In] IntPtr dwShareMode,
               [In] IntPtr dwPreferredProtocols,
               [In] IntPtr dwInitialization,
               [Out] out IntPtr pdwActiveProtocol);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardDisconnect(
               [In] IntPtr hCard,
               [In] IntPtr dwDisposition);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardBeginTransaction(
                [In] IntPtr hCard);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardEndTransaction(
               [In] IntPtr hCard,
               [In] IntPtr dwDisposition);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardTransmit(
                [In] IntPtr hCard,
                [In] IntPtr pioSendPci,
                [In] byte[] pbSendBuffer,
                [In] IntPtr cbSendLength,
                [In, Out] IntPtr pioRecvPci,
                [Out] byte[] pbRecvBuffer,
                [In, Out] ref IntPtr pcbRecvLength);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardControl(
                [In] IntPtr hCard,
                [In] IntPtr dwControlCode,
                [In] byte[] pbSendBuffer,
                [In] IntPtr cbSendLength,
                [Out] byte[] pbRecvBuffer,
                [In] IntPtr pcbRecvLength,
                [Out] out IntPtr lpBytesReturned);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardStatus(
               [In] IntPtr hCard,
               [Out] byte[] szReaderName,
               [In, Out] ref IntPtr pcchReaderLen,
               [Out] out IntPtr pdwState,
               [Out] out IntPtr pdwProtocol,
               [Out] byte[] pbAtr,
               [In, Out] ref IntPtr pcbAtrLen);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardGetStatusChange(
               [In] IntPtr hContext,
               [In] IntPtr dwTimeout,
               [In, Out] SCARD_READERSTATE[] rgReaderStates,
               [In] IntPtr cReaders);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardCancel(
                [In] IntPtr hContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardGetAttrib(
               [In] IntPtr hCard,
               [In] IntPtr dwAttrId,
               [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
               [In, Out] ref IntPtr pcbAttrLen);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardSetAttrib(
                [In] IntPtr hCard,
                [In] IntPtr dwAttrId,
                [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
                [In] IntPtr cbAttrLen);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardFreeMemory(
             [In] IntPtr hContext,
             [In] IntPtr pvMem);

            [DllImport(DL_LIB)]
            internal static extern IntPtr dlopen(
               [In] string szFilename,
               [In] int flag);

            [DllImport(DL_LIB)]
            internal static extern IntPtr dlsym(
               [In] IntPtr handle,
               [In] string szSymbol);

            [DllImport(DL_LIB)]
            internal static extern int dlclose(
               [In] IntPtr handle);
        }

        #endregion

        #region Osx Specific
        static class OSXNativeMethods
        {
            internal const string PCSC_LIB = "PCSC.framework/PCSC";
            private const string DL_LIB = "libdl.dylib";

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardEstablishContext(
                [In] IntPtr dwScope,
                [In] IntPtr pvReserved1,
                [In] IntPtr pvReserved2,
                [In, Out] ref IntPtr phContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardReleaseContext(
                [In] IntPtr hContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardIsValidContext(
              [In] IntPtr hContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardListReaders(
                [In] IntPtr hContext,
                [In] byte[] mszGroups,
                [Out] byte[] mszReaders,
                [In, Out] ref IntPtr pcchReaders);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardListReaderGroups(
                [In] IntPtr hContext,
                [Out] byte[] mszGroups,
                [In, Out] ref IntPtr pcchGroups);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardConnect(
                [In] IntPtr hContext,
                [In] byte[] szReader,
                [In] IntPtr dwShareMode,
                [In] IntPtr dwPreferredProtocols,
                [Out] out IntPtr phCard,
                [Out] out IntPtr pdwActiveProtocol);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardReconnect(
               [In] IntPtr hCard,
               [In] IntPtr dwShareMode,
               [In] IntPtr dwPreferredProtocols,
               [In] IntPtr dwInitialization,
               [Out] out IntPtr pdwActiveProtocol);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardDisconnect(
               [In] IntPtr hCard,
               [In] IntPtr dwDisposition);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardBeginTransaction(
                [In] IntPtr hCard);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardEndTransaction(
               [In] IntPtr hCard,
               [In] IntPtr dwDisposition);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardTransmit(
                [In] IntPtr hCard,
                [In] IntPtr pioSendPci,
                [In] byte[] pbSendBuffer,
                [In] IntPtr cbSendLength,
                [In, Out] IntPtr pioRecvPci,
                [Out] byte[] pbRecvBuffer,
                [In, Out] ref IntPtr pcbRecvLength);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardControl(
                [In] IntPtr hCard,
                [In] IntPtr dwControlCode,
                [In] byte[] pbSendBuffer,
                [In] IntPtr cbSendLength,
                [Out] byte[] pbRecvBuffer,
                [In] IntPtr pcbRecvLength,
                [Out] out IntPtr lpBytesReturned);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardStatus(
               [In] IntPtr hCard,
               [Out] byte[] szReaderName,
               [In, Out] ref IntPtr pcchReaderLen,
               [Out] out IntPtr pdwState,
               [Out] out IntPtr pdwProtocol,
               [Out] byte[] pbAtr,
               [In, Out] ref IntPtr pcbAtrLen);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardGetStatusChange(
               [In] IntPtr hContext,
               [In] IntPtr dwTimeout,
               [In, Out] SCARD_READERSTATE[] rgReaderStates,
               [In] IntPtr cReaders);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardCancel(
                [In] IntPtr hContext);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardGetAttrib(
               [In] IntPtr hCard,
               [In] IntPtr dwAttrId,
               [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
               [In, Out] ref IntPtr pcbAttrLen);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardSetAttrib(
                [In] IntPtr hCard,
                [In] IntPtr dwAttrId,
                [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
                [In] IntPtr cbAttrLen);

            [DllImport(PCSC_LIB)]
            internal static extern IntPtr SCardFreeMemory(
             [In] IntPtr hContext,
             [In] IntPtr pvMem);

            [DllImport(DL_LIB)]
            internal static extern IntPtr dlopen(
               [In] string szFilename,
               [In] int flag);

            [DllImport(DL_LIB)]
            internal static extern IntPtr dlsym(
               [In] IntPtr handle,
               [In] string szSymbol);

            [DllImport(DL_LIB)]
            internal static extern int dlclose(
               [In] IntPtr handle);
        }
        #endregion
    }
}
