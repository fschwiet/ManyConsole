using NUnit.Framework;
using System.IO;
using System.Text;

namespace ManyConsole.Tests
{
    public class Can_define_commands_with_aliases
    {
        [Test]
        public void CommandWithTwoAliasesDirect()
        {
            var output = run_command_with_parameters(new[] { "command" });
            StringAssert.AreEqualIgnoringCase("Executing command:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void CommandWithTwoAliasesFirst()
        {
            var output = run_command_with_parameters(new[] { "--command" });
            StringAssert.AreEqualIgnoringCase("Executing command:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void CommandWithTwoAliasesSecond()
        {
            var output = run_command_with_parameters(new[] { "-c" });
            StringAssert.AreEqualIgnoringCase("Executing command:", output.Trim(),
                "unexpected output for valid command");
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
        }
    }
}
