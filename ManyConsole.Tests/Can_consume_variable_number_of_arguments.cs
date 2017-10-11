using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_consume_variable_number_of_arguments : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a command expecting at least 2 parameters", delegate
            {
                when("the command is called with no parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command" });

                    then("the output has an errorstring asking for at least 2 parameters", delegate()
                    {
                        expect(() => output.Contains("Too few arguments-- expected at least 2"));
                    });
                });

                when("the command is called with 2 parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command", "1", "2" });

                    then("the output has no errorstring", delegate()
                    {
                        expect(() => output.Trim() == "Executing command:");
                    });
                });


                when("the command is called with 5 parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command", "1", "2", "3", "4", "5" });

                    then("the output has no errorstring", delegate()
                    {
                        expect(() => output.Trim() == "Executing command:");
                    });
                });
            });
        }

        public class CommandWithAtLeast2Parameters : ConsoleCommand
        {
            public CommandWithAtLeast2Parameters()
            {
                this.IsCommand("command");
                HasMinimumAdditionalArguments(2);
            }

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }

        private string run_command_with_parameters(string[] parameters)
        {
            return arrange(delegate
            {
                StringBuilder sb = new StringBuilder();
                var sw = new StringWriter(sb);

                ConsoleCommandDispatcher.DispatchCommand(
                    new ConsoleCommand[]
                    {
                        new CommandWithAtLeast2Parameters()
                    },
                    parameters,
                    sw);

                return sb.ToString();
            });
        }
    }
}
