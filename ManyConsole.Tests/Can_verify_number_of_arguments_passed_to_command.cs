using NUnit.Framework;
using System.IO;
using System.Text;

namespace ManyConsole.Tests
{
    public class Can_verify_number_of_arguments_passed_to_command
    {
        [Test]
        public void Expecting5CalledWith0()
        {
            var output = run_command_with_parameters(new[] { "command" });
            StringAssert.Contains("Invalid number of arguments-- expected 5 more.", output,
                "the output does not have an errorstring asking for 5 parameters");
        }
        [Test]
        public void Expecting5CalledWith8()
        {
            var output = run_command_with_parameters(new[] { "command", "1", "2", "3", "4", "5", "6", "7", "8" });
            StringAssert.Contains("Extra parameters specified: 6, 7, 8", output,
                "the output does not have an errorstring indicating the extra parameters");
        }
        [Test]
        public void Expecting5CalledWith5()
        {
            var output = run_command_with_parameters(new[] { "command", "1", "2", "3", "4", "5" });
            StringAssert.AreEqualIgnoringCase("Executing command:", output.Trim(),
                "unexpected output for valid command");
        }

        public class CommandWith5Parameters : ConsoleCommand
        {
            public CommandWith5Parameters()
            {
                this.IsCommand("command");
                HasAdditionalArguments(5);
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
                    new CommandWith5Parameters()
                },
                parameters,
                sw);

            return sb.ToString();
        }
    }
}
