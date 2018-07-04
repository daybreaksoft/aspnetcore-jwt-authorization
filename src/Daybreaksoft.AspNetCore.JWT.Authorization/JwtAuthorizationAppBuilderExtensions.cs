using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public static class JwtAuthorizationAppBuilderExtensions
    {
        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services, Type implementationType, Action<JwtAuthorizationOptions> configureOptions)
        {
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            // Add services
            services.AddOptions();

            services.Configure(configureOptions);

            services.AddScoped<JwtAuthorizationMiddleware>();

            services.AddScoped(typeof(IIdentityVerification), implementationType);

            return services;
        }

        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services, Action<IServiceCollection> implementationTypeAction, Action<JwtAuthorizationOptions> configureOptions)
        {
            if (implementationTypeAction == null)
            {
                throw new ArgumentNullException(nameof(implementationTypeAction));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            // Add services
            services.AddOptions();

            services.Configure(configureOptions);

            services.AddScoped<JwtAuthorizationMiddleware>();

            implementationTypeAction(services);

            return services;
        }

        public static IApplicationBuilder UseJwtAuthorization(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            
            return app.UseMiddleware<JwtAuthorizationMiddleware>();
        }
    }
}
