using System;
using System.Text.RegularExpressions;

namespace TwilioSmsRelay
{
    public class Validation
    {

        public static Tuple<string, string> SplitNumber(string body)
        {
            var forwardingToNumber = string.Empty;
            var forwardingMessage = string.Empty;
            int phoneLength = 12;
            if (body.Length >= phoneLength)
            {
                forwardingToNumber = body.Substring(0, phoneLength);
                forwardingMessage = body.Substring(phoneLength, body.Length - phoneLength).Trim();
            }
            return new Tuple<string, string>(forwardingToNumber, forwardingMessage);
        }

        public static string IsValidToForward(Tuple<string, string> message)
        {
            if (string.IsNullOrWhiteSpace(message.Item1))
            {
                return "No forwarding number provided.";
            }

            if (message.Item1.Length < 12)
            {
                return "Phone number must be at least 12 characters e.g. +15555555555";
            }

            var number = message.Item1.Substring(1);
            if (!new Regex(@"^\d+$").IsMatch(number))
            {
                return "Phone number must be only a plus sign and numbers followed by a space.";
            }

            return string.Empty;
        }
    }
}
