using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using FakeItEasy;
using ManyConsole.Internal;
using NJasmine;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Should_fail_strictly_on_error_when_running_noninteractive : GivenWhenThenFixture
    {
        public override void Specify()
        {
            StatusEchoCommand.RunCount = 0;

            given("console input is coming from the user", delegate
            {
                when("multiple commands run successfully", delegate
                {
                    var result = arrange(RunConsoleModeCommand(new string[]
                    {
                        "echo-status -s 0",
                        "echo-status -s 0",
                        "echo-status -s 0",
                        "x",
                    }, true));

                    then("the return code is 0", delegate
                    {
                        expect(() => result == 0);
                    });

                    then("all the commands run", delegate
                    {
                        expect(() => StatusEchoCommand.RunCount == 3);
                    });
                });

                when("one of multiple commands fails", delegate
                {
                    var arrangeAction = RunConsoleModeCommand(new string[]
                    {
                        "echo-status -s 0",
                        "echo-status -s 2",
                        "echo-status -s 0",
                        "x",
                    }, true);

                    var result = arrange(arrangeAction);

                    then("the return code is -1", delegate
                    {
                        expect(() => result == -1);
                    });

                    then("all the commands run", delegate
                    {
                        expect(() => StatusEchoCommand.RunCount == 3);
                    });
                });
            });

            given("console input is not coming from the the user", delegate
            {
                var result = arrange(RunConsoleModeCommand(new string[]
                    {
                        "echo-status -s 0",
                        "echo-status -s 456",
                        "echo-status -s 0",
                        "x",
                    }, false));

                then("execution stops after that command", delegate()
                {
                    expect(() => StatusEchoCommand.RunCount == 2);
                });

                then("the return value for the command is passed on", delegate()
                {
                    expect(() => result == 456);
                });
            });
        }

        Func<int> RunConsoleModeCommand(string[] lines, bool inputIsFromUser)
        {
            var injectedInputStream = new MemoryStream();

            var fakeConsoleWriter = new StringWriter();
            var fakeConsoleReader = new StreamReader(injectedInputStream);

            var consoleModeCommand = new ConsoleModeCommand(
                () => new ConsoleCommand[] {new StatusEchoCommand()},
                fakeConsoleWriter,
                fakeConsoleReader);

            arrange(delegate
            {
                var injectedInput = new StreamWriter(injectedInputStream);

                foreach (var line in lines)
                    injectedInput.WriteLine(line);
                injectedInput.Flush();

                injectedInputStream.Seek(0, SeekOrigin.Begin);
            });

            Func<int> arrangeAction =
                () =>
                ConsoleCommandDispatcher.DispatchCommand(new ConsoleCommand[] {consoleModeCommand}, new string[0],
                                                         fakeConsoleWriter);

            arrange(() => consoleModeCommand.RedirectionDetector = A.Fake<IConsoleRedirectionDetection>());
            arrange(() => A.CallTo(() => consoleModeCommand.RedirectionDetector.IsInputRedirected()).Returns(!inputIsFromUser));
            return arrangeAction;
        }

        public class StatusEchoCommand : ConsoleCommand
        {
            public static int RunCount = 0;

            public StatusEchoCommand()
            {
                this.IsCommand("echo-status", "Returns a particular status code");
                this.HasRequiredOption("s=", "Status code to return", v => StatusCode = int.Parse(v));
            }

            public int StatusCode;

            public override int Run(string[] remainingArguments)
            {
                RunCount++;
                return StatusCode;
            }
        }
    }
}
