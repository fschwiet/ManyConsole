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
            given("a no-op command that requires a parameter", delegate()
            {
                var commands = arrange(() => new ConsoleCommand[] { new CommandWithRequiredParameter() });

                when("that command is ran without the parameter", delegate()
                {
                    StringWriter output = new StringWriter();

                    var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(commands, 
                        new[] { "required" }, output));

                    then("the output indicates the parameter wasn't specified", delegate()
                    {
                        expect(() => output.ToString().Contains("Missing option: foo"));
                    });

                    then("the exit code indicates the call failed", delegate()
                    {
                        expect(() => exitCode == -1);
                    });
                });

                when("that command is ran with the parameter", delegate()
                {
                    StringWriter output = new StringWriter();

                    var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(commands, 
                        new[] { "required", "-foo", "bar" }, output));

                    then("the output is empty", delegate()
                    {
                        expect(() => string.IsNullOrEmpty(output.ToString().Trim()));
                    });

                    then("the exit code indicates the call succeeded", delegate()
                    {
                        expect(() => exitCode == 0);
                    });
                });
            });
        }

        public class CommandWithRequiredParameter : ConsoleCommand
        {
            public CommandWithRequiredParameter()
            {
                this.IsCommand("required", "This command has a required parameter");
                this.HasOption("ignored=", "An extra option.", v => { });
                this.HasRequiredOption("f|foo=", "This foo to use.", v => Foo = v);
                this.SkipsCommandSummaryBeforeRunning();
            }

            public string Foo;

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }
    }
}
