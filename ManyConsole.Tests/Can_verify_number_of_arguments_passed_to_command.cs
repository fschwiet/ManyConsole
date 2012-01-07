using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_verify_number_of_arguments_passed_to_command : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a command expecting 5 parameters", delegate
            {
                when("the command is called with no parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command" });

                    then("the output has an errorstring asking for 5 parameters", delegate()
                    {
                        expect(() => output.Contains("Invalid number of arguments-- expected 5 more."));
                    });
                });

                when("the command is called with 8 parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command", "1", "2", "3", "4", "5", "6", "7", "8" });

                    then("the output has an errorstring indicating the extra parameters", delegate()
                    {
                        expect(() => output.Contains("Extra parameters specified: 6, 7, 8"));
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

        public class CommandWith5Parameters : ConsoleCommand
        {
            public CommandWith5Parameters()
            {
                Command = "command";
                this.ArgumentsRequiredAfterOptions = 5;
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
                        new CommandWith5Parameters()
                    },
                    parameters,
                    sw);

                return sb.ToString();
            });
        }
    }
}
