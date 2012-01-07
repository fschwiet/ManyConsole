using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ManyConsole.Internal;

namespace ManyConsole
{
    public class ConsoleCommandDispatcher
    {
        public static int DispatchCommand(IEnumerable<ConsoleCommand> commands, string[] arguments, TextWriter consoleOut)
        {
            ConsoleCommand selectedCommand = null;

            TextWriter console = consoleOut;

            try
            {
                if (arguments.Count() < 1)
                    throw new ConsoleHelpAsException("No arguments specified.");

                foreach(var possibleCommand in commands)
                {
                    if (string.IsNullOrEmpty(possibleCommand.Command))
                    {
                        throw new InvalidOperationException(String.Format(
                            "Command {0} did not define property Command, which must specify its command text.",
                            possibleCommand.GetType().Name));
                    }

                    if (arguments.First().ToLower() == possibleCommand.Command.ToLower())
                    {
                        selectedCommand = possibleCommand;

                        break;
                    }
                }

                if (selectedCommand == null)
                    throw new ConsoleHelpAsException("Command name not recognized.");

                var remainingArguments = selectedCommand.Options.Parse(arguments.Skip(1));

                CheckArguments(remainingArguments, selectedCommand.RemainingArgumentsCount);

                ConsoleHelp.ShowParsedCommand(selectedCommand, console);

                return selectedCommand.Run(remainingArguments.ToArray());
            }
            catch (Exception e)
            {
                console.WriteLine();
                ConsoleHelpAsException.WriterErrorMessage(e, console);
                console.WriteLine();

                if (selectedCommand != null)
                {
                    if (e is ConsoleHelpAsException)
                        ConsoleHelp.ShowCommandHelp(selectedCommand, console);
                }
                else
                {
                    ConsoleHelp.ShowSummaryOfCommands(commands, console);
                }

                return -1;
            }
        }

        private static void CheckArguments(List<string> remainingArguments, int? parametersRequiredAfterOptions)
        {
            if (parametersRequiredAfterOptions.HasValue)
                ConsoleUtil.VerifyNumberOfArguments(remainingArguments.ToArray(),
                    parametersRequiredAfterOptions.Value);
        }

        public static IEnumerable<ConsoleCommand> FindCommandsInSameAssemblyAs(Type typeInSameAssembly)
        {
            var assembly = typeInSameAssembly.Assembly;

            var commandTypes = assembly.GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ConsoleCommand)))
                .Where(t => !t.IsAbstract)
                .OrderBy(t => t.FullName);

            List<ConsoleCommand> result = new List<ConsoleCommand>();

            foreach(var commandType in commandTypes)
            {
                var constructor = commandType.GetConstructor(new Type[] { });

                if (constructor == null)
                    continue;

                result.Add((ConsoleCommand)constructor.Invoke(new object[] { }));
            }

            return result;
        }
    }
}
