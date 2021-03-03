using NUnit.Framework;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ManyConsole.Tests
{
    public class Can_consume_variable_number_of_arguments_async
    {
        [Test]
        public async Task Expecting2CalledWith0Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command1" });
            StringAssert.Contains("Invalid number of arguments-- expected 2 more", output,
                "the output does not have an errorstring asking for 2 more parameters");
        }
        [Test]
        public async Task Expecting2CalledWith1Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command1", "1" });
            StringAssert.Contains("Invalid number of arguments-- expected 1 more", output,
                "the output does not have an errorstring asking for 1 more parameter");
        }
        [Test]
        public async Task Expecting2CalledWith2Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command1", "1", "2" });
            StringAssert.AreEqualIgnoringCase("Executing command1:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public async Task Expecting2CalledWith5Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command1", "1", "2", "3", "4", "5" });
            StringAssert.AreEqualIgnoringCase("Executing command1:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public async Task Expecting2To5CalledWith0Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command2" });
            StringAssert.Contains("Invalid number of arguments-- expected 2 more", output,
                "the output does not have an errorstring asking for 2 more parameters");
        }
        [Test]
        public async Task Expecting2To5CalledWith1Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command2", "1" });
            StringAssert.Contains("Invalid number of arguments-- expected 1 more", output,
                "the output does not have an errorstring asking for 1 more parameter");
        }
        [Test]
        public async Task Expecting2To5CalledWith2Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command2", "1", "2" });
            StringAssert.AreEqualIgnoringCase("Executing command2:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public async Task Expecting2To5CalledWith4Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command2", "1", "2", "3", "4" });
            StringAssert.AreEqualIgnoringCase("Executing command2:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public async Task Expecting2To5CalledWith5Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command2", "1", "2", "3", "4", "5" });
            StringAssert.AreEqualIgnoringCase("Executing command2:", output.Trim(),
                "unexpected output for valid command");
        }
        [Test]
        public async Task Expecting2To5CalledWith6Async()
        {
            var output = await run_command_with_parameters_async(new[] { "command2", "1", "2", "3", "4", "5", "6" });
            StringAssert.Contains("Extra parameters specified: 6", output,
                "the output does not have an errorstring iundicating superfluous parameter(s)");
        }

        public class CommandWithAtLeast2ParametersAsync : ConsoleCommand
        {
            public CommandWithAtLeast2ParametersAsync()
            {
                this.IsCommand("command1");
                HasAdditionalArgumentsBetween(2, null, "");
            }

            public override Task<int> RunAsync(string[] remainingArguments)
            {
                return Task.FromResult(0);
            }
        }

        public class CommandWith2To5ParametersAsync : ConsoleCommand
        {
            public CommandWith2To5ParametersAsync()
            {
                this.IsCommand("command2");
                HasAdditionalArgumentsBetween(2, 5, "");
            }

            public override Task<int> RunAsync(string[] remainingArguments)
            {
                return Task.FromResult(0);
            }
        }

        private async Task<string> run_command_with_parameters_async(string[] parameters)
        {
            StringBuilder sb = new StringBuilder();
            var sw = new StringWriter(sb);

            await ConsoleCommandDispatcher.DispatchCommandAsync(
                new ConsoleCommand[]
                {
                    new CommandWithAtLeast2ParametersAsync(),
                    new CommandWith2To5ParametersAsync()
                },
                parameters,
                sw);

            return sb.ToString();
        }
    }
}
