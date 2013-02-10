using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyConsole;

namespace SampleConsole
{
    public class StatefulConsoleCommand : ConsoleModeCommand
    {
        public int Count = 0;

        public StatefulConsoleCommand()
        {
            this.IsCommand("stateful", "Runs commands from the console statefully");
        }

        public override void WritePromptForCommands()
        {
            Console.WriteLine("You have seen this console {0} times.", Count++);

            base.WritePromptForCommands();
        }

        public override IEnumerable<ConsoleCommand> GetNextCommands()
        {
            return new ConsoleCommand[] {new GetTime(), new MattsCommand(), new DumpEmlFiles(), new DumpEmlFiles()};
        }
    }
}
