using ServiceBusEmulator.Abstractions.Security;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

internal class SR
{
    public string Format()
    {
        return null;
    }
}

namespace ServiceBusEmulator.Security
{


    internal class CbsTokenValidator : ITokenValidator
    {
        private const string DefaultSharedAccessKeyName = "all";
        private const string DefaultSharedAccessKey = "CLwo3FQ3S39Z4pFOQDefaiUd1dSsli4XOAj3Y9Uh1E=";

        private const string SharedAccessSignature = "SharedAccessSignature";
        private const string SignedResource = "sr";
        private const string Signature = "sig";
        private const string SignedKeyName = "skn";
        private const string SignedExpiry = "se";

        public static CbsTokenValidator Default { get; } = new CbsTokenValidator(DefaultSharedAccessKeyName, DefaultSharedAccessKey);

        public string SharedAccessKeyName { get; }

        public string SharedAccessKey { get; }

        public CbsTokenValidator(string sharedAccessKeyName, string sharedAccessKey)
        {
            SharedAccessKeyName = sharedAccessKeyName ?? throw new ArgumentNullException(nameof(sharedAccessKeyName));
            SharedAccessKey = sharedAccessKey ?? throw new ArgumentNullException(nameof(sharedAccessKey));
        }

        public void Validate(string token)
        {
            const char SasPairSeparator = '&';
            const char SasKeyValueSeparator = '=';
            const string SasFullName = SharedAccessSignature + " ";

            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentException(null, nameof(token));
            }

            if (!token.StartsWith(SasFullName, StringComparison.Ordinal))
            {
                throw new ArgumentException(null, nameof(token));
            }

            System.Collections.Generic.Dictionary<string, string> query = token
[SasFullName.Length..]
                .Split(new[] { SasPairSeparator }, StringSplitOptions.None)
                .Select(pair => pair.Split(new[] { SasKeyValueSeparator }, 2, StringSplitOptions.None))
                .ToDictionary(pair => pair[0], pair => pair.Length > 1 ? pair[1] : null);

            if (!query.TryGetValue(SignedResource, out string audienceUri))
            {
                throw new ArgumentException(null, nameof(token));
            }

            if (!query.TryGetValue(SignedExpiry, out string expiresOn))
            {
                throw new ArgumentException(null, nameof(token));
            }

            if (!query.TryGetValue(SignedKeyName, out string keyName))
            {
                throw new ArgumentException(null, nameof(token));
            }

            if (!query.TryGetValue(Signature, out string signature))
            {
                throw new ArgumentException(null, nameof(token));
            }

            keyName = HttpUtility.UrlDecode(keyName);

            if (keyName != SharedAccessKeyName)
            {
                throw new ArgumentException(null, nameof(token));
            }

            expiresOn = HttpUtility.UrlDecode(expiresOn);
            signature = HttpUtility.UrlDecode(signature);

            string sharedAccessSignature = Sign($"{audienceUri}\n{expiresOn}", Encoding.UTF8.GetBytes(SharedAccessKey));

            if (signature != sharedAccessSignature)
            {
                throw new ArgumentException(null, nameof(token));
            }
        }

        private static string Sign(string requestString, byte[] encodedSharedAccessKey)
        {
            using HMACSHA256 hmac = new(encodedSharedAccessKey);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestString)));
        }
    }
}
