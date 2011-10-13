using System;
using System.Collections.Generic;
using ManyConsole;
using NDesk.Options;

namespace MC.AX.DataUtility
{
    /// <summary>
    /// Example implementation of a ManyConsole command-line argument parser (ConsoleCommand) class
    /// </summary>
    public class Example : ConsoleCommand
    {
        /// <summary>
        /// Configure command options and describe details in the class contructor
        /// </summary>
        /// <example>
        /// Example usage:
        /// 
        /// SampleConsole.exe example "option one" two -b -o "optional argument" -l=first -l=second -l "the third option"
        /// 
        /// Expected output:
        /// 
        /// Executing Example (Example implementation of a ManyConsole command-line argument
        /// parser Command):
        ///    Argument1 : option one
        ///    Argument2 : two
        ///    BooleanOption : True
        ///    OptionalArgument1 : optional argument
        ///    OptionalArgumentList : System.Collections.Generic.List`1[System.String]
        ///
        ///Called Example command - Argument1 = "option one" Argument2 = "two" BooleanOptio
        ///n: True
        ///List Item 0 = "first"
        ///List Item 1 = "second"
        ///List Item 2 = "the third option"
        /// </example>
        public Example()
        {
            Command = "Example";
            OneLineDescription = "Example implementation of a ManyConsole command-line argument parser Command";
            RemainingArgumentsHelpText = "<Argument1> <Argument2>";

            // assign delegates to assign values from the optional command line args
            Options = new OptionSet()
            {
                {"b|booleanOption", "Boolean flag option", b => BooleanOption = b != null},
                {"l|list=", "Values to add to list", v => OptionalArgumentList.Add(v)},
                {"o|optionalArgument=", "Optional string argument", s => OptionalArgument1 = s}
            };
        }

        public string Argument1;
        public string Argument2;
        public string OptionalArgument1;
        public bool BooleanOption;
        public List<string> OptionalArgumentList = new List<string>();

        public override void FinishLoadingArguments(string[] remainingArguments)
        {
            // validate the number of expected arguments
            VerifyNumberOfArguments(remainingArguments, 2);
            // assign values from the command line args
            Argument1 = remainingArguments[0];
            Argument2 = remainingArguments[1];
        }

        public override int Run()
        {
            Console.WriteLine(@"Called Example command - Argument1 = ""{0}"" Argument2 = ""{1}"" BooleanOption: {2}", Argument1, Argument2, BooleanOption);

            OptionalArgumentList.ForEach((item) => Console.WriteLine(@"List Item {0} = ""{1}""", OptionalArgumentList.IndexOf(item), item));

            return 0;
        }
    }
}
