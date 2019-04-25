using System.ComponentModel;
using System.IO;
using NUnit.Framework;

namespace ManyConsole.Tests
{
    public class Show_useful_error_information
    {
        string TextWithinExpectedUsageHelp = "Expected usage:";

        [Test]
        public void UserTypesInputRejectedByNDeskOptions()
        {
            var trace = new StringWriter();

            var lastError = ConsoleCommandDispatcher.DispatchCommand(
                    new ConsoleCommand[] { new SomeCommandWithAParameter() },
                    new[] { "some", "/a" },
                    trace);

            // the error output gives the error message and typical help
            Assert.AreNotEqual(0, lastError);

            StringAssert.Contains("Missing required value for option '/a'", trace.ToString());
            StringAssert.Contains(TextWithinExpectedUsageHelp, trace.ToString());

            StringAssert.DoesNotContain("ndesk.options", trace.ToString().ToLower());
            StringAssert.DoesNotContain("exception", trace.ToString().ToLower());
    
            // a command causes other unexpected errors
            Assert.Throws<InvalidAsynchronousStateException>(() => ConsoleCommandDispatcher.DispatchCommand(
                        new ConsoleCommand[] { new SomeCommandThrowingAnException(), },
                        new string[0],
                        trace));
        }

        class SomeCommandWithAParameter : ConsoleCommand
        {
            public SomeCommandWithAParameter()
            {
                this.IsCommand("some");
                this.HasOption("a=", "a parameter", v => {});
            }

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }
        
        class SomeCommandThrowingAnException : ConsoleCommand
        {
            public SomeCommandThrowingAnException()
            {
                this.IsCommand("some");
            }

            public override int Run(string[] remainingArguments)
            {
                throw new InvalidAsynchronousStateException();
            }
        }
    }
}
