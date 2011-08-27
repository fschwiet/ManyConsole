using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ManyConsole.Internal
{
    public class ConsoleHelp
    {
        public static void ShowSummaryOfCommands(IEnumerable<ConsoleCommand> commands, TextWriter console)
        {
            console.WriteLine("Available commands are:");
            console.WriteLine();
                    
            foreach (var command in commands)
            {
                console.WriteLine("    {0}\t- {1}", command.Command, command.OneLineDescription);
            }
            console.WriteLine();
        }

        public static void ShowCommandHelp(ConsoleCommand selectedCommand, TextWriter console)
        {
            var haveOptions = selectedCommand.Options.Count > 0;

            console.WriteLine("'" + selectedCommand.Command + "' - " + selectedCommand.OneLineDescription);
            console.WriteLine();
            console.Write("Expected usage: {0} {1} ", AppDomain.CurrentDomain.FriendlyName, selectedCommand.Command);

            if (haveOptions)
                console.Write("<options> ");

            console.WriteLine(selectedCommand.RemainingArgumentsHelpText);

            if (haveOptions)
            {
                console.WriteLine("<options> available:");
                selectedCommand.Options.WriteOptionDescriptions(console);
            }
            console.WriteLine();
        }

        public static void ShowParsedCommand(ConsoleCommand consoleCommand, TextWriter tw)
        {
            if (!consoleCommand.TraceCommandAfterParse)
            {
                return;
            }

            string[] skippedProperties = new []{
                "Command",
                "RemainingArgumentsHelpText",
                "Options"
            };

            var deserializeRootElementName = consoleCommand.Command;

            var jsonObject = JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(consoleCommand), deserializeRootElementName);

            var nodeNames = jsonObject.Element(deserializeRootElementName).Nodes()
                .OfType<XElement>().Select(n => n.Name.LocalName).ToArray();

            var nodesToKeep = jsonObject.Element(deserializeRootElementName).Nodes()
                .OfType<XElement>()
                .Where(n => !skippedProperties.Contains(n.Name.LocalName)).ToArray();

            var newObject = new XElement(deserializeRootElementName);

            foreach(var node in nodesToKeep)
            {
                newObject.Add(node);
            }

            tw.WriteLine(JsonConvert.SerializeXNode(newObject, Formatting.Indented));
        }
    }
}
