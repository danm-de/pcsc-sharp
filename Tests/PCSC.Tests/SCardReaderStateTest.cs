using System;
using FluentAssertions;
using NUnit.Framework;

namespace PCSC.Tests
{
    [TestFixture]
    public class SCardReaderStateTest
    {
        [Test]
        public void CardChangeEventCntSetEventStateValueTest() {
            var state = new SCardReaderState {
                EventStateValue = IntPtr.Zero
            };
            state.CardChangeEventCnt
                .Should()
                .Be(0);

            //set the counter (upper 2 bytes of the event state) to 1
            state.EventStateValue = new IntPtr(0x00010000);
            state.CardChangeEventCnt
                .Should()
                .Be(1);

            //Set the maximum value
            //The maximum value depends on the architecture.
            //If the process is running 32-bit, IntPtr(long) throws an exception.
            if (IntPtr.Size == sizeof(long)) {
                state.EventStateValue = new IntPtr(0xFFFF0000);
                state.CardChangeEventCnt
                    .Should()
                    .Be(0xFFFF);
            } else {
                state.EventStateValue = new IntPtr(0x7FFF0000);
                state.CardChangeEventCnt
                    .Should()
                    .Be(0x7FFF);
            }
        }

        [Test]
        public void CardChangeEventCntSetTest() {
            var state = new SCardReaderState {
                EventState = SCRState.Empty,
                CardChangeEventCnt = 0
            };
            state.CardChangeEventCnt
                .Should()
                .Be(0);

            ((uint) state.EventStateValue.ToInt32())
                .Should()
                .Be((uint) SCRState.Empty);

            state.EventState
                .Should()
                .Be(SCRState.Empty, "EventState should not change by setting the Counter");

            //set the counter to 1
            state.CardChangeEventCnt = 1;
            state.CardChangeEventCnt
                .Should()
                .Be(1);
            ((uint) state.EventStateValue.ToInt32())
                .Should()
                .Be(0x00010010);

            state.EventState
                .Should()
                .Be(SCRState.Empty, "EventState should not change by setting the Counter");

            //Set the maximum value
            state.CardChangeEventCnt = 65535;
            state.CardChangeEventCnt
                .Should()
                .Be(65535);
            (unchecked((uint) state.EventStateValue.ToInt64()))
                .Should().Be(0xFFFF0010);

            //try to set a value that is too high
            ((Action) (() => state.CardChangeEventCnt = 65536))
                .Should().Throw<ArgumentException>();

            //try to set a negative value
            ((Action) (() => state.CardChangeEventCnt = -1))
                .Should().Throw<ArgumentException>();
        }
    }
}
