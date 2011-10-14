using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ManyConsole.Extensions;
using ManyConsole.Internal;
using NDesk.Options;
using System.Collections;

namespace ManyConsole
{
    public abstract class ConsoleCommand
    {
        public ConsoleCommand()
        {
            OneLineDescription = "";
            RemainingArgumentsHelpText = "";
            Options = new OptionSet();
            TraceCommandAfterParse = true;
            ExpectedArgumentsCount = 0;
            AutoReset = true;
        }

        internal string[] _skippedProperties = new[] { "Command", "RemainingArgumentsHelpText", "OneLineDescription", "Options", "TraceCommandAfterParse", "AutoReset", "ExpectedArgumentsCount" };

        public string Command { get; protected set; }
        public string OneLineDescription { get; protected set; }
        public string RemainingArgumentsHelpText { get; protected set; }
        public bool AutoReset { get; protected set; }
        public int ExpectedArgumentsCount { get; protected set; }
        
        public OptionSet Options { get; protected set; }
        
        public bool TraceCommandAfterParse { get; protected set; }

        public virtual void VerifyNumberOfArguments(string[] args)
        {
            if (args.Count() != ExpectedArgumentsCount)
                throw new ConsoleHelpAsException(string.Format("Invalid number of arguments. Expected {0}, found {1}.", ExpectedArgumentsCount, args.Count()));
        }

        /// <summary>
        /// Load the remaining arguments then validate all.
        /// </summary>
        /// <param name="remainingArguments">Arguments passed to the console after the parameters handled by the Options.</param>
        public virtual void FinishLoadingArguments(string[] remainingArguments) { }

        public virtual void ResetFields()
        {
            if (AutoReset)
            {

                var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !_skippedProperties.Contains(p.Name));

                var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !_skippedProperties.Contains(p.Name));

                foreach (var property in properties)
                {
                    Type propertyType = property.GetType();
                    // set reference types and nullable fields back to null, value types back to defaults
                    if (propertyType.IsValueType)
                    {
                        property.SetValue(this, Activator.CreateInstance(propertyType), new object[0]);
                    }
                    else
                    {
                        property.SetValue(this, null, new object[0]);
                    }
                }

                foreach (FieldInfo field in fields)
                {
                    Type fieldType = field.FieldType;
                    // set reference types and nullable fields back to null, value types back to defaults
                    // set generic lists back to being empty
                    if (fieldType.IsValueType)
                    {
                        field.SetValue(this, Activator.CreateInstance(fieldType));
                    }
                    else if (fieldType.IsGenericList())
                    {
                        var listItemType = fieldType.GetGenericArguments().First();
                        IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listItemType));
                        field.SetValue(this, list);
                    }
                    else
                    {
                        field.SetValue(this, null);
                    }
                }
            }
        }

        public abstract int Run();
    }
}