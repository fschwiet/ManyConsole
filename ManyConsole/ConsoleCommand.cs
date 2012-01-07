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
            RemainingArgumentsCount = 0;
            RemainingArgumentsHelpText = "";
        }

        public string Command { get; private set; }
        public string OneLineDescription { get; private set; }
        public OptionSet Options { get; protected set; }
        public bool TraceCommandAfterParse { get; private set; }
        public int? RemainingArgumentsCount { get; private set; }
        public string RemainingArgumentsHelpText { get; private set; }

        protected void IsCommand(string command, string oneLineDescription = "")
        {
            Command = command;
            OneLineDescription = oneLineDescription;
        }

        protected void HasAdditionalArguments(int? count = 0, string helpText = "")
        {
            RemainingArgumentsCount = count;
            RemainingArgumentsHelpText = helpText;
        }

        protected void AllowsAnyAdditionalArguments(string helpText = "")
        {
            HasAdditionalArguments(null, helpText);
        }

        protected void SkipsCommandSummaryBeforeRunning()
        {
            TraceCommandAfterParse = false;
        }

        public abstract int Run(string[] remainingArguments);
    }
}