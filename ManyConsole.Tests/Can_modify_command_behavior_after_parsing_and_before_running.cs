using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_modify_command_behavior_after_parsing_and_before_running : GivenWhenThenFixture
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

        public override void Specify()
        {
            it("can specify a return code and halt execution", () =>
            {
                var output = new StringWriter();
                var command = new OverridingCommand();

                var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(command, new[] { "/n", "123" }, output));

                expect(() => exitCode == 123);
                expect(() => String.IsNullOrEmpty(output.ToString()));
            });
        }
    }
}
