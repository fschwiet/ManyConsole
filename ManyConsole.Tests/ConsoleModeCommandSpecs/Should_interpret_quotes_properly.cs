using System.Collections.Generic;
using NUnit.Framework;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Should_interpret_quotes_properly : ConsoleModeCommandSpecification
    {
        public class AccumulateStringsCommand : ConsoleCommand
        {
            public List<string> Marked = new List<string>();
            public List<string> Unmarked = new List<string>(); 

            public AccumulateStringsCommand()
            {
                this.IsCommand("accumulate-strings", "Accumulates strings.");
                this.HasOption("s=", "A string.", v => Marked.Add(v));
                this.AllowsAnyAdditionalArguments("And more strings");
            }

            public override int Run(string[] remainingArguments)
            {
                Unmarked.AddRange(remainingArguments);
                return 0;
            }
        }

        [Test]
        public void CommandRunWithQuotedInput()
        {
            var command = new AccumulateStringsCommand();

            RunConsoleModeCommand(new string[]
                {
                    "accumulate-strings -s \"one two three\" \"four five six\"",
                    "x",
                },
                inputIsFromUser: true, command: command).Invoke();

            Assert.That(command.Marked, Is.EquivalentTo(new[] { "one two three" }));
            Assert.That(command.Unmarked, Is.EquivalentTo(new[] { "four five six" }));         
        }
    }
}
