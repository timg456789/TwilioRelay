using System;
using System.Collections.Generic;
using System.Linq;
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

            if (body.Equals("hop", StringComparison.OrdinalIgnoreCase))
            {
                var toNumber = IncomingPhoneNumberResource.Read(phoneNumber: new PhoneNumber(relayNumber)).Single();
                
                var newNumber = new SmsHop().Hop(Environment.GetEnvironmentVariable("twilioProductionSid"), toNumber.Sid, logging);

                MessageResource.Create(
                    to: from,
                    from: relayNumber,
                    body:
                    $"New phone number for Twilio Relay {newNumber.PhoneNumber.ToString()} - " +
                    $"{newNumber.Sid}");
                return Responses.EmptyResponse;
            }

            var splitMessage = Validation.SplitNumber(body);
            var validation = Validation.IsValidToForward(splitMessage);
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
