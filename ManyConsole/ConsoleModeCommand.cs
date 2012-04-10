using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManyConsole.Internal;

namespace ManyConsole
{
    public class ConsoleModeCommand : ConsoleCommand
    {
        private readonly TextReader _inputStream;
        private readonly TextWriter _outputStream;
        readonly Func<IEnumerable<ConsoleCommand>> _commandSource;

        public ConsoleModeCommand(
            Func<IEnumerable<ConsoleCommand>> commandSource,
            TextWriter outputStream = null,
            TextReader inputStream = null)
        {
            _inputStream = inputStream ?? Console.In;
            _outputStream = outputStream ?? Console.Out;

            this.IsCommand("run-console", "Run lines within a console");

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
            
            _outputStream.WriteLine(continuePrompt);

            bool haveError = false;
            string input = _inputStream.ReadLine();

            while (!input.Trim().Equals("x"))
            {
                if (input.Trim().Equals("?"))
                {
                    ConsoleCommandDispatcher.DispatchCommand(_commandSource(), new string[] { }, _outputStream);
                }
                else
                {
                    args = input.ToCommandLineArgs();
                    var result = ConsoleCommandDispatcher.DispatchCommand(_commandSource(), args, _outputStream);
                    haveError = haveError || result != 0;
                }
                _outputStream.WriteLine(continuePrompt);
                input = _inputStream.ReadLine();
            }

            return haveError ? -1 : 0;
        }
    }
}
