using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public class JwtAuthorizationMiddleware : IMiddleware
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly JwtAuthorizationOptions _options;
        private readonly ILogger<JwtAuthorizationMiddleware> _logger;

        public JwtAuthorizationMiddleware(
            IServiceProvider serviceProvider,
            IOptions<JwtAuthorizationOptions> options,
            ILogger<JwtAuthorizationMiddleware> logger = null)
        {
            _serviceProvider = serviceProvider;
            _options = options.Value;
            _logger = logger;
        }

        /// <summary>
        /// Request handling method.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Http.HttpContext for the current request.</param>
        /// <param name="next">The delegate representing the remaining middleware in the request pipeline.</param>
        /// <returns>A System.Threading.Tasks.Task that represents the execution of this middleware.</returns>
        public Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                return next(context);
            }

            _logger?.LogDebug($"Accessed path {_options.Path}");

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST") || !context.Request.HasFormContentType)
            {
                var errMessage = $"Bad request. Request must be application/x-www-form-urlencoded. Current request Content Type is {context.Request.ContentType}";
                _logger?.LogError(errMessage);

                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync(errMessage);
            }

            return WriteTokenAsync(context);
        }

        /// <summary>
        /// Generate token and then output if authenticated 
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Http.HttpContext for the current request.</param>
        /// <returns></returns>
        private async Task WriteTokenAsync(HttpContext context)
        {
            _logger?.LogDebug("Attempting to get identity.");

            // Try to get identity (sign in)
            var identityResult = await _serviceProvider.GetRequiredService<IIdentityVerification>().GetIdentity(context);
            if (identityResult == null)
            {
                throw new ArgumentNullException(nameof(identityResult));
            }
            else
            {
                if (identityResult.Identity == null)
                {
                    _logger?.LogError("Verify identity failed.");

                    context.Response.StatusCode = 400;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(string.IsNullOrEmpty(identityResult.ErrorMessage) ? "Invalid username or password." : identityResult.ErrorMessage);

                    return;
                }
            }

            var now = DateTime.UtcNow;

            // Create clamins
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            if (!string.IsNullOrEmpty(_options.Subject))
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, _options.Subject));
            }
            claims.AddRange(identityResult.Identity.Claims);

            // Create the JWT and write it to a string
            _logger?.LogDebug("Attempting to generate jwt token.");

            // build signing credentials.
            var signingCredentials = new SigningCredentials(_options.SecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtHeader = new JwtHeader(signingCredentials);
            var jwtPayload = new JwtPayload(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                issuedAt: now);

            var jwt = new JwtSecurityToken(jwtHeader, jwtPayload);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            _logger?.LogDebug($"Jwt token generated successful.");

            var response = new AccessToken
            {
                Scheme = _options.Scheme,
                Token = encodedJwt,
                ExpiresIn = (int)_options.Expiration.TotalSeconds
            };

            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, Formatting.Indented,
                new JsonSerializerSettings
                {
                    ContractResolver =
                        new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
                }));
        }
    }
}
