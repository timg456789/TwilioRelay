using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Twilio;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace TwilioSmsRelay
{
    public class Function
    {
        protected List<string> knownNumbers = new List<string>
        {
            Environment.GetEnvironmentVariable("phoneNumberTwilioPurchased"),
            Environment.GetEnvironmentVariable("phoneNumberTwilioPurchasedNorthSanDiego"),
            Environment.GetEnvironmentVariable("phoneNumberTwilioPurchasedSouthSanDiego"),
            Environment.GetEnvironmentVariable("phoneNumberCellPhone")
        };

        private readonly TwilioRequestValidator requestValidator =
            new TwilioRequestValidator(Environment.GetEnvironmentVariable("twilioProductionToken"));

        public APIGatewayProxyResponse FunctionHandler(
            APIGatewayProxyRequest request,
            ILambdaContext context)
        {
            Console.WriteLine(JsonConvert.SerializeObject(request));
            TwilioClient.Init(Environment.GetEnvironmentVariable("twilioProductionSid"),
                Environment.GetEnvironmentVariable("twilioProductionToken"));

            var parameters = request.Body.Split('&').ToDictionary(
                x => WebUtility.UrlDecode(x.Split('=')[0]),
                x => WebUtility.UrlDecode(x.Split('=')[1])
            );

            if (!requestValidator.IsFromTwilio(request, parameters))
            {
                return Responses.ForbiddenResponse;
            }

            return new Relay(
                new ConsoleLogging(),
                knownNumbers,
                Environment.GetEnvironmentVariable("phoneNumberTwilioPurchased"),
                    Environment.GetEnvironmentVariable("phoneNumberCellPhone"))
                .RelayMessage(
                    parameters["From"],
                    parameters["Body"]);
        }

    }
}
