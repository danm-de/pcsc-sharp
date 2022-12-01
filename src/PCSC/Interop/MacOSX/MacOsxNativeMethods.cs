using System;
using System.Runtime.InteropServices;
using System.Text;

namespace PCSC.Interop.MacOSX
{
    internal static class MacOsxNativeMethods
    {
        private static IntPtr _libHandle = IntPtr.Zero;
        private const string PCSC_LIB = "PCSC.framework/PCSC";
        private const string DL_LIB = "libdl.dylib";

        public static IntPtr GetSymFromLib(string symName) {
            #if NETSTANDARD
            // Step 1. load dynamic link library
            if (_libHandle == IntPtr.Zero) {
                _libHandle = dlopen(PCSC_LIB, (int) DLOPEN_FLAGS.RTLD_LAZY);
                if (_libHandle.Equals(IntPtr.Zero)) {
                    throw new Exception("PInvoke call dlopen() failed. If you use MacOS Ventura please update to .NET6 SDK or .NET7 SDK.");
                }
            }

            // Step 2. search symbol name in memory
            var symPtr = dlsym(_libHandle, symName);

            if (symPtr.Equals(IntPtr.Zero)) {
                throw new Exception("PInvoke call dlsym() failed");
            }
            return symPtr;
            #else
            // Step 1. load dynamic link library
            if(_libHandle == IntPtr.Zero) {
                if (!NativeLibrary.TryLoad(PCSC_LIB, out _libHandle)) {
                    throw new Exception("NativeLibrary.TryLoad PCSC_LIB failed");
                }
            }

            // Step 2. search symbol name in memory
            var symPtr = NativeLibrary.GetExport(_libHandle, symName);

            if (symPtr.Equals(IntPtr.Zero)) {
                throw new Exception("NativeLibrary.GetExport() failed");
            }

            return symPtr;
            #endif

        }

        [DllImport(PCSC_LIB)]
        internal static extern int SCardEstablishContext(
            [In] int dwScope,
            [In] IntPtr pvReserved1,
            [In] IntPtr pvReserved2,
            [In, Out] ref int phContext);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardReleaseContext(
            [In] int hContext);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardIsValidContext(
            [In] int hContext);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardListReaders(
            [In] int hContext,
            [In] byte[] mszGroups,
            [Out] byte[] mszReaders,
            [In, Out] ref int pcchReaders);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardListReaderGroups(
            [In] int hContext,
            [Out] byte[] mszGroups,
            [In, Out] ref int pcchGroups);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardConnect(
            [In] int hContext,
            [In] byte[] szReader,
            [In] int dwShareMode,
            [In] int dwPreferredProtocols,
            [Out] out int phCard,
            [Out] out int pdwActiveProtocol);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardReconnect(
            [In] int hCard,
            [In] int dwShareMode,
            [In] int dwPreferredProtocols,
            [In] int dwInitialization,
            [Out] out int pdwActiveProtocol);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardDisconnect(
            [In] int hCard,
            [In] int dwDisposition);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardBeginTransaction(
            [In] int hCard);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardEndTransaction(
            [In] int hCard,
            [In] int dwDisposition);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardTransmit(
            [In] int hCard,
            [In] IntPtr pioSendPci,
            [In] byte[] pbSendBuffer,
            [In] int cbSendLength,
            [In, Out] IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            [In, Out] ref int pcbRecvLength);

        [DllImport(PCSC_LIB, EntryPoint = "SCardControl132")]
        internal static extern int SCardControl(
            [In] int hCard,
            [In] int dwControlCode,
            [In] byte[] pbSendBuffer,
            [In] int cbSendLength,
            [Out] byte[] pbRecvBuffer,
            [In] int pcbRecvLength,
            [Out] out int lpBytesReturned);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardStatus(
            [In] int hCard,
            [Out] byte[] szReaderName,
            [In, Out] ref int pcchReaderLen,
            [Out] out int pdwState,
            [Out] out int pdwProtocol,
            [Out] byte[] pbAtr,
            [In, Out] ref int pcbAtrLen);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardGetStatusChange(
            [In] int hContext,
            [In] int dwTimeout,
            [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            SCARD_READERSTATE[] rgReaderStates,
            [In] int cReaders);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardCancel(
            [In] int hContext);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardGetAttrib(
            [In] int hCard,
            [In] int dwAttrId,
            [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
            [In, Out] ref int pcbAttrLen);

        [DllImport(PCSC_LIB)]
        internal static extern int SCardSetAttrib(
            [In] int hCard,
            [In] int dwAttrId,
            [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 3)]
            byte[] pbAttr,
            [In] int cbAttrLen);

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
