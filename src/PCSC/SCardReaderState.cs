using System;
using System.Runtime.InteropServices;
using PCSC.Interop;
using PCSC.Interop.Unix;
using PCSC.Interop.Windows;
using PCSC.Utils;
using WINDOWS_SCARD_READERSTATE = PCSC.Interop.Windows.SCARD_READERSTATE;
using UNIX_SCARD_READERSTATE = PCSC.Interop.Unix.SCARD_READERSTATE;

namespace PCSC
{
    /// <summary>A structures that contains the old and the new Smart Card reader status.</summary>
    /// <remarks>
    ///     <para>Is used as parameter in <see cref="M:PCSC.ISCardContext.GetStatusChange(System.IntPtr,PCSC.SCardReaderState[])" />.</para>
    ///     <para>The new event state will be contained in <see cref="P:PCSC.SCardReaderState.EventState" />. A status change might be a card insertion or removal event, a change in ATR, etc. To wait for a reader event (reader added or removed) you may use the special reader name "\\?PnP?\Notification". If a reader event occurs the state of this reader will change and the bit <see cref="F:PCSC.SCRState.Changed" /> will be set.</para>
    /// </remarks>
    public class SCardReaderState : IDisposable
    {
        // we're getting values greater than 0xFFFF back from SCardGetStatusChange 
        private const int EVENTSTATE_RANGE = 0xFFFF;
        private const long CHCOUNT_RANGE = 0xFFFF0000;

        private WINDOWS_SCARD_READERSTATE _winscardRstate;
        private UNIX_SCARD_READERSTATE _pcscliteRstate;

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
            // We must free unmanaged resources!
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
            if (Platform.IsWindows) {
                _winscardRstate = new WINDOWS_SCARD_READERSTATE {
                    // initialize embedded array
                    rgbAtr = new byte[WinSCardAPI.MAX_ATR_SIZE],
                    cbAtr = WinSCardAPI.MAX_ATR_SIZE
                };
            } else {
                _pcscliteRstate = new Interop.Unix.SCARD_READERSTATE {
                    // initialize embedded array
                    rgbAtr = new byte[PCSCliteAPI.MAX_ATR_SIZE],
                    cbAtr = (IntPtr) PCSCliteAPI.MAX_ATR_SIZE
                };
            }
        }

        /// <summary>
        /// User defined data.
        /// </summary>
        public long UserData {
            get { return (long) UserDataPointer; }
            set { UserDataPointer = unchecked((IntPtr) value); }
        }

        /// <summary>
        /// User defined data.
        /// </summary>
        public IntPtr UserDataPointer {
            get {
                return Platform.IsWindows
                    ? _winscardRstate.pvUserData
                    : _pcscliteRstate.pvUserData;
            }
            set {
                if (Platform.IsWindows) {
                    _winscardRstate.pvUserData = value;
                } else {
                    _pcscliteRstate.pvUserData = value;
                }
            }
        }


        /// <summary>
        /// Current state of reader.
        /// </summary>
        public SCRState CurrentState {
            get {
                return Platform.IsWindows
                    ? SCardHelper.ToSCRState((_winscardRstate.dwCurrentState & EVENTSTATE_RANGE))
                    : SCardHelper.ToSCRState(((int) _pcscliteRstate.dwCurrentState & EVENTSTATE_RANGE));
            }
            set {
                if (Platform.IsWindows) {
                    _winscardRstate.dwCurrentState =
                        (int) value & EVENTSTATE_RANGE;
                    return;
                }

                _pcscliteRstate.dwCurrentState =
                    (IntPtr) ((int) value & EVENTSTATE_RANGE);
            }
        }

        /// <summary>
        /// Current state of reader.
        /// </summary>
        public IntPtr CurrentStateValue {
            get {
                return Platform.IsWindows
                    ? (IntPtr) _winscardRstate.dwCurrentState
                    : _pcscliteRstate.dwCurrentState;
            }
            set {
                if (Platform.IsWindows) {
                    // On a 64-bit platform .ToInt32() will throw an OverflowException 
                    _winscardRstate.dwCurrentState = unchecked((int) value.ToInt64());
                } else {
                    _pcscliteRstate.dwCurrentState = value;
                }
            }
        }
        /// <summary>
        /// Reader state after a state change.
        /// </summary>
        public SCRState EventState {
            get {
                return Platform.IsWindows
                    ? SCardHelper.ToSCRState(
                        (_winscardRstate.dwEventState & EVENTSTATE_RANGE))
                    : SCardHelper.ToSCRState(
                        (((int) _pcscliteRstate.dwEventState) & EVENTSTATE_RANGE));
            }
            set {
                long lng = CardChangeEventCnt; // save CardChangeEventCounter
                if (Platform.IsWindows) {
                    _winscardRstate.dwEventState = ((int) value & EVENTSTATE_RANGE) | (int) lng;
                } else {
                    _pcscliteRstate.dwEventState = (IntPtr)
                        (((int) value & EVENTSTATE_RANGE) | (int) lng);
                }
            }
        }

