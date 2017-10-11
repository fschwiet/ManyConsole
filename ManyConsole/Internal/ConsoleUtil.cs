using System.Linq;

namespace ManyConsole.Internal
{
    public abstract class ConsoleUtil
    {
        public static void VerifyNumberOfArguments(string[] args, int expectedArgumentCount)
        {
            if (expectedArgumentCount < 0)
            {
                if (args.Count() < (-expectedArgumentCount))
                    throw new ConsoleHelpAsException(
                        string.Format("Too few arguments-- expected at least {0}", (-expectedArgumentCount)));
            }
            else
            {
                if (args.Count() < expectedArgumentCount)
                    throw new ConsoleHelpAsException(
                        string.Format("Invalid number of arguments-- expected {0} more.", expectedArgumentCount - args.Count()));

                if (args.Count() > expectedArgumentCount)
                    throw new ConsoleHelpAsException("Extra parameters specified: " + string.Join(", ", args.Skip(expectedArgumentCount).ToArray()));
            }
        }
    }
}