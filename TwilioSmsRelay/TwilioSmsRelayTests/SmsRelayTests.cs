using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
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
        public void Test_Auth()
        {
            var config = PrivateConfig.CreateFromPersonalJson();
            var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(config.TwilioProductionToken));
            var sigData = TwilioRequestValidator.GetSignatureData(
                "TWILIO WEB HOOK URL",
                "RAW TWILIO WEB HOOK POST DATA"
                    .Split('&')
                    .ToDictionary(
                        x => HttpUtility.UrlDecode(x.Split('=')[0]), // Note: Converts pluses to spaces
                        x => HttpUtility.UrlDecode(x.Split('=')[1])));
            output.WriteLine(sigData);
            var actualSignature = Convert.ToBase64String(hmacsha1.ComputeHash(Encoding.UTF8.GetBytes(sigData)));
            Assert.True(TwilioRequestValidator.SecureCompare(actualSignature, "TWILIO_SIGNATURE_FROM_HTTP_HEADER"));
        }
        
        //[Fact]
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

        //[Fact]
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

        [Fact]
        public void Message_Forwarding_Validation()
        {
            var split = Validation.SplitNumber("+15555555555 hey");
            Assert.Equal(string.Empty, Validation.IsValidToForward(split));
        }

        [Fact]
        public void List_Phone_Numbers()
        {
            var config = PrivateConfig.CreateFromPersonalJson();

            TwilioClient.Init(
                config.TwilioProductionSid,
                config.TwilioProductionToken);

            var numbers = IncomingPhoneNumberResource.Read(
                phoneNumber: new PhoneNumber("+17278886973"));

            output.WriteLine(numbers.Single().Sid);
        }

    }
}
