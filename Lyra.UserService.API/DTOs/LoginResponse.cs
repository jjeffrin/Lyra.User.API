namespace Lyra.UserService.API.DTOs
{
    public class LoginResponse
    {
        // public string AccessToken { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
