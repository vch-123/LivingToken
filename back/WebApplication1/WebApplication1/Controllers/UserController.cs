using System.Data.SqlTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Entity;
using WebApplication1.Service;
using WebApplication1.Entity;
using Microsoft.AspNetCore.Identity.Data;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "user")]
public class UserController:ControllerBase
{
    private readonly UserService _userService;
    private readonly EmailService _emailService;
    private readonly VerificationCodeService _verificationCodeService;
    public UserController(UserService userService,EmailService emailService,VerificationCodeService verificationCodeService)
    {
        _userService = userService;
        _emailService = emailService;
        _verificationCodeService = verificationCodeService;
    }
    [HttpGet]
    [Route("user-by-name/{id}")]
    public List<int> GetSomeNumbers(int id)
    {
        return new List<int>()
        {
            1, 2, 3,4,5
        };
    }

    [AllowAnonymous]
    [HttpGet("checkUserNameIsExisted/{userName}")]
    public bool CheckUserNameIsExisted(string userName)
    {
        return _userService.CheckUserNameIsExisted(userName);
    }
    
    [AllowAnonymous]
    [HttpPost("addUser")]
    public bool AddUser(UserDto.UserRegistrationDto userRegistrationDto)
    {
        return _userService.AddUser(userRegistrationDto);
    }

    [AllowAnonymous]
    [HttpPost("email")]
    public bool AddUser(string message)
    {
        _emailService.SendEmailAsync("tutumax@qq.com", "1132528903@qq.com", "fiafih", "加我免费领", new List<IFormFile>());
        return true;
    }

    [HttpPost("send-code")]
    public IActionResult SendCode([FromBody] SendCodeRequest req)
    {
        var code = _verificationCodeService.GenerateCode(req.Email);
        // TODO: 这里可以真正发邮件，或仅返回给前端做演示
        return Ok(new { Code = code });
    }

    // 2. 注册并校验验证码
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        if (!_verificationCodeService.ValidateCode(req.Email, req.Code))
            return BadRequest("验证码无效或已过期");

        // TODO: 真正写库注册用户
        return Ok("注册成功");
    }
    public record SendCodeRequest(string Email);
    public record RegisterRequest(string Email, string Code);


}