using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ManyConsole.Tests
{
    public class Can_modify_command_behavior_after_parsing_and_before_running
    {
        public class OverridingCommand : ConsoleCommand
        {
            public OverridingCommand()
            {
                this.IsCommand("fail-me-maybe");
                this.HasOption<int>("n=", "number", v => Maybe = v);
            }

            public int? Maybe;

            public override int? OverrideAfterHandlingArgumentsBeforeRun(string[] remainingArguments)
            {
                return Maybe;
            }

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }

        [Test]
        public void ReturnCodeAndHalt()
        {
            var output = new StringWriter();
            var command = new OverridingCommand();

            var exitCode = ConsoleCommandDispatcher.DispatchCommand(command, new[] { "/n", "123" }, output);

            Assert.AreEqual(123, exitCode, "Exit code not 123 as expected");
            Assert.That(String.IsNullOrEmpty(output.ToString()), "Output is not empty");
        }

    }
}
