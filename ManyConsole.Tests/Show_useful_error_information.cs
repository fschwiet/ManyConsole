using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;
using NUnit.Framework;

namespace ManyConsole.Tests
{
    public class Show_useful_error_information : GivenWhenThenFixture
    {
        string TextWithinExpectedUsageHelp = "Expected usage:";

        public override void Specify()
        {
            var trace = new StringWriter();

            when("the user types in input which is rejected by NDesk.Options", delegate
            {
                var lastError = arrange(() => ConsoleCommandDispatcher.DispatchCommand(
                    new ConsoleCommand[] { new SomeCommandWithAParameter() }, 
                    new[] { "some", "/a" }, 
                    trace));
                
                then("the error output gives the error message and typical help", delegate
                {
                    expect(() => lastError != 0);
                    
                    expect(() => trace.ToString().Contains("Missing required value for option '/a'"));
                    expect(() => trace.ToString().Contains(TextWithinExpectedUsageHelp));

                    expect(() => !trace.ToString().ToLower().Contains("ndesk.options"));
                    expect(() => !trace.ToString().ToLower().Contains("exception"));
                });
            });

            when("a command causes other unexpected errors", delegate
            {
                then("the exception passes through", delegate
                {
                    Assert.Throws<InvalidAsynchronousStateException>(() => ConsoleCommandDispatcher.DispatchCommand(
                        new ConsoleCommand[] { new SomeCommandThrowingAnException(), },
                        new string[0],
                        trace));
                });
            });
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
