using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
                var firstcommand = new InlinedCommand("command-a", "oneline description a");
                var secondCommand = new InlinedCommand("command-b", "oneline description b");

                var commands = new[]
                {
                    firstcommand,
                    secondCommand
                };

                when("we dispatch the commands with no arguments", delegate
                {
                    var writer = new StringWriter();

                    arrange(() => ConsoleCommandDispatcher.DispatchCommand(commands, new string[0], writer));

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
            });
        }
    }
}
