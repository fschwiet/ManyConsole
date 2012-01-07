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
            RemainingArgumentsHelpText = "";
            Options = new OptionSet();
            ParametersRequiredAfterOptions = 0;
            TraceCommandAfterParse = true;
        }

        public string Command { get; protected set; }
        public string OneLineDescription { get; protected set; }
        public string RemainingArgumentsHelpText { get; protected set; }
        
        public OptionSet Options { get; protected set; }

        public int? ParametersRequiredAfterOptions { get; protected set; }
        public bool TraceCommandAfterParse { get; protected set; }

        public abstract int Run(string[] remainingArguments);
    }
}