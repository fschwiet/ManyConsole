using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NDesk.Options;
using NJasmine;

namespace ManyConsole.Tests
{
    public class Can_overwrite_options_property : GivenWhenThenFixture
    {
        public class OverwriteCommand : ConsoleCommand
        {
            public int A;
            public int B;
            public string Result;

            public OverwriteCommand()
            {
                this.IsCommand("foo", "bar");
                this.HasOption<int>("A=", "first value", v => A = v);
                this.SkipsCommandSummaryBeforeRunning();

                var optionSet = new OptionSet();
                this.Options = optionSet;
                optionSet.Add<int>("B=", "second option", v => B = v);
            }

            public override int Run(string[] remainingArguments)
            {
                Result = A + "," + B;
                return 0;
            }
        }
        public override void Specify()
        {
            it("does not lose other arguments when property Options is overwritten", () =>
            {
                var command = new OverwriteCommand();
                var consoleOutput = new StringBuilder();

                var outputCode = ConsoleCommandDispatcher.DispatchCommand(
                    command, 
                    new []{"/A", "1", "/B", "2"}, 
                    new StringWriter(consoleOutput));

                expect(() => String.IsNullOrEmpty(consoleOutput.ToString()));
                expect(() => outputCode == 0);
                expect(() => command.Result == "1,2");
            });
        }
    }
}
