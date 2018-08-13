using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Daybreaksoft.AspNetCore.JWT.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AspNetCore.Sample.Services
{
    public class IdentityVerification : IIdentityVerification
    {
        public async Task<IdentityResult> GetIdentity(HttpContext context)
        {
            if (context.Request.Form["username"].ToString() == "test" && context.Request.Form["password"].ToString() == "test")
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new Claim(ClaimTypes.Name, "test"));
                identity.AddClaim(new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(new { Custom1 = 1, Custom2 = 2 })));

                return await Task.FromResult(new IdentityResult(identity));
            }
            else
            {
                return new IdentityResult("Invalid user or password.");
            }
        }
    }
}
