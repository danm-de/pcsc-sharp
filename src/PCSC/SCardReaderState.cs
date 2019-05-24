using System;
using System.Runtime.InteropServices;
using PCSC.Interop;
using PCSC.Interop.Linux;
using PCSC.Interop.MacOSX;
using PCSC.Interop.Windows;
using PCSC.Utils;
using WINDOWS_SCARD_READERSTATE = PCSC.Interop.Windows.SCARD_READERSTATE;
using LINUX_SCARD_READERSTATE = PCSC.Interop.Linux.SCARD_READERSTATE;
using MACOSX_SCARD_READERSTATE = PCSC.Interop.MacOSX.SCARD_READERSTATE;

namespace PCSC
{
    /// <summary>A structures that contains the old and the new Smart Card reader status.</summary>
    /// <remarks>
    ///     <para>Is used as parameter in <see cref="M:PCSC.ISCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" />.</para>
    ///     <para>The new event state will be contained in <see cref="SCardReaderState.EventState" />. A status change might be a card insertion or removal event, a change in ATR, etc. To wait for a reader event (reader added or removed) you may use the special reader name "\\?PnP?\Notification". If a reader event occurs the state of this reader will change and the bit <see cref="F:PCSC.SCRState.Changed" /> will be set.</para>
    /// </remarks>
    public class SCardReaderState : IDisposable
    {
        // we're getting values greater than 0xFFFF back from SCardGetStatusChange 
        private const int EVENTSTATE_RANGE = 0xFFFF;
        private const long CHCOUNT_RANGE = 0xFFFF0000;

        private WINDOWS_SCARD_READERSTATE _winscardRstate;
        private LINUX_SCARD_READERSTATE _linuxRstate;
        private MACOSX_SCARD_READERSTATE _macosxRstate;

        private IntPtr _pReaderName = IntPtr.Zero;
        private int _pReaderNameSize;

        /// <summary>
        /// Frees unmanaged resources.
        /// </summary>
        ~SCardReaderState() {
            Dispose(false);
        }

