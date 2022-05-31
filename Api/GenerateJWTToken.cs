using System.Collections.Generic;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;

namespace Api;

/// <summary>
///     Service class for performing authentication.
/// </summary>
public class GenerateJwtToken
{
    private readonly IJwtEncoder _jwtEncoder;

    public GenerateJwtToken()
    {
        // JWT specific initialization.
        IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
        IJsonSerializer serializer = new JsonNetSerializer();
        IBase64UrlEncoder base64Encoder = new JwtBase64UrlEncoder();
        _jwtEncoder = new JwtEncoder(algorithm, serializer, base64Encoder);
    }

    public string IssuingJwt(string user, string sessionToken)
    {
        var claims = new Dictionary<string, object>
        {
            // JSON representation of the user Reference with ID and display name
            { "username", user },

            // TODO: Add other claims here as necessary; maybe from a user database
            { "role", "admin" },

            { "token", sessionToken }
        };

        var token = _jwtEncoder.Encode(claims, "Your Secret Security key string"); // Put this key in config
        return token;
    }
}