using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Amazon.Lambda.APIGatewayEvents;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace TwilioSmsRelay
{
    public class Relay
    {
        private readonly ILogging logging;
        private readonly List<string> knownNumbers;
        private readonly string relayNumber;
        private readonly string defaultRelayNumber;
        
        public Relay(ILogging logging, List<string> knownNumbers, string relayNumber, string defaultRelayNumber)
        {
            this.logging = logging;
            this.knownNumbers = knownNumbers;
            this.relayNumber = relayNumber;
            this.defaultRelayNumber = defaultRelayNumber;
        }
        
        public APIGatewayProxyResponse RelayMessage(
            string from,
            string body)
        {
            if (!knownNumbers.Any(@from.Equals))
            {
                logging.Log("Not from twilio purchased. Forwarding to default SMS number.");
                var knownMessage = $"DEFAULTED FROM{from}: {body}";
                logging.Log(knownMessage);
                MessageResource.Create(
                    to: new PhoneNumber(defaultRelayNumber),
                    from: relayNumber,
                    body: knownMessage);
                return Responses.EmptyResponse;
            }

            var splitMessage = SplitNumber(body);
            var validation = IsValidToForward(splitMessage);
            if (!string.IsNullOrWhiteSpace(validation))
            {
                logging.Log("Not valid command. Forwarding to default SMS number.");
                var knownMessage = $"INVALID FROM{from} - {validation}: {body}";
                logging.Log(knownMessage);
                MessageResource.Create(
                    to: new PhoneNumber(defaultRelayNumber),
                    from: relayNumber,
                    body: knownMessage);
                return Responses.EmptyResponse;
            }

            if (!string.IsNullOrWhiteSpace(splitMessage.Item1) &&
                !string.IsNullOrWhiteSpace(splitMessage.Item2))
            {
                Send(new PhoneNumber(splitMessage.Item1), relayNumber, splitMessage.Item2);
                Send(new PhoneNumber(from), relayNumber, $"{from}:FORWARDED");
            }

            return Responses.EmptyResponse;
        }

        protected Tuple<string, string> SplitNumber(string body)
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

        protected string IsValidToForward(Tuple<string, string> message)
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
                return "Phone number must be only a plus sign and numbers followed by a space";
            }

            return string.Empty;
        }

        public void Send(PhoneNumber to, string from, string body)
        {
            var confirmationMessage = MessageResource.Create(to: to, from: from, body: body);
            if (confirmationMessage.ErrorCode.GetValueOrDefault() > 0 || !string.IsNullOrWhiteSpace(confirmationMessage.ErrorMessage))
            {
                logging.Log("Errors: " + confirmationMessage.ErrorCode + " - " + confirmationMessage.ErrorMessage);
            }
            else
            {
                logging.Log($"SENT {to} from {from}: {body}");
            }
        }

    }
}
