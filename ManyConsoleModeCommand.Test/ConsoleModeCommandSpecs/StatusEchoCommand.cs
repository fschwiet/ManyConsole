using System;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class StatusEchoCommand : ConsoleCommand
    {
        public static int RunCount = 0;

        public StatusEchoCommand()
        {
            this.IsCommand("echo-status", "Returns a particular status code");
            this.HasRequiredOption("s=", "Status code to return", v => StatusCode = Int32.Parse(v));
        }

        public int StatusCode;

        public override int Run(string[] remainingArguments)
        {
            RunCount++;
            return StatusCode;
        }
    }
}