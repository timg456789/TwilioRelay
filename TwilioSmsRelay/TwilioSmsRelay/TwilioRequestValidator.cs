using System;
using System.Collections.Generic;
using System.Text;
using Amazon.Lambda.APIGatewayEvents;
using Twilio.Security;

namespace TwilioSmsRelay
{
    public class TwilioRequestValidator
    {
        private readonly string twilioToken;

        public TwilioRequestValidator(string twilioToken)
        {
            this.twilioToken = twilioToken;
        }

        public static bool SecureCompare(string a, string b)
        {
            if (a == null || b == null)
                return false;
            int length = a.Length;
            if (length != b.Length)
                return false;
            int num = 0;
            for (int index = 0; index < length; ++index)
                num |= (int) a[index] ^ (int) b[index];
            return num == 0;
        }

        public static string GetSignatureData(string url, IDictionary<string, string> parameters)
        {
            StringBuilder stringBuilder = new StringBuilder(url);
            if (parameters != null)
            {
                List<string> stringList = new List<string>((IEnumerable<string>) parameters.Keys);
                stringList.Sort((IComparer<string>) StringComparer.Ordinal);
                using (List<string>.Enumerator enumerator = stringList.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        string current = enumerator.Current;
                        stringBuilder.Append(current).Append(parameters[current] ?? "");
                    }
                }
            }
            return stringBuilder.ToString();
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
