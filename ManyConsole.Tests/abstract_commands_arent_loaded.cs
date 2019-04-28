using NUnit.Framework;
using System.Linq;

namespace ManyConsole.Tests
{
    public class abstract_commands_arent_loaded
    {
        public abstract class AbstractCommand : ConsoleCommand
        {
        }

        public abstract class AnotherAbstractCommand : AbstractCommand
        {
            public AnotherAbstractCommand() {}
        }

        public class NonabstractCommand : AnotherAbstractCommand
        {
            public NonabstractCommand()
            {
                this.IsCommand("NonabstractCommand");
            }

            public override int Run(string[] remainingArguments)
            {
                return 0;
            }
        }

        [Test]
        public void AbstractCommandArentLoaded()
        {
            var commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(this.GetType());

            Assert.IsTrue(commands.Any(c => c.GetType() == typeof(NonabstractCommand)), "No non-abstract commands are found.");
            Assert.IsFalse(commands.Any(c => c.GetType() == typeof(AbstractCommand)), "AbstractCommands present - should be ignored.");
            Assert.IsFalse(commands.Any(c => c.GetType() == typeof(AnotherAbstractCommand)), "AnotherAbstractCommand present - should be ignored.");
        }

    }
}