        /// <summary>
        /// Frees unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Frees unmanaged resources.
        /// </summary>
        /// <param name="disposing">Ignored.</param>
        protected virtual void Dispose(bool disposing) {
            // Free unmanaged resources
            if (_pReaderName != IntPtr.Zero) {
                // Free unmanaged memory
                Marshal.FreeCoTaskMem(_pReaderName);
                _pReaderName = IntPtr.Zero;
                _pReaderNameSize = 0;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SCardReaderState"/> class.
        /// </summary>
        public SCardReaderState() {
            switch (Platform.Type) {
                case PlatformType.Windows:
                    _winscardRstate = new WINDOWS_SCARD_READERSTATE {
                        // initialize embedded array
                        rgbAtr = new byte[WinSCardAPI.MAX_ATR_SIZE],
                        cbAtr = WinSCardAPI.MAX_ATR_SIZE
                    };
                    break;
                case PlatformType.Linux:
                    _linuxRstate = new LINUX_SCARD_READERSTATE {
                        // initialize embedded array
                        rgbAtr = new byte[PCSCliteLinux.MAX_ATR_SIZE],
                        cbAtr = (IntPtr) PCSCliteLinux.MAX_ATR_SIZE
                    };
                    break;
                case PlatformType.MacOSX:
                    _macosxRstate = new MACOSX_SCARD_READERSTATE {
                        // initialize embedded array
                        rgbAtr = new byte[PCSCliteLinux.MAX_ATR_SIZE],
                        cbAtr = PCSCliteLinux.MAX_ATR_SIZE
                    };
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// User defined data.
        /// </summary>
        public long UserData {
            get { return UserDataPointer.ToInt64(); }
            set { UserDataPointer = unchecked((IntPtr) value); }
        }

        /// <summary>
        /// User defined data.
        /// </summary>
        public IntPtr UserDataPointer {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return _winscardRstate.pvUserData;
                    case PlatformType.Linux:
                        return _linuxRstate.pvUserData;
                    case PlatformType.MacOSX:
                        return _macosxRstate.pvUserData;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.pvUserData = value;
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.pvUserData = value;
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.pvUserData = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Current state of reader.
        /// </summary>
        public SCRState CurrentState {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return SCardHelper.ToSCRState(_winscardRstate.dwCurrentState & EVENTSTATE_RANGE);
                    case PlatformType.Linux:
                        return SCardHelper.ToSCRState(_linuxRstate.dwCurrentState.ToInt64() & EVENTSTATE_RANGE);
                    case PlatformType.MacOSX:
                        return SCardHelper.ToSCRState(_macosxRstate.dwCurrentState & EVENTSTATE_RANGE);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.dwCurrentState =
                            (int) value & EVENTSTATE_RANGE;
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.dwCurrentState =
                            (IntPtr) ((int) value & EVENTSTATE_RANGE);
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.dwCurrentState =
                            (int) value & EVENTSTATE_RANGE;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Current state of reader.
        /// </summary>
        public IntPtr CurrentStateValue {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return (IntPtr) _winscardRstate.dwCurrentState;
                    case PlatformType.Linux:
                        return _linuxRstate.dwCurrentState;
                    case PlatformType.MacOSX:
                        return (IntPtr) _macosxRstate.dwCurrentState;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.dwCurrentState = unchecked((int) value.ToInt64());
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.dwCurrentState = value;
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.dwCurrentState = unchecked((int) value.ToInt64());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Reader state after a state change.
        /// </summary>
        public SCRState EventState {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return SCardHelper.ToSCRState(_winscardRstate.dwEventState & EVENTSTATE_RANGE);
                    case PlatformType.Linux:
                        return SCardHelper.ToSCRState(_linuxRstate.dwEventState.ToInt64() & EVENTSTATE_RANGE);
                    case PlatformType.MacOSX:
                        return SCardHelper.ToSCRState(_macosxRstate.dwEventState & EVENTSTATE_RANGE);

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                long lng = CardChangeEventCnt; // save CardChangeEventCounter

                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.dwEventState = ((int) value & EVENTSTATE_RANGE) | (int) lng;
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.dwEventState = (IntPtr)
                            (((int) value & EVENTSTATE_RANGE) | (int) lng);
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.dwEventState = ((int) value & EVENTSTATE_RANGE) | (int) lng;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Reader state after a state change.
        /// </summary>
        public IntPtr EventStateValue {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return (IntPtr) _winscardRstate.dwEventState;
                    case PlatformType.Linux:
                        return _linuxRstate.dwEventState;
                    case PlatformType.MacOSX:
                        return (IntPtr) _macosxRstate.dwEventState;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.dwEventState = unchecked((int) value.ToInt64());
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.dwEventState = value;
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.dwEventState = unchecked((int) value.ToInt64());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Number of change events.
        /// </summary>
        public int CardChangeEventCnt {
            get {
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        return (int) ((_winscardRstate.dwEventState & CHCOUNT_RANGE) >> 16);
                    case PlatformType.Linux:
                        return unchecked((int) ((_linuxRstate.dwEventState.ToInt64() & CHCOUNT_RANGE) >> 16));
                    case PlatformType.MacOSX:
                        return (int) ((_macosxRstate.dwEventState & CHCOUNT_RANGE) >> 16);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set {
                if ((value < 0) || (value > 0xFFFF))
                    throw new ArgumentOutOfRangeException(nameof(value));
                var es = (long) EventState; // save EventState

                //The upper 2 bytes of the EventStateValue hold the CardChangeEventCounter, the lower 2 bytes the EventState
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.dwEventState = unchecked((int)
                            (((value << 16) & CHCOUNT_RANGE) | es));
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.dwEventState = unchecked((IntPtr)
                            (((value << 16) & CHCOUNT_RANGE) | es));
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.dwEventState = unchecked((int)
                            (((value << 16) & CHCOUNT_RANGE) | es));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// The reader's name.
        /// </summary>
        public string ReaderName {
            get {
                if (_pReaderName == IntPtr.Zero) {
                    return null;
                }

                var tmp = new byte[_pReaderNameSize];
                Marshal.Copy(_pReaderName, tmp, 0, _pReaderNameSize);
                return SCardHelper.ConvertToString(tmp, tmp.Length, Platform.Lib.TextEncoding);
            }
            set {
                // Free reserved memory
                if (_pReaderName != IntPtr.Zero) {
                    Marshal.FreeCoTaskMem(_pReaderName);
                    _pReaderName = IntPtr.Zero;
                    _pReaderNameSize = 0;
                }

                if (value != null) {
                    var tmp = SCardHelper.ConvertToByteArray(value, Platform.Lib.TextEncoding, 0);
                    _pReaderName = Marshal.AllocCoTaskMem(tmp.Length + Platform.Lib.CharSize);
                    _pReaderNameSize = tmp.Length;
                    Marshal.Copy(tmp, 0, _pReaderName, tmp.Length);
                    for (var i = 0; i < (Platform.Lib.CharSize); i++) {
                        Marshal.WriteByte(_pReaderName, tmp.Length + i, 0); // String ends with \0 (or 0x00 0x00)
                    }
                }

                switch (Platform.Type) {
                    case PlatformType.Windows:
                        _winscardRstate.pszReader = _pReaderName;
                        break;
                    case PlatformType.Linux:
                        _linuxRstate.pszReader = _pReaderName;
                        break;
                    case PlatformType.MacOSX:
                        _macosxRstate.pszReader = _pReaderName;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        /// Answer To Reset (ATR)
        /// </summary>
        public byte[] Atr {
            get {
                byte[] tmp;

                switch (Platform.Type) {
                    case PlatformType.Windows:
                        if (_winscardRstate.cbAtr <= WinSCardAPI.MAX_ATR_SIZE) {
                            tmp = new byte[_winscardRstate.cbAtr];
                        } else {
                            // error occurred during SCardGetStatusChange()
                            tmp = new byte[WinSCardAPI.MAX_ATR_SIZE];
                            _winscardRstate.cbAtr = WinSCardAPI.MAX_ATR_SIZE;
                        }

                        Array.Copy(_winscardRstate.rgbAtr, tmp, _winscardRstate.cbAtr);
                        break;
                    case PlatformType.Linux:
                        var cbAtr = unchecked((int) _linuxRstate.cbAtr.ToInt64());
                        if (cbAtr <= PCSCliteLinux.MAX_ATR_SIZE) {
                            tmp = new byte[(int) _linuxRstate.cbAtr];
                        } else {
                            // error occurred during SCardGetStatusChange()
                            tmp = new byte[PCSCliteLinux.MAX_ATR_SIZE];
                            _linuxRstate.cbAtr = (IntPtr) PCSCliteLinux.MAX_ATR_SIZE;
                        }

                        Array.Copy(_linuxRstate.rgbAtr, tmp, cbAtr);
                        break;
                    case PlatformType.MacOSX:
                        if (_macosxRstate.cbAtr <= PCSCliteMacOsX.MAX_ATR_SIZE) {
                            tmp = new byte[_macosxRstate.cbAtr];
                        } else {
                            // error occurred during SCardGetStatusChange()
                            tmp = new byte[PCSCliteMacOsX.MAX_ATR_SIZE];
                            _macosxRstate.cbAtr = PCSCliteMacOsX.MAX_ATR_SIZE;
                        }

                        Array.Copy(_macosxRstate.rgbAtr, tmp, _macosxRstate.cbAtr);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return tmp;
            }
            set {
                var tmp = value;
                // the size of rstate.rgbAtr MUST(!) be MAX_ATR_SIZE 
                switch (Platform.Type) {
                    case PlatformType.Windows:
                        if (tmp.Length != WinSCardAPI.MAX_ATR_SIZE) {
                            Array.Resize(ref tmp, WinSCardAPI.MAX_ATR_SIZE);
                        }

                        _winscardRstate.rgbAtr = tmp;
                        _winscardRstate.cbAtr = value.Length;
                        break;
                    case PlatformType.Linux:
                        if (tmp.Length != PCSCliteLinux.MAX_ATR_SIZE) {
                            Array.Resize(ref tmp, PCSCliteLinux.MAX_ATR_SIZE);
                        }

                        _linuxRstate.rgbAtr = tmp;
                        _linuxRstate.cbAtr = (IntPtr) value.Length;
                        break;
                    case PlatformType.MacOSX:
                        if (tmp.Length != PCSCliteMacOsX.MAX_ATR_SIZE) {
                            Array.Resize(ref tmp, PCSCliteMacOsX.MAX_ATR_SIZE);
                        }

                        _macosxRstate.rgbAtr = tmp;
                        _macosxRstate.cbAtr = value.Length;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        internal WINDOWS_SCARD_READERSTATE WindowsReaderState {
            get => _winscardRstate;
            set => _winscardRstate = value;
        }

        internal LINUX_SCARD_READERSTATE LinuxReaderState {
            get => _linuxRstate;
            set => _linuxRstate = value;
        }
        
        internal MACOSX_SCARD_READERSTATE MacOsXReaderState {
            get => _macosxRstate;
            set => _macosxRstate = value;
        }
    }
}
