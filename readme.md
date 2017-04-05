### ManyConsole

Available on Nuget: http://nuget.org/List/Packages/ManyConsole

Thanks to Daniel González for providing some additional documentation: http://dgondotnet.blogspot.dk/2013/08/my-last-console-application-manyconsole.html

NDesk.Options is a great library for processing command-line parameters.  ManyConsole extends NDesk.Options to allow building console applications that support separate commands.

If you are not familiar with NDesk.Options, you should start by using that: http://www.ndesk.org/Options.  Add ManyConsole when you feel the urge to differentiate commands (you'll still need the NDesk.Options usage).

ManyConsole provides a console interface for the user to list available commands, call and get help for each.

To use ManyConsole:

- Create a command line app referencing the ManyConsole nuget.
- Have Program.Main call ConsoleCommandDispatcher (see https://github.com/fschwiet/ManyConsole/blob/master/SampleConsole/Program.cs)
  - You can use ConsoleCommandDispatcher to find commands in an assembly, or not.
- To add a command to your console application, inherit from ConsoleCommand.
  - See the sample comands at https://github.com/fschwiet/ManyConsole/tree/master/SampleConsole
  - Commands can be forced to show the user the help text by throwing an exception of type: ConsoleHelpAsException
  - There are a handful of methods you can call from the derived class's constructor to add metadata to the command.  Use autocompete to find them.

### Quick Start Guide

Run this from NuGet Package Management Console:

```posh
Install-Package ManyConsole
```

Drop this in to automatically load all of the commands that we'll create next:

```csharp
public class Program
{
    public static int Main(string[] args)
    {
        var commands = GetCommands();

        return ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);
    }

    public static IEnumerable<ConsoleCommand> GetCommands()
    {
        return ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
    }
}
```

Create a command with one optional, one required and a short and long description:

```csharp
public class PrintFileCommand : ConsoleCommand
{
    private const int Success = 0;
    private const int Failure = 2;

    public string FileLocation { get; set; }
    public bool StripCommaCharacter { get; set; }

    public PrintFileCommand()
    {
        // Register the actual command with a simple (optional) description.
        IsCommand("PrintFile", "Quick print utility.");
            
        // Add a longer description for the help on that specific command.
        HasLongDescription("This can be used to quickly read a file's contents " +
        "while optionally stripping out the ',' character.");
            
        // Required options/flags, append '=' to obtain the required value.
        HasRequiredOption("f|file=", "The full path of the file.", p => FileLocation = p);

        // Optional options/flags, append ':' to obtain an optional value, or null if not specified.
        HasOption("s|strip:", "Strips ',' from the file before writing to output.",
            t => StripCommaCharacter = t == null ? true : Convert.ToBoolean(t));
    }

    public override int Run(string[] remainingArguments)
    {
        try
        {
            var fileContents = File.ReadAllText(FileLocation);

            if (StripCommaCharacter)
                fileContents = fileContents.Replace(",", string.Empty);

            Console.Out.WriteLine(fileContents);

            return Success;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            Console.Error.WriteLine(ex.StackTrace);

            return Failure;
        }
    }
}
```

Ok, now when you run it, it should work:

```
>ManyConsoleDocumentation PrintFile -f "C:\HelloWorld.txt"

Extra parameters specified: C:\HelloWorld.txt

'PrintFile' - Quick print utility.

This can be used to quickly read a file's contents while optionally stripping out the ',' character.

Expected usage: ManyConsoleDocumentation.exe PrintFile <options>
<options> available:
  -f, --file                 The full path of the file.
  -s, --strip                Strips ',' from the file before writing to
                               output.
```

It doesn't work and thinks we specified an invalid parameter. This is because options that are followed by a parameter must have an '=' symbol, so update the two commands with `f|file=` and `s|strip=`, it should now work:

```
>ManyConsoleDocumentation PrintFile -f "C:\HelloWorld.txt"

Executing PrintFile (Quick print utility.):
    FileLocation : C:\HelloWorld.txt
    StripCommaCharacter : False

Hello, world!

>ManyConsoleDocumentation PrintFile -f "C:\HelloWorld.txt" -s true

Executing PrintFile (Quick print utility.):
    FileLocation : C:\HelloWorld.txt
    StripCommaCharacter : True

Hello world!
```

Now you can easily supply multiple commands with their own set of unique arguments:

```csharp
public class EchoCommand : ConsoleCommand
{
    public string ToEcho { get; set; }

    public EchoCommand()
    {
        IsCommand("Echo", "Echo's text");
        HasRequiredOption("t|text=", "The text to echo back.", t => ToEcho = t);
    }

    public override int Run(string[] remainingArguments)
    {
        Console.Out.WriteLine(ToEcho);

        return 0;
    }
}
```

Here's how the help looks, plus help for our two commands:

```
>ManyConsoleDocumentation help

Available commands are:

    Echo        - Echo's text
    PrintFile   - Quick print utility.

    help <name> - For help with one of the above commands

>ManyConsoleDocumentation help PrintFile

'PrintFile' - Quick print utility.

This can be used to quickly read a file's contents while optionally stripping out the ',' character.

Expected usage: ManyConsoleDocumentation.exe PrintFile <options>
<options> available:
  -f, --file=VALUE           The full path of the file.
  -s, --strip=VALUE          Strips ',' from the file before writing to
                               output.

>ManyConsoleDocumentation help Echo

'Echo' - Echo's text

Expected usage: ManyConsoleDocumentation.exe Echo <options>
<options> available:
  -t, --text=VALUE           The text to echo back.
```
