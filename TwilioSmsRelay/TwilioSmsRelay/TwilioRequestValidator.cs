using System;
using System.Collections.Generic;
using Amazon.Lambda.APIGatewayEvents;
using Twilio.Security;

namespace TwilioSmsRelay
{
    class TwilioRequestValidator
    {
        private readonly string twilioToken;

        public TwilioRequestValidator(string twilioToken)
        {
            this.twilioToken = twilioToken;
        }

        public virtual bool IsFromTwilio(APIGatewayProxyRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var url = request.Headers["X-Forwarded-Proto"] +
                          "://" + request.Headers["Host"] + "/" +
                          request.RequestContext.Stage +
                          request.Path;
                var signature = request.Headers["X-Twilio-Signature"];
                var requestValidator = new RequestValidator(twilioToken);
                return requestValidator.Validate(url, parameters, signature);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
