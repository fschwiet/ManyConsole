using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ManyConsole.Internal
{
    public class ConsoleHelp
    {
        public static void ShowSummaryOfCommands(IEnumerable<ConsoleCommand> commands, TextWriter console)
        {
            console.WriteLine();
            console.WriteLine("Available commands are:");
            console.WriteLine();

            string helpCommand = "help <name>";

            var commandList = commands.ToList();
            var n = commandList.Select(c => c.Command).Concat(new [] { helpCommand}).Max(c => c.Length) + 1;
            var commandFormatString = "    {0,-" + n + "}- {1}";

            foreach (var command in commandList)
            {
                console.WriteLine(commandFormatString, command.Command, command.OneLineDescription);
            }
            console.WriteLine();
            console.WriteLine(commandFormatString, helpCommand, "For help with one of the above commands");
            console.WriteLine();
        }

        public static void ShowCommandHelp(ConsoleCommand selectedCommand, TextWriter console, bool skipExeInExpectedUsage = false)
        {
            var haveOptions = selectedCommand.GetActualOptions().Count > 0;

            console.WriteLine();
            console.WriteLine("'" + selectedCommand.Command + "' - " + selectedCommand.OneLineDescription);
            if (selectedCommand.Aliases != null && selectedCommand.Aliases.Count > 0)
            {
                console.WriteLine("Aliases:");
                foreach (string alias in selectedCommand.Aliases)
                {
                    console.WriteLine("  " + alias);
                }
            }
            console.WriteLine();

            if (!string.IsNullOrEmpty(selectedCommand.LongDescription))
            {
                console.WriteLine(selectedCommand.LongDescription);
                console.WriteLine();
            }

            console.Write("Expected usage:");

            if (!skipExeInExpectedUsage)
            {
                console.Write(" " + AppDomain.CurrentDomain.FriendlyName);
            }

            console.Write(" " + selectedCommand.Command);

            if (haveOptions)
                console.Write(" <options> ");

            console.WriteLine(selectedCommand.RemainingArgumentsHelpText);

            if (haveOptions)
            {
                console.WriteLine("<options> available:");
                selectedCommand.GetActualOptions().WriteOptionDescriptions(console);
            }
            console.WriteLine();
        }

        public static void ShowParsedCommand(ConsoleCommand consoleCommand, TextWriter consoleOut)
        {
            if (!consoleCommand.TraceCommandAfterParse)
            {
                return;
            }

            var properties = consoleCommand.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !consoleCommand.TraceCommandSkipProperties.Contains(p.Name));

            var fields = consoleCommand.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !consoleCommand.TraceCommandSkipProperties.Contains(p.Name));

            Dictionary<string,string> allValuesToTrace = new Dictionary<string, string>();

            foreach (var property in properties)
            {
                allValuesToTrace[property.Name] = MakeObjectReadable(property.GetValue(consoleCommand, new object[0]));
            }

            foreach (var field in fields)
            {
                allValuesToTrace[field.Name] = MakeObjectReadable(field.GetValue(consoleCommand));
            }

            consoleOut.WriteLine();

            string introLine = String.Format("Executing {0}", consoleCommand.Command);

            if (string.IsNullOrEmpty(consoleCommand.OneLineDescription))
                introLine = introLine + ":";
            else
                introLine = introLine + " (" + consoleCommand.OneLineDescription + "):";

            consoleOut.WriteLine(introLine);
            
            foreach(var value in allValuesToTrace.OrderBy(k => k.Key))
                consoleOut.WriteLine("    " + value.Key + " : " + value.Value);

            consoleOut.WriteLine();
        }

        static string MakeObjectReadable(object value)
        {
            string readable;

            if (value is System.Collections.IEnumerable && !(value is string))
            {
                readable = "";
                var separator = "";

                foreach (var member in (IEnumerable) value)
                {
                    readable += separator + MakeObjectReadable(member);
                    separator = ", ";
                }
            }
            else if (value != null)
                readable = value.ToString();
            else
                readable = "null";
            return readable;
        }
    }
}
