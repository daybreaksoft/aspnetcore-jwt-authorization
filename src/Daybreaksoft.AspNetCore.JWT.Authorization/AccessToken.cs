namespace Daybreaksoft.AspNetCore.JWT.Authorization
{
    public class AccessToken
    {
        public string scheme { get; set; }

        public string access_token { get; set; }

        public int expires_in { get; set; }
    }
}
