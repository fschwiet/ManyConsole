namespace ManyConsole.Internal
{
    using System;

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
            return Console.IsOutputRedirected;
        }
        public bool IsInputRedirected()
        {
            return Console.IsInputRedirected;
        }
        public bool IsErrorRedirected()
        {
            return Console.IsErrorRedirected;
        }

    }
}
