using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ManyConsole;

namespace SampleConsole
{
    public class DumpEmlFilesCommand : ConsoleCommand
    {
        public DumpEmlFilesCommand()
        {
            this.IsCommand("dump-eml", "Prints the contents of eml file(s).");

            this.HasOption("r|recursive", "Print files recursively", v => Recursive = v != null);
            this.HasOption("h|header=", "Mail header to include", v => HeadersToPrint.Add(v));

            HasAdditionalArguments(1, "<fileOrDirectory>");
        }

        public string Path;
        public bool Recursive;
        public List<string> HeadersToPrint = new List<string>();

        public override int Run(string[] remainingArguments)
        {
            Path = remainingArguments[0];

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
            var mail = Sasa.Net.Mail.Message.Parse(File.ReadAllText(filepath));

            Console.WriteLine("SUBJECT:\t{0}", mail.Subject);
            Console.WriteLine("FROM:\t{0}", mail.From);

            foreach(var to in mail.To)
            {
                Console.WriteLine("TO:\t{0}", to);
            }
            
            var headersPresent = mail.Headers.Keys.OfType<string>().Select(s => s.ToLower());

            foreach (var header in HeadersToPrint)
            {
                if (headersPresent.Contains(header.ToLower()))
                {
                    Console.WriteLine("{0}:\t{1}", header.ToUpper(), mail.Headers[header]);
                }
            }

            Console.WriteLine("BODY:");
            Console.WriteLine(mail.Body);
        }
    }
}
