using System;
using System.Collections.Generic;
using PCSC;

namespace ReaderStatus
{
    public class Program
    {
        public static void Main() {
            var contextFactory = ContextFactory.Instance;
            using (var ctx = contextFactory.Establish(SCardScope.System)) {
                var readerNames = ctx.GetReaders();

                if (NoReaderFound(readerNames)) {
                    Console.WriteLine("No reader connected.");
                    Console.ReadKey();
                    return;
                }

                var readerStates = ctx.GetReaderStatus(readerNames);

                foreach (var state in readerStates) {
                    PrintReaderState(state);

                    // free system resources (required for each returned state)
                    state.Dispose();
                }
            }
            Console.ReadKey();
        }

        private static void PrintReaderState(SCardReaderState state) {
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            var atr = BitConverter.ToString(state.Atr ?? new byte[0]);
            Console.WriteLine($"Reader: {state.ReaderName}\n" +
                              $"CurrentState: {state.CurrentState}\n" +
                              $"EventState: {state.EventState}\n" +
                              $"CurrentStateValue: {state.CurrentStateValue}\n" +
                              $"EventStateValue: {state.EventStateValue}\n" +
                              $"UserData: {state.UserData}\n" +
                              $"CardChangeEventCnt: {state.CardChangeEventCnt}\n" +
                              $"ATR: {atr}");
        }

        private static bool NoReaderFound(ICollection<string> readerNames) {
            return readerNames == null || readerNames.Count < 1;
        }
    }
}