using System;
using System.Collections.Generic;
using ManyConsole;
using NDesk.Options;

namespace MC.AX.DataUtility
{
    /// <summary>
    /// Example implementation of a ManyConsole command-line argument parser (ConsoleCommand) class
    /// </summary>
    public class ExampleCommand : ConsoleCommand
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
        public ExampleCommand()
        {
            this.IsCommand("Example", "Example implementation of a ManyConsole command-line argument parser Command");

            this.HasOption("b|booleanOption", "Boolean flag option", b => BooleanOption = true);

            //  Setting .Options directly is the old way to do this, you may prefer to call the helper
            //  method HasOption/HasRequiredOption.
            Options = new OptionSet()
            {
                {"l|list=", "Values to add to list", v => OptionalArgumentList.Add(v)},
                {"r|requiredArguments=", "Optional string argument requiring a value be specified afterwards", s => OptionalArgument1 = s},
                {"o|optionalArgument:", "Optional String argument which is null if no value follow is specified", s => OptionalArgument2 = s ?? "<no argument specified>"}
            };

            this.HasRequiredOption("requiredOption=", "Required string argument also requiring a value.", s => { });
            this.HasOption("anotherOptional=", "Another way to specify optional arguments", s => {});

            HasAdditionalArguments(2, "<Argument1> <Argument2>");
        }

        public string Argument1;
        public string Argument2;
        public string OptionalArgument1;
        public string OptionalArgument2;
        public bool BooleanOption;
        public List<string> OptionalArgumentList = new List<string>();

        public override int Run(string[] remainingArguments)
        {
            Argument1 = remainingArguments[0];
            Argument2 = remainingArguments[1];

            Console.WriteLine(@"Called Example command - Argument1 = ""{0}"" Argument2 = ""{1}"" BooleanOption: {2}", Argument1, Argument2, BooleanOption);

            OptionalArgumentList.ForEach((item) => Console.WriteLine(@"List Item {0} = ""{1}""", OptionalArgumentList.IndexOf(item), item));

            if (BooleanOption)
            {
                throw new Exception("Throwing unhandled exception because BooleanOption is true");
            }

            return 0;
        }
    }
}
