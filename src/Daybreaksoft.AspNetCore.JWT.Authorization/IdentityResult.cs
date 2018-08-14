using System;
using System.Security.Claims;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public class IdentityResult
    {
        public IdentityResult(ClaimsIdentity identity, TimeSpan? expiration = null)
        {
            this.Identity = identity;
            if (expiration.HasValue) this.Expiration = expiration.Value;
        }

        public IdentityResult(string errroMessage)
        {
            this.ErrorMessage = errroMessage;
        }

        /// <summary>
        /// Claim identity 
        /// </summary>
        public ClaimsIdentity Identity { get; }

        /// <summary>
        /// Indicate when does the token expiry. Default value is 60 mins.
        /// </summary>
        public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(60);

        /// <summary>
        /// Error message if verify failed.
        /// </summary>
        public string ErrorMessage { get; }
    }
}
