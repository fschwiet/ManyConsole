using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Components.DictionaryAdapter;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_have_required_parameters : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a no-op command that requires a parameter", delegate()
            {
                var noopCommand = new CommandWithRequiredParameter();
                var commands = new ConsoleCommand[] { noopCommand };

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

                    then("the option is actually received", delegate()
                    {
                        expect(() => noopCommand.Foo == "bar");
                    });
                });
            });

            given("a command that requires an integer parameter", () =>
            {
                int result = 0;
                var requiresInteger = arrange(() => new TestCommand()
                    .IsCommand("parse-int")
                    .HasRequiredOption<int>("value=", "The integer value", v => result = v));

                when("the command is passed an integer value", () =>
                {
                    StringWriter output = new StringWriter();

                    var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(requiresInteger,
                        new[] { "parse-int", "-value", "42" }, output));

                    then("the command is told the parameter", ()=>
                    {
                        expect(() => result == 42);
                    });

                    then("the return value is success", () => expect(() => exitCode == 0));
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
