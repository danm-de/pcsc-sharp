using System;
using System.Collections.Generic;
using PCSC;

namespace ListReaders {
    class Program {
        public static void Main(string[] args) {
            var contextFactory = ContextFactory.Instance;
            using (var context = contextFactory.Establish(SCardScope.System)) {
                var readerNames = context.GetReaders();
                PrintAllReaders(readerNames);

                var groupNames = context.GetReaderGroups();
                PrintAllReaderGroups(groupNames);
                PrintReadersPerGroup(groupNames, context);

                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
            }
        }

        private static void PrintReadersPerGroup(string[] groupNames, ISCardContext context) {
            foreach (var groupName in groupNames) {
                Console.WriteLine("\nGroup " + groupName + " contains ");
                foreach (var readerName in context.GetReaders(new[] {groupName})) {
                    Console.WriteLine("\t" + readerName);
                }
            }
        }

        private static void PrintAllReaderGroups(string[] groupNames) {
            Console.WriteLine("\nCurrently configured readers groups: ");
            foreach (var groupName in groupNames) {
                Console.WriteLine("\t" + groupName);
            }
        }

        private static void PrintAllReaders(IEnumerable<string> readerNames) {
            Console.WriteLine("Currently connected readers: ");
            foreach (var name in readerNames) {
                Console.WriteLine("\t" + name);
            }
        }
    }
}
