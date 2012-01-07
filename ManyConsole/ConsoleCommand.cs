using System;
using System.IO;
using System.Xml.Linq;
using ManyConsole.Internal;
using NDesk.Options;

namespace ManyConsole
{
    public abstract class ConsoleCommand : ConsoleUtil
    {
        public ConsoleCommand()
        {
            OneLineDescription = "";
            Options = new OptionSet();
            TraceCommandAfterParse = true;
            ArgumentsRequiredAfterOptions = 0;
            SampleTextForArgumentsRequiredAfterOptions = "";
        }

        public string Command { get; protected set; }
        public string OneLineDescription { get; protected set; }
        public string SampleTextForArgumentsRequiredAfterOptions { get; protected set; }
        
        public OptionSet Options { get; protected set; }

        public int? ArgumentsRequiredAfterOptions { get; protected set; }
        public bool TraceCommandAfterParse { get; protected set; }

        public abstract int Run(string[] remainingArguments);
    }
}