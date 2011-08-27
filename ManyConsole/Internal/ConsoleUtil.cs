using System.Linq;

namespace ManyConsole.Internal
{
    public abstract class ConsoleUtil
    {
        public static void VerifyNumberOfArguments(string[] args, int expectedArgumentCount)
        {
            if (args.Count() != expectedArgumentCount)
                throw new ConsoleHelpAsException("Invalid number of arguments.");
        }
    }
}