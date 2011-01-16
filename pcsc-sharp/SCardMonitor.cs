/*
Copyright (c) 2010 Daniel Mueller <daniel@danm.de>
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
using System.Threading;

namespace PCSC
{
    public delegate void StatusChangeEvent(object sender, StatusChangeEventArgs e);
    public delegate void CardInsertedEvent(object sender, CardStatusEventArgs e);
    public delegate void CardRemovedEvent(object sender, CardStatusEventArgs e);
    public delegate void CardInitializedEvent(object sender, CardStatusEventArgs e);
    public delegate void MonitorExceptionEvent(object sender, PCSCException ex);

    public class StatusChangeEventArgs : EventArgs
    {
        public string ReaderName;
        public SCRState LastState;
        public SCRState NewState;
        public byte[] ATR;

        public StatusChangeEventArgs() { }
        public StatusChangeEventArgs(string ReaderName, SCRState LastState, SCRState NewState, byte[] Atr)
        {
            this.ReaderName = ReaderName;
            this.LastState = LastState;
            this.NewState = NewState;
            this.ATR = Atr;
        }
    }

    public class CardStatusEventArgs : EventArgs
    {
        public string ReaderName;
        public SCRState State;
        public byte[] Atr;

        public CardStatusEventArgs() { }
        public CardStatusEventArgs(string ReaderName, SCRState State, byte[] Atr)
        {
            this.ReaderName = ReaderName;
            this.State = State;
            this.Atr = Atr;
        }
    }

    public class SCardMonitor : IDisposable
    {
        public event StatusChangeEvent StatusChanged;
        public event CardInsertedEvent CardInserted;
        public event CardRemovedEvent CardRemoved;
        public event CardInitializedEvent Initialized;
        public event MonitorExceptionEvent MonitorException;

        internal SCardContext context;
        internal SCRState[] previousState;
        internal IntPtr[] previousStateValue;
        internal Thread monitorthread;

        internal string[] readernames;
        public string[] ReaderNames
        {
            get
            {
                if (readernames == null)
                    return null;

                string[] tmp = new string[readernames.Length];
                Array.Copy(readernames, tmp, readernames.Length);

                return tmp;
            }
        }

        internal bool monitoring;
        public bool Monitoring
        {
            get { return monitoring; }
        }

        public IntPtr GetCurrentStateValue(int index)
        {
            if (previousStateValue == null)
                throw new InvalidOperationException("Monitor object is not initialized.");

            lock (previousStateValue)
            {
                // actually "previousStateValue" contains the last known value.
                if (index < 0 || index > previousStateValue.Length)
                    throw new ArgumentOutOfRangeException("index");
                return previousStateValue[index];
            }
        }

        public SCRState GetCurrentState(int index)
        {
            if (previousState == null)
                throw new InvalidOperationException("Monitor object is not initialized.");

            lock (previousState)
            {
                // "previousState" contains the last known value.
                if (index < 0 || index > previousState.Length)
                    throw new ArgumentOutOfRangeException("index");
                return previousState[index];
            }
        }

        public string GetReaderName(int index)
        {
            if (readernames == null)
                throw new InvalidOperationException("Monitor object is not initialized.");

            lock (readernames)
            {
                if (index < 0 || index > readernames.Length)
                    throw new ArgumentOutOfRangeException("index");
                return readernames[index];
            }
        }

        public int ReaderCount
        {
            get
            {
                lock (readernames)
                {
                    if (readernames == null)
                        return 0;

                    return readernames.Length;
                }
            }
        }

        public SCardMonitor(SCardContext hContext)
        {
            if (hContext == null)
                throw new ArgumentNullException("hContext");

            this.context = hContext;
        }

        public SCardMonitor(SCardContext hContext, SCardScope scope)
            : this(hContext)
        {
            hContext.Establish(scope);
        }

        ~SCardMonitor()
        {
            Dispose();
        }

        public void Dispose()
        {
            Cancel();
        }

        public void Cancel()
        {
            if (monitoring)
            {
                context.Cancel();
                readernames = null;
                previousStateValue = null;
                previousState = null;

                monitoring = false;
            }
        }

        public void Start(string readername)
        {
            if (readername == null || readername.Equals(""))
                throw new ArgumentNullException("readername");

            Start(new string[] { readername });
        }

        public void Start(string[] readernames)
        {
            if (monitoring)
                Cancel();

            lock (this)
            {
                if (!monitoring)
                {
                    if (readernames == null || readernames.Length == 0)
                        throw new ArgumentNullException("readernames");
                    if (context == null || !context.IsValid())
                        throw new InvalidContextException(SCardError.InvalidHandle,
                            "No connection context object specified.");

                    this.readernames = readernames;
                    previousState = new SCRState[readernames.Length];
                    previousStateValue = new IntPtr[readernames.Length];

                    monitorthread = new Thread(new ThreadStart(StartMonitor));
                    monitorthread.IsBackground = true;

                    monitorthread.Start();
                }
            }
        }

        private void StartMonitor()
        {
            monitoring = true;

            SCardReaderState[] state = new SCardReaderState[readernames.Length];

            for (int i = 0; i < readernames.Length; i++)
            {
                state[i] = new SCardReaderState();
                state[i].ReaderName = readernames[i];
                state[i].CurrentState = SCRState.Unaware; /* force SCardGetStatusChange to return 
                                                             immediately with the current reader state */
            }

            SCardError rc = context.GetStatusChange(IntPtr.Zero, state);

            if (rc == SCardError.Success)
            {
                // initialize event
                if (Initialized != null)
                {
                    for (int i = 0; i < state.Length; i++)
                    {
                        Initialized(this,
                                    new CardStatusEventArgs(readernames[i],
                                        (state[i].EventState & (~(SCRState.Changed))),
                                        state[i].ATR));
                        previousState[i] = state[i].EventState & (~(SCRState.Changed)); // remove "Changed"
                        previousStateValue[i] = state[i].EventStateValue;
                    }
                }

                while (true)
                {
                    for (int i = 0; i < state.Length; i++)
                    {
                        state[i].CurrentStateValue = previousStateValue[i];
                    }

                    // block until status change occurs                    
                    rc = context.GetStatusChange(SCardReader.Infinite, state);

                    // Cancel?
                    if (rc != SCardError.Success)
                        break;

                    for (int i = 0; i < state.Length; i++)
                    {
                        SCRState newState = state[i].EventState;
                        newState &= (~(SCRState.Changed)); // remove "Changed"

                        byte[] Atr = state[i].ATR;

                        // Status change
                        if (StatusChanged != null &&
                            (previousState[i] != newState))
                            StatusChanged(this,
                                          new StatusChangeEventArgs(readernames[i],
                                                                    previousState[i],
                                                                    newState,
                                                                    Atr));

                        // Card inserted
                        if (((newState & SCRState.Present) == SCRState.Present) &&
                            ((previousState[i] & SCRState.Empty) == SCRState.Empty))
                            if (CardInserted != null)
                                CardInserted(this,
                                             new CardStatusEventArgs(readernames[i],
                                                                     newState,
                                                                     Atr));

                        // Card removed
                        if (((newState & SCRState.Empty) == SCRState.Empty) &&
                            ((previousState[i] & SCRState.Present) == SCRState.Present))
                            if (CardRemoved != null)
                                CardRemoved(this,
                                            new CardStatusEventArgs(readernames[i],
                                                                    newState,
                                                                    Atr));

                        previousState[i] = newState;
                        previousStateValue[i] = state[i].EventStateValue;
                    }
                }
            }

            monitoring = false;

            if (rc != SCardError.Cancelled)
                if (MonitorException != null)
                    MonitorException(this, new PCSCException(rc,
                        "An error occured during SCardGetStatusChange(..)."));
        }
    }
}
