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
                string result = null;
                var noopCommand = new TestCommand()
                    .IsCommand("required", "This command has a required parameter")
                    .HasOption("ignored=", "An extra option.", v => { })
                    .HasRequiredOption("f|foo=", "This foo to use.", v => result = v)
                    .SkipsCommandSummaryBeforeRunning();
                
                when_the_command_is_ran_without_the_parameter_then_the_console_gives_error_output(noopCommand, "foo");

                when("that command is ran with the parameter", delegate()
                {
                    StringWriter output = new StringWriter();

                    var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(noopCommand, 
                        new[] { "required", "-foo", "bar" }, output));

                    then("the exit code indicates the call succeeded", delegate()
                    {
                        expect(() => exitCode == 0);
                    });

                    then("the option is actually received", delegate()
                    {
                        expect(() => result == "bar");
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

                when_the_command_is_ran_without_the_parameter_then_the_console_gives_error_output(requiresInteger, "value");
            });
        }

        void when_the_command_is_ran_without_the_parameter_then_the_console_gives_error_output(ConsoleCommand command, string parameterName)
        {
            when("that command is ran without the parameter", delegate()
            {
                StringWriter output = new StringWriter();

                var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(command,new[] {command.Command}, output));

                then("the output indicates the parameter wasn't specified",
                     delegate() { expect(() => output.ToString().Contains("Missing option: " + parameterName)); });

                then("the exit code indicates the call failed", delegate() { expect(() => exitCode == -1); });
            });
        }
    }
}
