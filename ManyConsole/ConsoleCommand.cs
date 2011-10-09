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
            TraceCommandAfterParse = true;
        }

        public string Command { get; protected set; }
        public string OneLineDescription { get; protected set; }
        public string RemainingArgumentsHelpText { get; protected set; }
        
        public OptionSet Options { get; protected set; }
        
        public bool TraceCommandAfterParse { get; protected set; }

        /// <summary>
        /// Load the remaining arguments then validate all.
        /// </summary>
        /// <param name="remainingArguments">Arguments passed to the console after the parameters handled by the Options.</param>
        public virtual void FinishLoadingArguments(string[] remainingArguments)
        {
            VerifyNumberOfArguments(remainingArguments, 0);
        }

        public abstract int Run();
    }
}