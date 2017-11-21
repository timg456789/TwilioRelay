using System;

namespace TwilioSmsRelay
{
    public class ConsoleLogging : ILogging
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}
