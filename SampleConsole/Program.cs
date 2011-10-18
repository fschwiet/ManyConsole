using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));

            // include ConsoleModeCommand if you want to allow commands to be run from the console
            // input
            ConsoleModeCommand consoleRunner = new ConsoleModeCommand(GetCommands);
            commands = commands.Concat(new[] { consoleRunner });

            // run the command for the console input
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }

        static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}
