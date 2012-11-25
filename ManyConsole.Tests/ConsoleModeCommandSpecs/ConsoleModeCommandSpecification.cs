using System;
using System.IO;
using FakeItEasy;
using ManyConsole.Internal;
using NJasmine;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public abstract class ConsoleModeCommandSpecification : GivenWhenThenFixture
    {
        public Func<int> RunConsoleModeCommand(string[] lines, bool inputIsFromUser, ConsoleCommand command, TextWriter outputWriter = null)
        {
            var injectedInputStream = new MemoryStream();

            var fakeConsoleWriter = outputWriter ?? new StringWriter();
            var fakeConsoleReader = new StreamReader(injectedInputStream);

            var consoleModeCommand = new ConsoleModeCommand(
                () => new ConsoleCommand[] {command},
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

            IConsoleRedirectionDetection redirectionDetection = A.Fake<IConsoleRedirectionDetection>();
            arrange(() => consoleModeCommand.SetConsoleRedirectionDetection(redirectionDetection));
            arrange(() => A.CallTo(() => redirectionDetection.IsInputRedirected()).Returns(!inputIsFromUser));
            
            return () =>
                   ConsoleCommandDispatcher.DispatchCommand(new ConsoleCommand[] {consoleModeCommand}, new string[0],
                                                            fakeConsoleWriter);
        }
    }
}