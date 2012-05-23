using System;
using System.IO;
using FakeItEasy;
using ManyConsole.Internal;
using NJasmine;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public abstract class ConsoleModeCommandSpecification : GivenWhenThenFixture
    {
        public Func<int> RunConsoleModeCommand(string[] lines, bool inputIsFromUser, TextWriter outputWriter = null)
        {
            var injectedInputStream = new MemoryStream();

            var fakeConsoleWriter = outputWriter ?? new StringWriter();
            var fakeConsoleReader = new StreamReader(injectedInputStream);

            var consoleModeCommand = new ConsoleModeCommand(
                () => new ConsoleCommand[] {new Should_fail_strictly_on_error_when_running_noninteractive.StatusEchoCommand()},
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

            arrange(() => consoleModeCommand.RedirectionDetector = A.Fake<IConsoleRedirectionDetection>());
            arrange(() => A.CallTo(() => consoleModeCommand.RedirectionDetector.IsInputRedirected()).Returns(!inputIsFromUser));
            
            return () =>
                   ConsoleCommandDispatcher.DispatchCommand(new ConsoleCommand[] {consoleModeCommand}, new string[0],
                                                            fakeConsoleWriter);
        }
    }
}