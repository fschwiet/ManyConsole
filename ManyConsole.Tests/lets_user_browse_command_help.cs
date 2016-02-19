using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;
using NJasmine;
using NJasmine.Extras;

namespace ManyConsole.Tests
{
    public class lets_user_browse_command_help : GivenWhenThenFixture
    {
        public override void Specify()
        {
            given("we have some commands", delegate
            {
                var firstcommand = new TestCommand().IsCommand("command-a", "oneline description a");
                var secondCommand = new TestCommand().IsCommand("command-b", "oneline description b");

                var commands = new ConsoleCommand[]
                {
                    firstcommand,
                    secondCommand
                }.ToList();

                var writer = new StringWriter();

                WhenTheUserDoesNotSpecifyACommandThenShowAvailableCommands(commands, writer, firstcommand, secondCommand, new string[0]);
                WhenTheUserDoesNotSpecifyACommandThenShowAvailableCommands(commands, writer, firstcommand, secondCommand, new [] { "help"});

                ShouldShowHelpWhenRequested(commands, new string[] { "command-c", "/?" });
                ShouldShowHelpWhenRequested(commands, new string[] { "help", "command-c" });
            });
        }

        private void WhenTheUserDoesNotSpecifyACommandThenShowAvailableCommands(List<ConsoleCommand> commands, StringWriter writer,
                                                                                ConsoleCommand firstcommand,
                                                                                ConsoleCommand secondCommand, string[] arguments)
        {
            when("the user does not specify a command", delegate
                {
                    arrange(() => ConsoleCommandDispatcher.DispatchCommand(commands, arguments, writer));

                    then("the output contains a list of available commands", delegate
                        {
                            var output = writer.ToString();

                            Expect.That(output).ContainsInOrder(
                                firstcommand.Command,
                                firstcommand.OneLineDescription,
                                secondCommand.Command,
                                secondCommand.OneLineDescription);
                        });
                });
        }

        private void ShouldShowHelpWhenRequested(List<ConsoleCommand> commands, string[] consoleArguments)
        {
            var writer = new StringWriter();

            when("we call a command, asking for help", delegate
            {
                var commandC = new TestCommand()
                    .IsCommand("command-c", "one line description for C")
                    .HasLongDescription(
@"Lorem ipsum dolor sit amet, consectetur adipiscing elit,
sed do eiusmod tempor incididunt ut labore et dolore magna
aliqua. Ut enim ad minim veniam, quis nostrud exercitation
ullamco laboris nisi ut aliquip ex ea commodo consequat.
Duis aute irure dolor in reprehenderit in voluptate velit
esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
occaecat cupidatat non proident, sunt in culpa qui officia
deserunt mollit anim id est laborum.")
                    .HasAdditionalArguments(0, "<remaining> <args>")
                    .HasOption("o|option=", "option description", v => { });

                commands.Add(commandC);

                var exitCode = arrange(() => ConsoleCommandDispatcher.DispatchCommand(commands, consoleArguments, writer));

                then("the output contains a all help available for that command", delegate
                {
                    var output = writer.ToString();
                    Expect.That(output).ContainsInOrder(
                        commandC.Command,
                        commandC.OneLineDescription,
                        commandC.LongDescription,
                        commandC.RemainingArgumentsHelpText,
                        "-o",
                        "--option",
                        "option description");
                });

                then("the process exit code is non-zero", () => expect(() => exitCode == -1));
            });
        }
    }
}
