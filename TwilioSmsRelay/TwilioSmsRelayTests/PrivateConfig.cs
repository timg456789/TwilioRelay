using System.IO;
using Newtonsoft.Json;

namespace TwilioSmsRelayTests
{
    public class PrivateConfig
    {
        [JsonProperty("galleryUsername")]
        public string GalleryUsername { get; set; }

        [JsonProperty("galleryPassword")]
        public string GalleryPassword { get; set; }

        [JsonProperty("twilioTestSid")]
        public string TwilioTestSid { get; set; }

        [JsonProperty("twilioTestToken")]
        public string TwilioTestToken { get; set; }

        [JsonProperty("twilioProductionSid")]
        public string TwilioProductionSid { get; set; }

        [JsonProperty("twilioProductionToken")]
        public string TwilioProductionToken { get; set; }

        [JsonProperty("phoneNumberLandline")]
        public string PhoneNumberLandline { get; set; }

        [JsonProperty("phoneNumberCellPhone")]
        public string PhoneNumberCellPhone { get; set; }

        [JsonProperty("phoneNumberTwilioPurchased")]
        public string PhoneNumberTwilioPurchased { get; set; }

        [JsonProperty("phoneNumberTwilioPurchasedNorthSanDiego")]
        public string PhoneNumberTwilioPurchasedNorthSanDiego { get; set; }

        [JsonProperty("phoneNumberTwilioPurchasedSouthSanDiego")]
        public string PhoneNumberTwilioPurchasedSouthSanDiego { get; set; }

        [JsonProperty("twilioVoiceEndpoint")]
        public string TwilioVoiceEndpoint { get; set; }

        [JsonProperty("twilioSmsPotArn")]
        public string TwilioSmsPotArn { get; set; }

        public static string PersonalJson => "C:\\Users\\peon\\Desktop\\projects\\Memex\\personal.json";

        public static PrivateConfig CreateFromPersonalJson()
        {
            return Create(PersonalJson);
        }

        private static PrivateConfig Create(string fullPath)
        {
            var json = File.ReadAllText(fullPath);
            return JsonConvert.DeserializeObject<PrivateConfig>(json);
        }
    }
}
