using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Should_give_user_prompt_in_interactive_mode : ConsoleModeCommandSpecification
    {
        public override void Specify()
        {
            var output = new StringWriter();

            when("the console mode command is running for the user", delegate()
            {
                var result = arrange(RunConsoleModeCommand(new string[]
                    {
                        "echo-status -s 0",
                        "x",
                    },
                    inputIsFromUser: true, command: new StatusEchoCommand(),
                    outputWriter: output));

                then("the output contains a helpful prompt", delegate
                {
                    expect(() => output.ToString().Contains(ConsoleModeCommand.FriendlyContinuePrompt));
                });
            });

            when("the console mode command is not running for the user", delegate()
            {
                var result = arrange(RunConsoleModeCommand(new string[]
                    {
                        "echo-status -s 0",
                        "x",
                    },
                    inputIsFromUser: false, command: new StatusEchoCommand(),
                    outputWriter: output));

                then("the output does not contain the helpful prompt", delegate
                {
                    expect(() => !output.ToString().Contains(ConsoleModeCommand.FriendlyContinuePrompt));
                });
            });
        }
    }
}
