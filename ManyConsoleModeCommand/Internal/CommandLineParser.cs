using System;
using System.Runtime.InteropServices;

namespace ManyConsole.Internal
{
    public static class CommandLineParser
    {
        /// <summary>
        /// Splits a string in the same way that Windows splits up a command line into args 
        /// (i.e. identical to how this is implemented under the covers: http://msdn.microsoft.com/en-us/library/system.environment.getcommandlineargs.aspx)
        /// </summary>
        /// <param name="commandLine">a string containing command line arguments</param>
        /// <returns></returns>
        /// <remarks>taken from http://stackoverflow.com/q/298830/5351</remarks>
        public static string[] Parse(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }


        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);
    }
}
