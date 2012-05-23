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
        public IConsoleRedirectionDetection RedirectionDetector = new ConsoleRedirectionDetection();
        public static string FriendlyContinuePrompt = "\nEnter a command or 'x' to exit or '?' for help";
        private string continuePrompt;

        public ConsoleModeCommand(
            Func<IEnumerable<ConsoleCommand>> commandSource,
            TextWriter outputStream = null,
            TextReader inputStream = null,
            string friendlyContinueText = null)
        {
            _inputStream = inputStream ?? Console.In;
            _outputStream = outputStream ?? Console.Out;

            this.IsCommand("run-console", "Run in console mode, treating each line of console input as a command.");

            _commandSource = () =>
            {
                var commands = commandSource();
                return commands.Where(c => !(c is ConsoleModeCommand));  // don't cross the beams
            };

            continuePrompt = friendlyContinueText ?? FriendlyContinuePrompt;
        }

        public override int Run(string[] remainingArguments)
        {
            string[] args;

            bool isInputRedirected = RedirectionDetector.IsInputRedirected();

            if (!isInputRedirected)
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
                    if (result != 0)
                    {
                        haveError = true;

                        if (isInputRedirected)
                            return result;
                    }
                }
                
                if (!isInputRedirected)
                    _outputStream.WriteLine(continuePrompt);

                input = _inputStream.ReadLine();
            }

            return haveError ? -1 : 0;
        }
    }
}