        /// <summary>
        /// Reader state after a state change.
        /// </summary>
        public IntPtr EventStateValue {
            get {
                return Platform.IsWindows
                    ? (IntPtr) _winscardRstate.dwEventState
                    : _pcscliteRstate.dwEventState;
            }
            set {
                if (Platform.IsWindows) {
                    // On a 64-bit platforms .ToInt32() will throw an OverflowException 
                    _winscardRstate.dwEventState = unchecked((int) value.ToInt64());
                } else {
                    _pcscliteRstate.dwEventState = value;
                }
            }
        }

        /// <summary>
        /// Number of change events.
        /// </summary>
        public int CardChangeEventCnt {
            get {
                return Platform.IsWindows
                    ? (int) ((_winscardRstate.dwEventState & CHCOUNT_RANGE) >> 16)
                    : (int) ((((long) _pcscliteRstate.dwEventState) & CHCOUNT_RANGE) >> 16);
            }
            set {
                if ((value < 0) || (value > 0xFFFF))
                    throw new ArgumentOutOfRangeException (nameof(value));
                var es = (long) EventState; // save EventState
                
                //The upper 2 bytes of the EventStateValue hold the CardChangeEventCounter, the lower 2 bytes the EventState
                if (Platform.IsWindows) {
                    _winscardRstate.dwEventState = unchecked((int)
                        (((value << 16) & CHCOUNT_RANGE) | es));
                } else {
                    _pcscliteRstate.dwEventState = unchecked((IntPtr)
                        (((value << 16) & CHCOUNT_RANGE) | es));
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

                if (Platform.IsWindows) {
                    _winscardRstate.pszReader = _pReaderName;
                } else {
                    _pcscliteRstate.pszReader = _pReaderName;
                }
            }
        }

        /// <summary>
        /// Answer To Reset (ATR)
        /// </summary>
        public byte[] Atr {
            get {
                byte[] tmp;

                if (Platform.IsWindows) {
                    if (_winscardRstate.cbAtr <= WinSCardAPI.MAX_ATR_SIZE) {
                        tmp = new byte[_winscardRstate.cbAtr];
                    } else {
                        // error occurred during SCardGetStatusChange()
                        tmp = new byte[WinSCardAPI.MAX_ATR_SIZE];
                        _winscardRstate.cbAtr = WinSCardAPI.MAX_ATR_SIZE;
                    }
                    Array.Copy(_winscardRstate.rgbAtr, tmp, _winscardRstate.cbAtr);
                } else {
                    if ((int) _pcscliteRstate.cbAtr <= PCSCliteAPI.MAX_ATR_SIZE) {
                        tmp = new byte[(int) _pcscliteRstate.cbAtr];
                    } else {
                        // error occurred during SCardGetStatusChange()
                        tmp = new byte[PCSCliteAPI.MAX_ATR_SIZE];
                        _pcscliteRstate.cbAtr = (IntPtr) PCSCliteAPI.MAX_ATR_SIZE;
                    }
                    Array.Copy(_pcscliteRstate.rgbAtr, tmp, (int) _pcscliteRstate.cbAtr);
                }

                return tmp;
            }
            set {
                var tmp = value;
                // the size of rstate.rgbAtr MUST(!) be MAX_ATR_SIZE 
                if (Platform.IsWindows) {
                    if (tmp.Length != WinSCardAPI.MAX_ATR_SIZE) {
                        Array.Resize(ref tmp, WinSCardAPI.MAX_ATR_SIZE);
                    }
                    _winscardRstate.rgbAtr = tmp;
                    _winscardRstate.cbAtr = value.Length;
                } else {
                    if (tmp.Length != PCSCliteAPI.MAX_ATR_SIZE) {
                        Array.Resize(ref tmp, PCSCliteAPI.MAX_ATR_SIZE);
                    }
                    _pcscliteRstate.rgbAtr = tmp;
                    _pcscliteRstate.cbAtr = (IntPtr) value.Length;
                }
            }
        }

        internal WINDOWS_SCARD_READERSTATE WindowsReaderState {
            get { return _winscardRstate; }
            set { _winscardRstate = value; }
        }

        internal UNIX_SCARD_READERSTATE UnixReaderState {
            get { return _pcscliteRstate; }
            set { _pcscliteRstate = value; }
        }
    }
}
