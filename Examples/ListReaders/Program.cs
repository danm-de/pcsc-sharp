using System;
using PCSC;
using PCSC.Context;

namespace ListReaders
{
    public class Program
    {
        public static void Main() {
            var contextFactory = ContextFactory.Instance;
            using (var context = contextFactory.Establish(SCardScope.System)) {

                Console.WriteLine("Context is valid? -> " + context.IsValid());

                // list all (smart card) readers
                Console.WriteLine("Currently connected readers: ");
                var readerNames = context.GetReaders();
                foreach (var readerName in readerNames) {
                    Console.WriteLine("\t" + readerName);
                }

                // list all configured reader groups
                Console.WriteLine("\nCurrently configured readers groups: ");
                var groupNames = context.GetReaderGroups();
                foreach (var groupName in groupNames) {
                    Console.WriteLine("\t" + groupName);
                }

                // list readers for each group
                foreach (var groupName in groupNames) {
                    Console.WriteLine("\nGroup " + groupName + " contains ");
                    foreach (var readerName in context.GetReaders(new[] {groupName})) {
                        Console.WriteLine("\t" + readerName);
                    }
                }

                context.Release();
                Console.WriteLine("Context is valid? -> " + context.IsValid());
                Console.ReadKey();
            }
        }
    }
}