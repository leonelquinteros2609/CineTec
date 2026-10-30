using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CineTec.Core.Helpers
{
    public static class JwtHelper
    {
        public static string GenerateToken(
            string secretKey,
            string issuer,
            string audience,
            int userId,
            string nombre,
            string email,
            string rol,
            DateTime expiration)
        {
            var header = new Dictionary<string, object>
            {
                ["alg"] = "HS256",
                ["typ"] = "JWT"
            };

            var expEpoch = new DateTimeOffset(expiration).ToUnixTimeSeconds();
            var payload = new Dictionary<string, object>
            {
                ["iss"] = issuer,
                ["aud"] = audience,
                ["sub"] = userId.ToString(),
                ["id"] = userId,
                ["name"] = nombre,
                ["email"] = email,
                ["role"] = rol,
                ["rol"] = rol,
                ["exp"] = expEpoch
            };

            var headerJson = JsonSerializer.Serialize(header);
            var payloadJson = JsonSerializer.Serialize(payload);

            var encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
            var encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));
            var dataToSign = $"{encodedHeader}.{encodedPayload}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
            var encodedSignature = Base64UrlEncode(signature);

            return $"{dataToSign}.{encodedSignature}";
        }

        public static (bool Valid, Dictionary<string, string>? Claims) ValidateToken(string token, string secretKey)
        {
            try
            {
                var parts = token.Trim().Split('.');
                if (parts.Length != 3)
                {
                    return (false, null);
                }

                var dataToSign = $"{parts[0]}.{parts[1]}";
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
                var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign));
                var expectedSignature = Base64UrlEncode(signature);

                if (parts[2] != expectedSignature)
                {
                    return (false, null);
                }

                var payloadBytes = Base64UrlDecode(parts[1]);
                var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                var doc = JsonDocument.Parse(payloadJson);

                var claims = new Dictionary<string, string>();
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    claims[prop.Name] = prop.Value.ToString();
                }

                if (claims.TryGetValue("exp", out var expStr) && long.TryParse(expStr, out var expEpoch))
                {
                    var expDate = DateTimeOffset.FromUnixTimeSeconds(expEpoch);
                    if (expDate < DateTimeOffset.UtcNow)
                    {
                        return (false, null);
                    }
                }

                return (true, claims);
            }
            catch
            {
                return (false, null);
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            var output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }
            return Convert.FromBase64String(output);
        }
    }
}
