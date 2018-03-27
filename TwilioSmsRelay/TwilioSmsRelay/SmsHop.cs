using System.Linq;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.AvailablePhoneNumberCountry;
namespace TwilioSmsRelay
{
    class SmsHop
    {
        public IncomingPhoneNumberResource Hop(
            string twilioAccountSid,
            string relaySid,
            ILogging logging)
        {
            var original = IncomingPhoneNumberResource.Fetch(relaySid);
            string relayPhoneNumber = original.PhoneNumber.ToString();
            logging.Log($"Releasing phone number {relayPhoneNumber} {relaySid}");
            IncomingPhoneNumberResource.Delete(relaySid);

            var localAvailableNumbers = LocalResource.Read("US");
            var desiredNumber = localAvailableNumbers.First();

            logging.Log($"Purchasing new phone number {desiredNumber.PhoneNumber}");

            var newNumber = IncomingPhoneNumberResource.Create(
                twilioAccountSid,
                desiredNumber.PhoneNumber,
                smsMethod: original.SmsMethod,
                smsUrl: original.SmsUrl);

            logging.Log($"New phone number purchased {newNumber.PhoneNumber} {newNumber.Sid}");
            return newNumber;
        }
    }
}
