using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ManyConsole
{
    public class ConsoleHelpAsException : Exception
    {
        public ConsoleHelpAsException(string message) : base(message)
        {
        }

        public static void WriterErrorMessage(Exception e, TextWriter tw)
        {
            var userFriendly = e as ConsoleHelpAsException;

            if (e is ConsoleHelpAsException || e is NDesk.Options.OptionException)
            {
                tw.WriteLine(e.Message);
            }
            else
            {
                tw.Write("Exception encountered: " + e.ToString());
            }
            
        }
    }
}
