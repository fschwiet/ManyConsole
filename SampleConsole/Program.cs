using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyConsole;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
            ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
        }
    }
}
