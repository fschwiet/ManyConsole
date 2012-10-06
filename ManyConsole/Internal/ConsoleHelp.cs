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
            console.WriteLine("Available commands are:");
            console.WriteLine();

            var commandList = commands.ToList();
            var n = commandList.Max(c => c.Command.Length) + 1;
            var commandFormatString = "    {0,-" + n + "}- {1}";

            foreach (var command in commandList)
            {
                console.WriteLine(commandFormatString, command.Command, command.OneLineDescription);
            }
            console.WriteLine();
        }

        public static void ShowCommandHelp(ConsoleCommand selectedCommand, TextWriter console)
        {
            var haveOptions = selectedCommand.GetActualOptions().Count > 0;

            console.WriteLine("'" + selectedCommand.Command + "' - " + selectedCommand.OneLineDescription);
            console.WriteLine();
            console.Write("Expected usage: {0} {1} ", AppDomain.CurrentDomain.FriendlyName, selectedCommand.Command);

            if (haveOptions)
                console.Write("<options> ");

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

            string[] skippedProperties = new []{
                "Command",
                "OneLineDescription",
                "Options",
                "TraceCommandAfterParse",
                "RemainingArgumentsCount",
                "RemainingArgumentsHelpText",
                "RequiredOptions"
            };

            var properties = consoleCommand.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !skippedProperties.Contains(p.Name));

            var fields = consoleCommand.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !skippedProperties.Contains(p.Name));

            Dictionary<string,string> allValuesToTrace = new Dictionary<string, string>();

            foreach (var property in properties)
            {
                object value = property.GetValue(consoleCommand, new object[0]);
                allValuesToTrace[property.Name] = value != null ? value.ToString() : "null";
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

                readable = readable;
            }
            else if (value != null)
                readable = value.ToString();
            else
                readable = "null";
            return readable;
        }
    }
}
