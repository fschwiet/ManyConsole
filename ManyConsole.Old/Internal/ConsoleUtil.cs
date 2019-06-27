using System.Linq;

namespace ManyConsole.Internal
{
    public abstract class ConsoleUtil
    {
        public static void VerifyNumberOfArguments(string[] args, int? expectedArgumentCountMin, int? expectedArgumentCountMax)
        {
            if (expectedArgumentCountMin.HasValue && args.Count() < expectedArgumentCountMin.Value)
                throw new ConsoleHelpAsException(
                    string.Format("Invalid number of arguments-- expected {0} more.", expectedArgumentCountMin.Value - args.Count()));

            if (expectedArgumentCountMax.HasValue && args.Count() > expectedArgumentCountMax.Value)
                throw new ConsoleHelpAsException("Extra parameters specified: " + string.Join(", ", args.Skip(expectedArgumentCountMax.Value).ToArray()));
        }
    }
}