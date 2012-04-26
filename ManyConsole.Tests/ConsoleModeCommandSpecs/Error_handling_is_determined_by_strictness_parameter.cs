using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Error_handling_is_determined_by_strictness_parameter : GivenWhenThenFixture
    {
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

        public override void Specify()
        {
            StatusEchoCommand.RunCount = 0;

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

                var result = arrange(() => ConsoleCommandDispatcher.DispatchCommand(
                    new ConsoleCommand[] {consoleModeCommand}, new string[0], fakeConsoleWriter));

                then("the return code is 0", delegate
                {
                    expect(() => result == 0);
                });

                then("all the commands run", delegate
                {
                    expect(() => StatusEchoCommand.RunCount == 3);
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

                var result = arrange(() => ConsoleCommandDispatcher.DispatchCommand(
                    new ConsoleCommand[] { consoleModeCommand }, new string[0], fakeConsoleWriter));


                then("the return code is -1", delegate
                {
                    expect(() => result == -1);
                });

                then("all the commands run", delegate
                {
                    expect(() => StatusEchoCommand.RunCount == 3);
                });
            });

            when("a command fails while running in strict mode", delegate
            {
                arrange(delegate
                {
                    var injectedInput = new StreamWriter(injectedInputStream);

                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("echo-status -s 456");
                    injectedInput.WriteLine("echo-status -s 0");
                    injectedInput.WriteLine("x");
                    injectedInput.Flush();

                    injectedInputStream.Seek(0, SeekOrigin.Begin);
                });

                var result = arrange(() => ConsoleCommandDispatcher.DispatchCommand(
                    new ConsoleCommand[] { consoleModeCommand }, new string[] {"-strict"}, fakeConsoleWriter));

                
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
