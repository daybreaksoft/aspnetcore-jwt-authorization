# AspNetCore JWT Authorization
Daybreaksoft.AspNetCore.JWT.Authorization is an middleware that implements JWT authorization server. It allows to pass values that used by verify identity, then return an access token.
# Installing via NuGet
The easiest way to install Daybreaksoft.Extensions.Functions is via [NuGet](https://www.nuget.org/packages/Daybreaksoft.AspNetCore.JWT.Authorization).  
In Visual Studio's [Package Manager Console](https://docs.microsoft.com/zh-cn/nuget/tools/package-manager-console), enter the following command:
```bash
Install-Package Daybreaksoft.AspNetCore.JWT.Authorization
```
# Support Frameworks
- netstandard2.0
# How to use
Add services related Authentication and JWTAuthroziation.
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

    var issuer = "issuer.com";
    var audience = "audience.com";
    
    // The keys at least has 16 words.
    var securityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("mysupersecretykey"));
    
    // Add Authentication
    var tokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = securityKey,

        ValidateIssuer = true,
        ValidIssuer = issuer,

        ValidateAudience = true,
        ValidAudience = audience,

        ValidateLifetime = true,

        ClockSkew = TimeSpan.Zero
    };

    services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = tokenValidationParameters;
        });

    // Add Jwt authorization
    services.AddJwtAuthorization(typeof(IdentityVerification), options =>
    {
        options.Issuer = issuer;
        options.Audience = audience;
        options.Expiration = TimeSpan.FromMinutes(60);
        options.SecurityKey = securityKey;
    });
}
```
Uses JWTAuthorization middleware and Authentication middleware.
```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    // Use JWT Authorization
    app.UseJwtAuthorization();

    // Use Authentication
    app.UseAuthentication();

    app.UseMvc();
}
```
The JWT Authorization options.
```csharp
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
    /// Indicate the message when verify identity failed. Default value is 
    /// </summary>
    public string VerifyIdentityFailedMessage { get; set; } = "Invalid username or password.";

    /// <summary>
    /// Security key
    /// </summary>
    public SecurityKey SecurityKey { get; set; }
}
```
Implements how to verify identity. If verifies successful, then return an claim identity.
```csharp
public class IdentityVerification : IIdentityVerification
{
    public async Task<ClaimsIdentity> GetIdentity(HttpContext context)
    {
        if (context.Request.Form["username"] == "test" && context.Request.Form["password"] == "test")
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Name, "test"));
            identity.AddClaim(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(new {Custom1 = 1, Custom2 =2})));

            return await Task.FromResult(identity);
        }
        else
        {
            return null;
        }
    }
}
```
