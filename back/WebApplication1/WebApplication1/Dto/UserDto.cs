using WebApplication1.Entity;

namespace WebApplication1.Dto;

public class UserDto
{
    public class UserRegistrationDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public GenderEnum Gender { get; set; }
        public string VerificationCode { get; set; }
    }

    public class UserLoginDto
    {
        public string UserNameOrEmail { get; set; }
        public string Password { get; set; }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = "ThisIsASecretKey1234567890"; // ø…–¥»Î appsettings.json
        public string Issuer { get; set; } = "MyApp";
        public string Audience { get; set; } = "MyAppUser";
        public int ExpireMinutes { get; set; } = 60;
    }
}