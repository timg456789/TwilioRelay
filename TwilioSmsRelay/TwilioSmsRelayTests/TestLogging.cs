using TwilioSmsRelay;
using Xunit.Abstractions;

namespace TwilioSmsRelayTests
{
    public class TestLogging : ILogging
    {
        private readonly ITestOutputHelper output;

        public TestLogging(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void Log(string message)
        {
            output.WriteLine(message);
        }
    }
}
