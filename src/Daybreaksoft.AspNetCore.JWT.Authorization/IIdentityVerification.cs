using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public interface IIdentityVerification
    {
        /// <summary>
        /// Attempting to get identity via passed values in HttpContext.
        /// </summary>
        /// <param name="context">The Microsoft.AspNetCore.Http.HttpContext for the current request.</param>
        /// <returns>Return a claims-based identity.</returns>
        Task<ClaimsIdentity> GetIdentity(HttpContext context);
    }
}
