using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManyConsole;

namespace SampleConsole
{
    public class ThrowException : ConsoleCommand
    {
        public ThrowException()
        {
            this.IsCommand("throw-exception", "Throws an exception.");
            this.HasOption("m=", "Error message to be thrown.", v => Message = v);
        }

        public string Message = "Command ThrowException threw an exception with this message.";

        public override int Run(string[] remainingArguments)
        {
            throw new Exception(Message);
        }
    }
}
