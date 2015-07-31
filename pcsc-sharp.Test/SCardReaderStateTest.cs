using NUnit.Framework;
using System;
using PCSC;

namespace PcSc
{
	[TestFixture ()]
	public class SCardReaderStateTest
	{
		[Test ()]
		public void CardChangeEventCntSetEventStateValueTest ()
		{
			SCardReaderState state = new SCardReaderState ();
			state.EventStateValue = new IntPtr(0);
			Assert.AreEqual (0, state.CardChangeEventCnt);
			//set the counter (upper 2 bytes of the event state) to 1
			state.EventStateValue = new IntPtr (0x00010000);
			Assert.AreEqual (1, state.CardChangeEventCnt);
			//Set the maximum value
			state.EventStateValue = new IntPtr (0xFFFF0000);
			Assert.AreEqual (65535, state.CardChangeEventCnt);
		}

		[Test ()]
		public void CardChangeEventCntSetTest ()
		{
			SCardReaderState state = new SCardReaderState ();
			state.EventState = SCRState.Empty;
			state.CardChangeEventCnt = 0;
			Assert.AreEqual (0, state.CardChangeEventCnt);
			Assert.AreEqual ((uint)SCRState.Empty, (uint)state.EventStateValue.ToInt32());
			Assert.AreEqual (SCRState.Empty, state.EventState, "EventState should not change by setting the Counter");
			//set the counter to 1
			state.CardChangeEventCnt = 1;
			Assert.AreEqual (1, state.CardChangeEventCnt);
			Assert.AreEqual (0x00010010, (uint)state.EventStateValue.ToInt32());
			Assert.AreEqual (SCRState.Empty, state.EventState, "EventState should not change by setting the Counter");
			//Set the maximum value
			state.CardChangeEventCnt = 65535;
			Assert.AreEqual (65535, state.CardChangeEventCnt);
			Assert.AreEqual (0xFFFF0010, (uint)state.EventStateValue.ToInt32());
			//try to set a value that is too high
			Assert.Throws(Is.InstanceOf<ArgumentException>(), () => (state.CardChangeEventCnt = 65536));
			//try to set a negative value
			Assert.Throws(Is.InstanceOf<ArgumentException>(), () => (state.CardChangeEventCnt = -1));
		}
	}
}

