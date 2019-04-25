using System;
using System.IO;
using System.Text;
using Mono.Options;
using NUnit.Framework;

namespace ManyConsole.Tests
{
    public class Can_overwrite_options_property 
    {
        public class OverwriteCommand : ConsoleCommand
        {
            public int A;
            public int B;
            public string Result;

            public OverwriteCommand()
            {
                this.IsCommand("foo", "bar");
                this.HasOption<int>("A=", "first value", v => A = v);
                this.SkipsCommandSummaryBeforeRunning();

                var optionSet = new OptionSet();
                this.Options = optionSet;
                optionSet.Add<int>("B=", "second option", v => B = v);
            }

            public override int Run(string[] remainingArguments)
            {
                Result = A + "," + B;
                return 0;
            }
        }
        [Test]
        public void DoNotLooseOtherArgumentsWhenPropertyOptionsAreOverwritten()
        {
            var command = new OverwriteCommand();
            var consoleOutput = new StringBuilder();

            var outputCode = ConsoleCommandDispatcher.DispatchCommand(
                command,
                new[] { "/A", "1", "/B", "2" },
                new StringWriter(consoleOutput));

            Assert.That(String.IsNullOrEmpty(consoleOutput.ToString()), "Console output is not empty");
            Assert.AreEqual(0, outputCode, "Output is not zero");
            StringAssert.AreEqualIgnoringCase("1,2", command.Result);
        }
    }
}
