using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NDesk.Options;

namespace ManyConsole.Tests
{
    public class InlinedCommand : ConsoleCommand
    {
        public InlinedCommand(string commandText = "", string oneLineDescription = "", string remaingArgumentsHelpText = "", OptionSet options = null, Func<int> runAction = null)
        {
            Command = commandText;
            OneLineDescription = oneLineDescription;
            RemainingArgumentsHelpText = remaingArgumentsHelpText;
            Options = options ?? new OptionSet();
            TraceCommandAfterParse = true;
            RunAction = runAction ?? delegate { return 0; };
        }

        private Func<int> RunAction;

        public override int Run(string[] remainingArguments)
        {
            return RunAction();
        }
    }
}
