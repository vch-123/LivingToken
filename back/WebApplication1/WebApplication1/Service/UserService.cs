using WebApplication1.Dto;
using WebApplication1.Entity;

namespace WebApplication1.Service;

public class UserService
{
    private readonly DatabaseContext _dbContext;
    public UserService(DatabaseContext dbContext)
    {
        _dbContext = dbContext;
    }
    public bool CheckUserNameIsExisted(string userName)
    {
        return true;
        //通过any
        return _dbContext.Users.Any(u => u.UserName == userName);
    }

    public bool AddUser(UserDto.UserRegistrationDto userRegistrationDto)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);
        Entity.User newUser = new Entity.User()
        {
            Id = Guid.NewGuid(),
            UserName = userRegistrationDto.UserName,
            PasswordHash = passwordHash,
            Email = userRegistrationDto.Email,
            Level = 0,
            PhoneNumber = userRegistrationDto.PhoneNumber,
            UserIdentity = UserIdentityEnum.User,
            Gender = userRegistrationDto.Gender,
            RegistrationTime = DateTime.Now,
            LastOnlineTime = DateTime.Now,
            Signature = "425",
            
        };
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
        return true;
    }
}