using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public static class JwtAuthorizationServerAppBuilderExtensions
    {
        public static IServiceCollection AddJwtAuthorizationServer(this IServiceCollection services, Type implementationType, Action<JwtAuthorizationServerOptions> configureOptions)
        {
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            // Register services
            services.AddOptions();

            services.Configure(configureOptions);

            services.AddScoped<JwtAuthorizationServerMiddleware>();

            services.AddScoped(typeof(IIdentityVerification), implementationType);

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
