using System;
using System.Collections.Generic;
using System.Linq;
using ManyConsole.Internal;

namespace ManyConsole
{
    public class ConsoleModeCommand : ConsoleCommand
    {
        readonly Func<IEnumerable<ConsoleCommand>> _commandSource;

        public ConsoleModeCommand(Func<IEnumerable<ConsoleCommand>> commandSource)
        {
            Command = "run-console";
            OneLineDescription = "Run lines within a console";

            _commandSource = () =>
            {
                var commands = commandSource();
                return commands.Where(c => !(c is ConsoleModeCommand));  // don't cross the beams
            };
        }

        public override int Run(string[] remainingArguments)
        {
            string[] args;
            string continuePrompt = "\nEnter a command or 'x' to exit or '?' for help";
            
            Console.WriteLine(continuePrompt);

            string input = Console.ReadLine();

            while (!input.Trim().Equals("x"))
            {
                if (input.Trim().Equals("?"))
                {
                    Console.Clear();
                    ConsoleCommandDispatcher.DispatchCommand(_commandSource(), new string[] { }, Console.Out);
                }
                else
                {
                    args = input.ToCommandLineArgs();
                    ConsoleCommandDispatcher.DispatchCommand(_commandSource(), args, Console.Out);
                }
                Console.WriteLine(continuePrompt);
                input = Console.ReadLine();
            }

            return 0;
        }
    }
}
