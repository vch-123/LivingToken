using System.Collections.Concurrent;
using WebApplication1.Controllers;
using WebApplication1.Dto;
using WebApplication1.Entity;
using WebApplication1.Helper;

namespace WebApplication1.Service;

public class UserService
{
    private readonly DatabaseContext _dbContext;
    

    private readonly VerificationCodeService _verificationCodeService;
    private readonly EmailService _emailService;
    public UserService(DatabaseContext dbContext, EmailService emailService,VerificationCodeService verificationCodeService)
    {
        _dbContext = dbContext;
        _emailService = emailService;
        _verificationCodeService = verificationCodeService;
    }
    public bool CheckUserNameIsExisted(string userName)
    { 
        //通过any
        return _dbContext.Users.Any(u => u.UserName == userName);
    }
    
    

    

    public bool AddUser(UserDto.UserRegistrationDto userRegistrationDto)
    {
        if (!_verificationCodeService.CheckVerificationCode(userRegistrationDto))
        {
            return false;
        }

        string passwordHash = BCrypt.Net.BCrypt.HashPassword(userRegistrationDto.Password);
        Entity.User newUser = new Entity.User()
        {
            Id = Guid.NewGuid(),
            UserName = userRegistrationDto.UserName,
            PasswordHash = passwordHash,
            Email = userRegistrationDto.Email,
            Level = 0,
            PhoneNumber = "132",
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

    public bool LoginOk(object req)
    {
        return true;
    }
}