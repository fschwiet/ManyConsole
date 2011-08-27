### ManyConsole

Available on Nuget: *pending*

NDesk.Options is a great library for processing command-line parameters.  ManyConsole extends NDesk.Options to allow building console applications that support separate commands.

ManyConsole provides a console interface for the user to list available commands, call and get help for each.

To use ManyConsole:
  1.  Create a command line app referencing the ManyConsole Nuget package.
  2.  Have Program.Main call (*pending*)
  3.  For each command, create a class inheriting from ConsoleCommand
      - In the command class's constructor, set "Command" to the text for the command (and other properties
      - Overload Run()
