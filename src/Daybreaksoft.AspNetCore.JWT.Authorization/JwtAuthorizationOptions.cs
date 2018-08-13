using System;
using Microsoft.IdentityModel.Tokens;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
public class JwtAuthorizationOptions
{
    /// <summary>
    /// Indicate what's the token request scheme. Default value is Bearer.
    /// </summary>
    public string Scheme { get; set; } = "Bearer";

    /// <summary>
    /// Indicate what's the token request path. Default value is /token.
    /// </summary>
    public string Path { get; set; } = "/token";

    /// <summary>
    /// If this value not be null, a { sub, 'subject'} claim will be added.
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// If this value is not null, a { iss, 'issuer' } claim will be added.
    /// </summary>
    public string Issuer { get; set; }

    /// <summary>
    /// If this value is not null, a { aud, 'audience' } claim will be added.
    /// </summary>
    public string Audience { get; set; }

    /// <summary>
    /// Indicate when does the token expiry. Default value is 60 mins.
    /// </summary>
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(60);
        
    /// <summary>
    /// Security key
    /// </summary>
    public SecurityKey SecurityKey { get; set; }
}
}
