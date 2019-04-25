using NUnit.Framework;
using System.IO;
using System.Text;

namespace ManyConsole.Tests
{
    public class Can_consume_variable_number_of_arguments
    {
        [Test]
        public void Expecting2CalledWith0()
        {
            var output = run_command_with_parameters(new[] { "command1" });
            StringAssert.Contains("Invalid number of arguments-- expected 2 more", output,
                "the output does not have an errorstring asking for 2 more parameters");
        }
        [Test]
        public void Expecting2CalledWith1()
        {
            var output = run_command_with_parameters(new[] { "command1", "1" });
            StringAssert.Contains("Invalid number of arguments-- expected 1 more", output,
                "the output does not have an errorstring asking for 1 more parameter");
        }
        [Test]
        public void Expecting2CalledWith2()
        {
            var output = run_command_with_parameters(new[] { "command1", "1", "2" });
            StringAssert.AreEqualIgnoringCase("Executing command1:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void Expecting2CalledWith5()
        {
            var output = run_command_with_parameters(new[] { "command1", "1", "2", "3", "4", "5" });
            StringAssert.AreEqualIgnoringCase("Executing command1:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void Expecting2To5CalledWith0()
        {
            var output = run_command_with_parameters(new[] { "command2" });
            StringAssert.Contains("Invalid number of arguments-- expected 2 more", output,
                "the output does not have an errorstring asking for 2 more parameters");
        }
        [Test]
        public void Expecting2To5CalledWith1()
        {
            var output = run_command_with_parameters(new[] { "command2", "1" });
            StringAssert.Contains("Invalid number of arguments-- expected 1 more", output,
                "the output does not have an errorstring asking for 1 more parameter");
        }
        [Test]
        public void Expecting2To5CalledWith2()
        {
            var output = run_command_with_parameters(new[] { "command2", "1", "2" });
            StringAssert.AreEqualIgnoringCase("Executing command2:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void Expecting2To5CalledWith4()
        {
            var output = run_command_with_parameters(new[] { "command2", "1", "2", "3", "4" });
            StringAssert.AreEqualIgnoringCase("Executing command2:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void Expecting2To5CalledWith5()
        {
            var output = run_command_with_parameters(new[] { "command2", "1", "2", "3", "4", "5" });
            StringAssert.AreEqualIgnoringCase("Executing command2:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public void Expecting2To5CalledWith6()
        {
            var output = run_command_with_parameters(new[] { "command2", "1", "2", "3", "4", "5", "6" });
            StringAssert.Contains("Extra parameters specified: 6", output,
                "the output does not have an errorstring iundicating superfluous parameter(s)");
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
        }
    }
}
