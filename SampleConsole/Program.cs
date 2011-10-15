using System;
using System.Collections.Generic;
using ManyConsole;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // locate any commands in the assembly (or use an IoC container, or whatever source)
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));

            // run the command for the console input
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);

            //
            // optionally, run commands for each line of console input as well
            // Note that the commands are reloaded to prevent different runs from interfering
            // with each other.
            //
            string continuePrompt = "Enter a command or 'x' to exit or '?' for help";
            
            Console.WriteLine(continuePrompt);

            string input = Console.ReadLine();

            while (!input.Trim().Equals("x"))
            {
                if (input.Trim().Equals("?"))
                {
                    Console.Clear();
                    ConsoleCommandDispatcher.DispatchCommand(GetCommands(), new string[] { }, Console.Out);
                }
                else
                {
                    args = input.ToCommandLineArgs();
                    ConsoleCommandDispatcher.DispatchCommand(GetCommands(), args, Console.Out);
                }
                Console.WriteLine(continuePrompt);
                input = Console.ReadLine();
            }
        }

        static IEnumerable<ConsoleCommand> GetCommands()
        {
            return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
        }
    }
}
