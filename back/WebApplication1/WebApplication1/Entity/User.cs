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
        public required Guid Id { get;set; }      
        public required string UserName { get;set; }     
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public string Signature { get; set; } //个性签名
        public required int Level { get; set; }
        public string? PhoneNumber { get; set; } //手机号
        public UserIdentityEnum UserIdentity { get; set; }
        public GenderEnum Gender { get; set; }
        public DateTime RegistrationTime { get; set; }
        public DateTime? LastOnlineTime { get;set; } 
    }
}
