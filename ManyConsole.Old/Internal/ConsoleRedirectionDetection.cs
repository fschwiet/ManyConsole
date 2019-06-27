using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ManyConsole.Internal
{
    using System;
    using System.Runtime.InteropServices;

    // implementation from http://stackoverflow.com/questions/3453220/how-to-detect-if-console-in-stdin-has-been-redirected

    public interface IConsoleRedirectionDetection
    {
        bool IsOutputRedirected();
        bool IsInputRedirected();
        bool IsErrorRedirected();
    }

    public class ConsoleRedirectionDetection : IConsoleRedirectionDetection
    {
        public bool IsOutputRedirected()
        {
            return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout));
        }
        public bool IsInputRedirected()
        {
            return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin));
        }
        public bool IsErrorRedirected()
        {
            return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr));
        }

        // P/Invoke:
        private enum FileType { Unknown, Disk, Char, Pipe };
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
        [DllImport("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
    }
}
