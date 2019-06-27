using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using ManyConsole.Internal;

namespace ManyConsole
{
    public class ConsoleCommandDispatcher
    {
        public static int DispatchCommand(ConsoleCommand command, string[] arguments, TextWriter consoleOut)
        {
            return DispatchCommand(new [] {command}, arguments, consoleOut);
        }

        public static int DispatchCommand(IEnumerable<ConsoleCommand> commands, string[] arguments, TextWriter consoleOut, bool skipExeInExpectedUsage = false)
        {
            ConsoleCommand selectedCommand = null;

            TextWriter console = consoleOut;

            foreach (var command in commands)
            {
                ValidateConsoleCommand(command);
            }

            try
            {
                List<string> remainingArguments;

                if (commands.Count() == 1)
                {
                    selectedCommand = commands.First();

                    if (arguments.Count() > 0 && CommandMatchesArgument(selectedCommand, arguments.First()))
                    {
                        remainingArguments = selectedCommand.GetActualOptions().Parse(arguments.Skip(1));
                    }
                    else
                    {
                        remainingArguments = selectedCommand.GetActualOptions().Parse(arguments);
                    }
                }
                else
                {
                    if (arguments.Count() < 1)
                        throw new ConsoleHelpAsException("No arguments specified.");

                    if (arguments[0].Equals("help", StringComparison.InvariantCultureIgnoreCase))
                    {
                        selectedCommand = GetMatchingCommand(commands, arguments.Skip(1).FirstOrDefault());

                        if (selectedCommand == null)
                            ConsoleHelp.ShowSummaryOfCommands(commands, console);
                        else
                            ConsoleHelp.ShowCommandHelp(selectedCommand, console, skipExeInExpectedUsage);

                        return -1;
                    }

                    selectedCommand = GetMatchingCommand(commands, arguments.First());

                    if (selectedCommand == null)
                        throw new ConsoleHelpAsException("Command name not recognized.");

                    remainingArguments = selectedCommand.GetActualOptions().Parse(arguments.Skip(1));
                }

                selectedCommand.CheckRequiredArguments();

                CheckRemainingArguments(remainingArguments, selectedCommand.RemainingArgumentsCountMin, selectedCommand.RemainingArgumentsCountMax);

                var preResult = selectedCommand.OverrideAfterHandlingArgumentsBeforeRun(remainingArguments.ToArray());

                if (preResult.HasValue)
                    return preResult.Value;

                ConsoleHelp.ShowParsedCommand(selectedCommand, console);

                return selectedCommand.Run(remainingArguments.ToArray());
            }
            catch (ConsoleHelpAsException e)
            {
                return DealWithException(e, console, skipExeInExpectedUsage, selectedCommand, commands);
            }
            catch (Mono.Options.OptionException e)
            {
                return DealWithException(e, console, skipExeInExpectedUsage, selectedCommand, commands);
            }
        }

        private static int DealWithException(Exception e, TextWriter console, bool skipExeInExpectedUsage, ConsoleCommand selectedCommand, IEnumerable<ConsoleCommand> commands)
        {
            if (selectedCommand != null)
            {
                console.WriteLine();
                console.WriteLine(e.Message);
                ConsoleHelp.ShowCommandHelp(selectedCommand, console, skipExeInExpectedUsage);
            }
            else
            {
                ConsoleHelp.ShowSummaryOfCommands(commands, console);
            }

            return -1;
        }
  
        private static ConsoleCommand GetMatchingCommand(IEnumerable<ConsoleCommand> command, string name)
        {
            return command.FirstOrDefault(c => CommandMatchesArgument(c, name));
        }

        private static bool CommandMatchesArgument(ConsoleCommand command, string arg)
        {
            if (String.IsNullOrEmpty(arg))
            {
                return false;
            }
            if (arg.Equals(command.Command, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            } else if (command.Aliases != null && command.Aliases.Count > 0)
            {
                foreach (string alias in command.Aliases)
                {
                    if (arg.Equals(alias, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static void ValidateConsoleCommand(ConsoleCommand command)
        {
            if (string.IsNullOrEmpty(command.Command))
            {
                throw new InvalidOperationException(String.Format(
                    "Command {0} did not call IsCommand in its constructor to indicate its name and description.",
                    command.GetType().Name));
            }
        }

        private static void CheckRemainingArguments(List<string> remainingArguments, int? parametersRequiredAfterOptionsMin, int? parametersRequiredAfterOptionsMax)
        {
            ConsoleUtil.VerifyNumberOfArguments(remainingArguments.ToArray(),
                    parametersRequiredAfterOptionsMin, parametersRequiredAfterOptionsMax);
        }

        public static IEnumerable<ConsoleCommand> FindCommandsInSameAssemblyAs(Type typeInSameAssembly)
        {
            if (typeInSameAssembly == null)
                throw new ArgumentNullException("typeInSameAssembly");

            return FindCommandsInAssembly(typeInSameAssembly.Assembly);
        }

        public static IEnumerable<ConsoleCommand> FindCommandsInAllLoadedAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(FindCommandsInAssembly);
        }

        public static IEnumerable<ConsoleCommand> FindCommandsInAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

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
