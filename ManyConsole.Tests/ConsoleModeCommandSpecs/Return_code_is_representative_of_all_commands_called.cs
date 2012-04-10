using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Return_code_is_representative_of_all_commands_called : GivenWhenThenFixture
    {
        public class StatusEchoCommand : ConsoleCommand
        {
            public StatusEchoCommand()
            {
                this.IsCommand("echo-status", "Returns a particular status code");
                this.HasRequiredOption("s=", "Status code to return", v => StatusCode = int.Parse(v));
            }

            public int StatusCode;

            public override int Run(string[] remainingArguments)
            {
                return StatusCode;
            }
        }

        public override void Specify()
        {
            var injectedInputStream = new MemoryStream();

            var fakeConsoleWriter = new StringWriter();
            var fakeConsoleReader = new StreamReader(injectedInputStream);

            var consoleModeCommand = new ConsoleModeCommand(
                () => new ConsoleCommand[] { new StatusEchoCommand()}, 
                fakeConsoleWriter, 
                fakeConsoleReader);

            when("multiple commands run successfully", delegate
            {
                arrange(delegate
                {
                    var injectedInput = new StreamWriter(injectedInputStream);

                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("x");
                    injectedInput.Flush();

                    injectedInputStream.Seek(0, SeekOrigin.Begin);
                });

                var result = arrange(() => consoleModeCommand.Run(new string[0]));

                then("the return code is 0", delegate
                {
                    expect(() => result == 0);
                });
            });

            when("some of multiple commands fails", delegate
            {
                arrange(delegate
                {
                    var injectedInput = new StreamWriter(injectedInputStream);

                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("echo-status -s 2");
                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("x");
                    injectedInput.Flush();

                    injectedInputStream.Seek(0, SeekOrigin.Begin);
                });

                var result = arrange(() => consoleModeCommand.Run(new string[0]));

                then("the return code is -1", delegate
                {
                    expect(() => result == -1);
                });
            });
        }
    }
}
