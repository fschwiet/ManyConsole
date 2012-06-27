using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManyConsole.Tests
{
    public class TestCommand : ConsoleCommand
    {
        public Func<int> Action;

        public override int Run(string[] remainingArguments)
        {
            return Action();
        }
    }
}
