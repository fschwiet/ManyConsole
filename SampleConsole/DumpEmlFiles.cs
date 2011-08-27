using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ManyConsole;

namespace SampleConsole
{
    public class DumpEmlFiles : ConsoleCommand
    {
        public DumpEmlFiles()
        {
            Command = "dump-eml";
            OneLineDescription = "Prints the contents of eml file(s).";
            RemainingArgumentsHelpText = "<filename>";
        }

        protected string Filename;

        public override void FinishLoadingArguments(string[] remainingArguments)
        {
            VerifyNumberOfArguments(remainingArguments, 1);

            Filename = remainingArguments[0];
        }

        public override int Run()
        {
            var mail = Sasa.Net.Mail.Message.ParseMailMessage(File.ReadAllText(Filename));

            Console.WriteLine("SUBJECT:\t{0}", mail.Subject);
            Console.WriteLine("FROM:\t{0}", mail.From);
            foreach(var to in mail.To)
            {
                Console.WriteLine("TO:\t{0}", to);
            }
            Console.WriteLine("BODY:");
            Console.WriteLine(mail.Body);

            return 0;
        }
    }
}
