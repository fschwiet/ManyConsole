using NUnit.Framework;

namespace ManyConsole.Tests.ConsoleModeCommandSpecs
{
    public class Should_fail_strictly_on_error_when_running_noninteractive : ConsoleModeCommandSpecification
    {
        [Test]
        public void ConsoleInputFromUserSuccess()
        {
            StatusEchoCommand.RunCount = 0;

            // multiple commands run successfully
            var result = RunConsoleModeCommand(new string[]
            {
                "echo-status -s 0",
                "echo-status -s 0",
                "echo-status -s 0",
                "x",
            }, true, new StatusEchoCommand()).Invoke();

            Assert.AreEqual(0, result);
            Assert.AreEqual(3, StatusEchoCommand.RunCount);

        }

        [Test]
        public void ConsoleInputFromUserFail()
        {
            StatusEchoCommand.RunCount = 0;

            var result = RunConsoleModeCommand(new string[]
            {
                "echo-status -s 0",
                "echo-status -s 2",
                "echo-status -s 0",
                "x",
            }, true, new StatusEchoCommand()).Invoke();

            Assert.AreEqual(-1, result);
            Assert.AreEqual(3, StatusEchoCommand.RunCount);
        }

        [Test]
        public void ConsoleInputNotFromUserSuccess()
        {
            StatusEchoCommand.RunCount = 0;

            var result = RunConsoleModeCommand(new string[]
                {
                    "echo-status -s 0",
                    "echo-status -s 456",
                    "echo-status -s 0",
                    "x",
                }, false, new StatusEchoCommand()).Invoke();

            Assert.AreEqual(456, result);
            Assert.AreEqual(2, StatusEchoCommand.RunCount);
        }
    }
}
