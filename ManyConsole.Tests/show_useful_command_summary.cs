using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManyConsole.Tests
{
    public class show_useful_command_summary
    {
        class SomeCommand : ConsoleCommand
        {
            public SomeCommand()
            {
                this.IsCommand("thecommand", "One-line description");
                PropertyB = "def";
            }

            public string FieldA = "abc";
            public string PropertyB { get; set; }
            public int? PropertyC { get; set; }
            public IEnumerable<int>  PropertyD = new int[] { 1,2,3 };

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }

        [Test]
        public void RunSimpleCommand()
        {
            StringBuilder result = new StringBuilder();
            var sw = new StringWriter(result);

            ConsoleCommandDispatcher.DispatchCommand(
                new ConsoleCommand[]
                {
                    new SomeCommand()
                },
                new string[] { "thecommand" },
                sw);

            // the output includes a summary of the command
            StringAssert.AreEqualIgnoringCase(@"
Executing thecommand (One-line description):
    FieldA : abc
    PropertyB : def
    PropertyC : null
    PropertyD : 1, 2, 3

", result.ToString());
        }
    }
}
