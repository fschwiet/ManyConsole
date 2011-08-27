using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ManyConsole;
using NDesk.Options;

namespace SampleConsole
{
    public class DumpEmlFiles : ConsoleCommand
    {
        public DumpEmlFiles()
        {
            Command = "dump-eml";
            OneLineDescription = "Prints the contents of eml file(s).";
            RemainingArgumentsHelpText = "<fileOrDirectory>";
            Options = new OptionSet()
            {
                {"r|recursive", "Print files recursively", v => Recursive = v != null},
                {"h|header=", "Mail header to include", v => HeadersToPrint.Add(v)}
            };
        }

        string Path;
        bool Recursive;
        private List<string> HeadersToPrint = new List<string>();

        public override void FinishLoadingArguments(string[] remainingArguments)
        {
            VerifyNumberOfArguments(remainingArguments, 1);

            Path = remainingArguments[0];
        }

        public override int Run()
        {
            if (File.Exists(Path))
            {
                PrintEmlFile(Path);
            }
            else if (Directory.Exists(Path))
            {
                PrintEmlDirectory(Path);
            }
            else
            {
                throw new Exception("Could not find file or directory at " + Path);
            }

            return 0;
        }

        private void PrintEmlDirectory(string directory)
        {
            var di = new DirectoryInfo(directory);

            foreach(var entry in di.GetFileSystemInfos())
            {
                if (entry is FileInfo)
                {
                    PrintEmlFile(entry.FullName);
                }
                else if (Recursive && entry is DirectoryInfo)
                {
                    PrintEmlDirectory(entry.FullName);
                }
            }
        }

        private void PrintEmlFile(string filepath)
        {
            var mail = Sasa.Net.Mail.Message.ParseMailMessage(File.ReadAllText(filepath));

            Console.WriteLine("SUBJECT:\t{0}", mail.Subject);
            Console.WriteLine("FROM:\t{0}", mail.From);
            foreach(var header in HeadersToPrint)
            {
                if (mail.Headers.Keys.OfType<string>().Contains(header))
                {
                   Console.WriteLine("Header '{0}': {1}", header, mail.Headers[header]);
                }
            }

            foreach(var to in mail.To)
            {
                Console.WriteLine("TO:\t{0}", to);
            }
            Console.WriteLine("BODY:");
            Console.WriteLine(mail.Body);
        }
    }
}
