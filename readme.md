### ManyConsole

Available on Nuget: http://nuget.org/List/Packages/ManyConsole

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