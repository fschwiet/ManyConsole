using NUnit.Framework;
using System.IO;

namespace ManyConsole.Tests
{
    public class Console_interface_is_simplified_when_there_is_only_one_command
    {
        private const int Success = 999;

        public class ExampleCommand : ConsoleCommand
        {
            public ExampleCommand()
            {
                this.IsCommand("Example");
                this.HasOption("f|foo=", "This foo to use.", v => Foo = v);
                this.SkipsCommandSummaryBeforeRunning();
            }

            public override int Run(string[] remainingArguments)
            {
                return Success;
            }

            public string Foo { get; set; }
        }

        [Test]
        public void NoParametersSpecified()
        {
            var exampleCommand = new ExampleCommand();

            var output = new StringWriter();
            var exitCode = ConsoleCommandDispatcher.DispatchCommand(exampleCommand, new string[0], output);

            then_the_command_runs_without_tracing_parameter_information(output, exitCode);

            Assert.IsNull(exampleCommand.Foo);
        }
        [Test]
        public void OnlyCommandIsSpecified()
        {
            var exampleCommand = new ExampleCommand();

            var output = new StringWriter();
            var exitCode = ConsoleCommandDispatcher.DispatchCommand(exampleCommand, new[] { "Example" }, output);

            then_the_command_runs_without_tracing_parameter_information(output, exitCode);

            Assert.IsNull(exampleCommand.Foo);
        }
        [Test]
        public void OnlyParameterIsNotCommand()
        {
            var exampleCommand = new ExampleCommand();

            var output = new StringWriter();
            var exitCode = ConsoleCommandDispatcher.DispatchCommand(exampleCommand, new[] { "/f=bar" }, output);

            then_the_command_runs_without_tracing_parameter_information(output, exitCode);

            StringAssert.AreEqualIgnoringCase("bar", exampleCommand.Foo);
        }
        [Test]
        public void CommandAndParameterSpecified()
        {
            var exampleCommand = new ExampleCommand();

            var output = new StringWriter();
            var exitCode = ConsoleCommandDispatcher.DispatchCommand(exampleCommand, new[] { "Example", "/f=bar" }, output);

            then_the_command_runs_without_tracing_parameter_information(output, exitCode);

            StringAssert.AreEqualIgnoringCase("bar", exampleCommand.Foo);
        }
        private void then_the_command_runs_without_tracing_parameter_information(StringWriter output, int exitCode)
        {
            Assert.That(string.IsNullOrEmpty(output.ToString().Trim()),
                "the output is not empty");
            Assert.AreEqual(Success, exitCode);
        }
    }
}
