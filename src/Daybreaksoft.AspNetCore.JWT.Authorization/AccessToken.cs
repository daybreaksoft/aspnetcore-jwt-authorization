namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public class AccessToken
    {
        public string Scheme { get; set; }

        public string Token { get; set; }

        public int ExpiresIn { get; set; }
    }
}
