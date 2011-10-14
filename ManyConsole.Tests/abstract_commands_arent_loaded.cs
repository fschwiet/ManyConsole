using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NJasmine;

namespace ManyConsole.Tests
{
    public class abstract_commands_arent_loaded : GivenWhenThenFixture
    {
        public abstract class AbstractCommand : ConsoleCommand
        {
        }

        public class NonabstractCommand : AbstractCommand
        {
            public NonabstractCommand()
            {
                Command = "NonabstractCommand";
            }

            public override int Run()
            {
                return 0;
            }
        }

        public override void Specify()
        {
            it("when loading commands from an assembly, abstract commands are ignored", delegate
            {
                var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(this.GetType());

                expect(() => commands.Any(c => c.GetType() == typeof(NonabstractCommand)));
                expect(() => !commands.Any(c => c.GetType() == typeof(AbstractCommand)));
            });
        }
    }
}
