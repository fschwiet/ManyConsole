using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class show_useful_command_summary : GivenWhenThenFixture
    {
        class SomeCommand : ConsoleCommand
        {
            public SomeCommand()
            {
                Command = "thecommand";
                OneLineDescription = "One-line description";
                PropertyB = "def";
            }

            public string FieldA = "abc";
            public string PropertyB { get; set; }
            public int? PropertyC { get; set; }

            public override int Run()
            {
                return 0;
            }
        }

        public override void Specify()
        {
            when("a simple command is run", delegate
            {
                StringBuilder result = new StringBuilder();
                arrange(delegate
                {
                    var sw = new StringWriter(result);

                    ConsoleCommandDispatcher.DispatchCommand(
                        new ConsoleCommand[]
                        {
                            new SomeCommand()
                        },
                        new string[] { "thecommand" },
                        sw);
                });

                then("the output includes a summary of the command", delegate
                {
                    expect(() => result.ToString() == @"
Executing thecommand (One-line description):
    FieldA : abc
    PropertyB : def
    PropertyC : null

");
                });
            });
        }
    }
}
