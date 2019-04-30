using NUnit.Framework;
using System.IO;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Should_give_user_prompt_in_interactive_mode : ConsoleModeCommandSpecification
    {
        [Test]
        public void ConsoleModeCommandIsRunningForTheUser()
        {
            var output = new StringWriter();
            RunConsoleModeCommand(new string[]
            {
                    "echo-status -s 0",
                    "x",
            },
            inputIsFromUser: true, command: new StatusEchoCommand(),
            outputWriter: output).Invoke();

            StringAssert.Contains(ConsoleModeCommand.FriendlyContinuePrompt,
                output.ToString(),
                "the output does not contain a helpful prompt");
        }

        [Test]
        public void ConsoleModeCommandIsNotRunningForTheUser()
        {
            var output = new StringWriter();
            RunConsoleModeCommand(new string[]
                {
                    "echo-status -s 0",
                    "x",
                },
                inputIsFromUser: false, command: new StatusEchoCommand(),
                outputWriter: output).Invoke();
            StringAssert.DoesNotContain(ConsoleModeCommand.FriendlyContinuePrompt,
                output.ToString(),
                "the output contains the helpful prompt");
        }
    }
}
