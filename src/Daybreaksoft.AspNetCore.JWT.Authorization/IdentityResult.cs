using System.Security.Claims;

namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public class IdentityResult
    {
        public IdentityResult(ClaimsIdentity identity)
        {
            this.Identity = identity;
        }

        public IdentityResult(string errroMessage)
        {
            this.ErrorMessage = errroMessage;
        }

        public ClaimsIdentity Identity { get; }

        public string ErrorMessage { get; }
    }
}
