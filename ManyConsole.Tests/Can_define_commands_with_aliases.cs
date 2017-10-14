using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_define_commands_with_aliases : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("a command with 2 aliases", delegate
            {
                when("the arguments specify the command directly", delegate()
                {
                    var output = run_command_with_parameters(new[] { "command" });

                    then("the output has no errorstring", delegate()
                    {
                        expect(() => output.Trim() == "Executing command:");
                    });
                });
                when("the arguments specifies the first alias", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "--command" });

                    then("the output has no errorstring", delegate ()
                    {
                        expect(() => output.Trim() == "Executing command:");
                    });
                });
                when("the arguments specifies the second alias", delegate ()
                {
                    var output = run_command_with_parameters(new[] { "-c" });

                    then("the output has no errorstring", delegate ()
                    {
                        expect(() => output.Trim() == "Executing command:");
                    });
                });
            });
        }

        public class CommandWith2Aliases: ConsoleCommand
        {
            public CommandWith2Aliases()
            {
                this.IsCommand("command");
                this.HasAlias("--command");
                this.HasAlias("-c");
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
                        new CommandWith2Aliases()
                    },
                    parameters,
                    sw);

                return sb.ToString();
            });
        }
    }
}
