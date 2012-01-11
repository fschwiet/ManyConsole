using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_have_required_parameters : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a command that requires a parameter", delegate()
            {
                var commands = new ConsoleCommand[] { new CommandWithRequiredParameter() };

                when("that command is ran without the parameter", delegate()
                {
                    StringWriter output = new StringWriter();

                    var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(commands, new[] { "required" }, output));

                    then("the output indicates the parameter wasn't specified", delegate()
                    {
                        expect(() => output.ToString().Contains("parameter not found: foo"));
                    });

                    then("the exit code indicates the call failed", delegate()
                    {
                        expect(() => exitCode == -1);
                    });
                });
            });
        }

        public class CommandWithRequiredParameter : ConsoleCommand
        {
            public CommandWithRequiredParameter()
            {
                this.IsCommand("required", "This command has a required parameter");
                this.HasOption("foo=", "This foo to use.", v => Foo = v);
            }

            public string Foo;

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }
    }
}
