using System;
using ManyConsole;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            // locate any commands in the assembly
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));

            // immediately deal with the initial execution (i.e. command args)
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);

            // the remainder of this code shows how to continue to deal with input from the command prompt 
            // in the same way as we do for command-line args
            string continuePrompt = "Enter a command or 'x' to exit or '?' for help";
            
            Console.WriteLine(continuePrompt);

            string input = Console.ReadLine();

            while (!input.Trim().Equals("x"))
            {
                if (input.Trim().Equals("?"))
                {
                    Console.Clear();
                    ConsoleCommandDispatcher.DispatchCommand(commands, new string[] { }, Console.Out);
                }
                else
                {
                    args = input.ToCommandLineArgs();
                    ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
                }
                Console.WriteLine(continuePrompt);
                input = Console.ReadLine();
            }
        }
    }
}
