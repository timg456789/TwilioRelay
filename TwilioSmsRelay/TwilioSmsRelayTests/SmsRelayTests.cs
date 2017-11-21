using System;
using System.Collections.Generic;
using Twilio;
using TwilioSmsRelay;
using Xunit;
using Xunit.Abstractions;

namespace TwilioSmsRelayTests
{
    public class SmsRelayTests
    {
        private readonly ITestOutputHelper output;

        public SmsRelayTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void Test_SMS_Receiving()
        {
            var config = PrivateConfig.CreateFromPersonalJson();

            TwilioClient.Init(
                config.TwilioProductionSid,
                config.TwilioProductionToken);

            var knownNumbers = new List<string> {"+15555555555"};

            var relay = new Relay(new TestLogging(output), knownNumbers, config.PhoneNumberTwilioPurchased, config.PhoneNumberCellPhone);
            relay.RelayMessage("+15555555555", "hi " + Guid.NewGuid().ToString().Substring(0, 10));
        }

        [Fact]
        public void Test_SMS_Forwarding()
        {
            var config = PrivateConfig.CreateFromPersonalJson();

            TwilioClient.Init(
                config.TwilioProductionSid,
                config.TwilioProductionToken);

            var knownNumbers = new List<string>
            {
                config.PhoneNumberCellPhone,
                config.PhoneNumberTwilioPurchased,
                config.PhoneNumberTwilioPurchasedNorthSanDiego,
                config.PhoneNumberTwilioPurchasedSouthSanDiego
            };

            var relay = new Relay(new TestLogging(output), knownNumbers, config.PhoneNumberTwilioPurchased, config.PhoneNumberCellPhone);
            relay.RelayMessage(
                config.PhoneNumberCellPhone,
                $"{config.PhoneNumberTwilioPurchasedNorthSanDiego} hi {Guid.NewGuid().ToString().Substring(0, 10)}"
            );
        }

    }
}
