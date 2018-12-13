namespace ManyConsole.Internal
{
    using System;
    using System.Runtime.InteropServices;

    // implementation from http://stackoverflow.com/questions/3453220/how-to-detect-if-console-in-stdin-has-been-redirected
    //net 45 implementation https://stackoverflow.com/questions/3453220/how-to-detect-if-console-in-stdin-has-been-redirected
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
#if NET40
                return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdout));
#else
            return Console.IsOutputRedirected;
#endif
        }
        public bool IsInputRedirected()
        {
#if NET40
            return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stdin));
#else
            return Console.IsInputRedirected;
#endif
        }
        public bool IsErrorRedirected()
        {
#if NET40
            return FileType.Char != GetFileType(GetStdHandle(StdHandle.Stderr));
#else
            return Console.IsErrorRedirected;
#endif
        }

#if NET40
        // P/Invoke:
        private enum FileType { Unknown, Disk, Char, Pipe };
        private enum StdHandle { Stdin = -10, Stdout = -11, Stderr = -12 };
        [DllImport("kernel32.dll")]
        private static extern FileType GetFileType(IntPtr hdl);
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(StdHandle std);
#endif
    }
}
