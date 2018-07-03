using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Sid.Jwt.Token.Authorization.Server
{
    public static class JwtTokenAuthorizationServerAppBuilderExtensions
    {
        public static IServiceCollection AddJwtTokenAuthorizationServer(this IServiceCollection services, Type userFinderImplementation, Action<JwtAuthorizationServerOptions> configureOptions)
        {
            if (userFinderImplementation == null)
            {
                throw new ArgumentNullException(nameof(userFinderImplementation));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            // Register services
            services.AddOptions();

            services.Configure(configureOptions);

            services.AddScoped<JwtAuthorizationServerMiddleware>();

            services.AddScoped(typeof(IUserFinder), userFinderImplementation);

            return services;
        }

        public static IApplicationBuilder UseJwtAuthorizationServer(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            return app.UseMiddleware<JwtAuthorizationServerMiddleware>();
        }
    }
}
