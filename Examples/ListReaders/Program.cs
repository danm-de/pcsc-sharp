using System;
using System.Collections.Generic;
using System.Text;

using PCSC;

namespace ListReaders
{
    class Program
    {
        static void Main(string[] args)
        {
            SCardContext context = new SCardContext();
            context.Establish(SCardScope.System);

            Console.Out.WriteLine("Context is valid? -> " + context.IsValid());

            // list all (smart card) readers
            Console.Out.WriteLine("Currently connected readers: ");
            string[] readers = context.GetReaders();
            foreach (string reader in readers)
                Console.WriteLine("\t" + reader);

            // list all configured reader groups
            Console.Out.WriteLine("\nCurrently configured readers groups: ");
            string[] groups = context.GetReaderGroups();
            foreach (string group in groups)
                Console.WriteLine("\t" + group);

            // list readers for each group
            foreach (string group in groups)
            {
                Console.WriteLine("\nGroup " + group + " contains ");
                foreach (string reader in context.GetReaders(new string[] { group }))
                    Console.WriteLine("\t" + reader);
            }

            context.Release();
            Console.Out.WriteLine("Context is valid? -> " + context.IsValid());
            return;
        }
    }
}
