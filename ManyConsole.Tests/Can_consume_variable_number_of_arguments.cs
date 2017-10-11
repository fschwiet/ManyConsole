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
                    var output = run_command_with_parameters(new[] { "command1" });

                    then("the output has an errorstring asking for at least 2 parameters", delegate()
                    {
                        expect(() => output.Contains("Invalid number of arguments-- expected 2 more"));
                    });
                });

                when("the command is called with 1 parameter", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command1", "1" });

                    then("the output has an errorstring asking for at least 1 parameter more", delegate ()
                    {
                        expect(() => output.Contains("Invalid number of arguments-- expected 1 more"));
                    });
                });

                when("the command is called with 2 parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command1", "1", "2" });

                    then("the output has no errorstring", delegate()
                    {
                        expect(() => output.Trim() == "Executing command1:");
                    });
                });
                
                when("the command is called with 5 parameters", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command1", "1", "2", "3", "4", "5" });

                    then("the output has no errorstring", delegate()
                    {
                        expect(() => output.Trim() == "Executing command1:");
                    });
                });
            });

            given("a command expecting between 2 and 5 parameters", delegate
            {
                when("the command is called with no parameters", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command2" });

                    then("the output has an errorstring asking for at least 2 parameters", delegate ()
                    {
                        expect(() => output.Contains("Invalid number of arguments-- expected 2 more"));
                    });
                });

                when("the command is called with 1 parameter", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command2", "1" });

                    then("the output has an errorstring asking for at least 1 parameter more", delegate ()
                    {
                        expect(() => output.Contains("Invalid number of arguments-- expected 1 more"));
                    });
                });

                when("the command is called with 2 parameters", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command2", "1", "2" });

                    then("the output has no errorstring", delegate ()
                    {
                        expect(() => output.Trim() == "Executing command2:");
                    });
                });
                
                when("the command is called with 4 parameters", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command2", "1", "2", "3", "4" });

                    then("the output has no errorstring", delegate ()
                    {
                        expect(() => output.Trim() == "Executing command2:");
                    });
                });

                when("the command is called with 5 parameters", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command2", "1", "2", "3", "4", "5" });

                    then("the output has no errorstring", delegate ()
                    {
                        expect(() => output.Trim() == "Executing command2:");
                    });
                });
                
                when("the command is called with 6 parameters", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "command2", "1", "2", "3", "4", "5", "6" });

                    then("the output has an errorstring indicating the extra parameters", delegate ()
                    {
                        expect(() => output.Contains("Extra parameters specified: 6"));
                    });
                });
            });
        }

        public class CommandWithAtLeast2Parameters : ConsoleCommand
        {
            public CommandWithAtLeast2Parameters()
            {
                this.IsCommand("command1");
                HasAdditionalArgumentsBetween(2, null, "");
            }

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }

        public class CommandWith2To5Parameters : ConsoleCommand
        {
            public CommandWith2To5Parameters()
            {
                this.IsCommand("command2");
                HasAdditionalArgumentsBetween(2, 5, "");
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
                        new CommandWithAtLeast2Parameters(),
                        new CommandWith2To5Parameters()
                    },
                    parameters,
                    sw);

                return sb.ToString();
            });
        }
    }
}
