using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Should_fail_strictly_on_error_when_running_noninteractive : ConsoleModeCommandSpecification
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
                    }, true, new StatusEchoCommand()));

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
                    }, true, new StatusEchoCommand());

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
                    }, false, new StatusEchoCommand()));

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
    }
}
