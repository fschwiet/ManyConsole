
using NUnit.Framework;

namespace ManyConsole.Tests
{
    static class MyStringAssert
    {
        public static void ContainsInOrder(string actual, params string[] args)
        {
            int i = 0;
            bool result = true;
            foreach (var s in args)
            {
                int pos = actual.IndexOf(s);
                if (pos < i)
                    result = false;
                else
                    i = pos + 1;                
            }
            Assert.IsTrue(result);
        }

    }
}
