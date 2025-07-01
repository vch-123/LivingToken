using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Entity
{
    public enum UserIdentityEnum
    {
        Lu1FanAndFlood=0,
        Admin=1,
        User=2
    }

    public enum GenderEnum
    {
        Male=0,
        Female=1,
        Secrecy=2
    }

    public class User
    {
        public required int Id { get;set; }      
        public required string UserName { get;set; }     
        public required string Salt { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required string Signature { get; set; }
        public string? PhoneNumber { get; set; }
        public UserIdentityEnum UserIdentity { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime RegistrationTime { get; set; } = DateTime.UtcNow;
        public DateTime LastOnlineTime { get;set; } = DateTime.UtcNow;
    }
}
