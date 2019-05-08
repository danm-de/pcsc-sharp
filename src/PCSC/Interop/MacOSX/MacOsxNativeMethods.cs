﻿using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop.MacOSX
{
    internal static class MacOsxNativeMethods
    {
        private const string C_LIB = "libc";
        private const string OS_NAME_OSX = "Darwin";
        private static IntPtr _libHandle = IntPtr.Zero;

        public static IntPtr GetSymFromLib(string symName) {
            // Step 1. load dynamic link library
            if (_libHandle == IntPtr.Zero) {
                _libHandle = dlopen(PCSC_LIB, (int) DLOPEN_FLAGS.RTLD_LAZY);
                if (_libHandle.Equals(IntPtr.Zero)) {
                    throw new Exception("PInvoke call dlopen() failed");
                }
            }

            // Step 2. search symbol name in memory
            var symPtr = dlsym(_libHandle, symName);

            if (symPtr.Equals(IntPtr.Zero)) {
                throw new Exception("PInvoke call dlsym() failed");
            }

            return symPtr;
        }

        private static string GetUnameSysName() {
            var utsNameBuffer = new byte[1000];

            if (uname(utsNameBuffer) == 0) {
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

        private const string PCSC_LIB = "PCSC.framework/PCSC";
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
}