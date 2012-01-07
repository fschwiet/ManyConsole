using System;
using System.IO;
using System.Xml.Linq;
using ManyConsole.Internal;
using NDesk.Options;

namespace ManyConsole
{
    public abstract class ConsoleCommand : ConsoleUtil
    {
        public ConsoleCommand()
        {
            OneLineDescription = "";
            Options = new OptionSet();
            TraceCommandAfterParse = true;
            RemainingArgumentsCount = 0;
            RemainingArgumentsHelpText = "";
        }

        public string Command { get; private set; }
        public string OneLineDescription { get; private set; }
        public OptionSet Options { get; protected set; }
        public bool TraceCommandAfterParse { get; private set; }
        public int? RemainingArgumentsCount { get; private set; }
        public string RemainingArgumentsHelpText { get; private set; }

        public ConsoleCommand IsCommand(string command, string oneLineDescription = "")
        {
            Command = command;
            OneLineDescription = oneLineDescription;
            return this;
        }

        public ConsoleCommand HasAdditionalArguments(int? count = 0, string helpText = "")
        {
            RemainingArgumentsCount = count;
            RemainingArgumentsHelpText = helpText;
            return this;
        }

        public ConsoleCommand AllowsAnyAdditionalArguments(string helpText = "")
        {
            HasAdditionalArguments(null, helpText);
            return this;
        }

        public ConsoleCommand SkipsCommandSummaryBeforeRunning()
        {
            TraceCommandAfterParse = false;
            return this;
        }

        public ConsoleCommand HasOption(string prototype, string description, Action<string> action)
        {
            Options.Add(prototype, description, action);
            return this;
        }

        public ConsoleCommand HasOption<T>(string prototype, string description, Action<T> action)
        {
            Options.Add(prototype, description, action);
            return this;
        }

        public ConsoleCommand HasOption(string prototype, string description, OptionAction<string,string> action)
        {
            Options.Add(prototype, description, action);
            return this;
        }

        public ConsoleCommand HasOption<TKey, TValue>(string prototype, string description, OptionAction<TKey,TValue> action)
        {
            Options.Add(prototype, description, action);
            return this;
        }

        public abstract int Run(string[] remainingArguments);
    }
}