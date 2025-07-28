using System.Data.SqlTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Dto;
using WebApplication1.Entity;
using WebApplication1.Service;
using WebApplication1.Entity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using static WebApplication1.Dto.UserDto;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "user")]
public class UserController:ControllerBase
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserService _userService;
    private readonly EmailService _emailService;
    private readonly VerificationCodeService _verificationCodeService;
    public UserController(UserService userService,EmailService emailService,VerificationCodeService verificationCodeService, IOptions<JwtSettings> jwtSettings)
    {
        _userService = userService;
        _emailService = emailService;
        _verificationCodeService = verificationCodeService;
        _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings));
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

    //[HttpPost]
    //[Route("login")]
    //public IActionResult Login(UserLoginDto userLoginDto)
    //{
    //    return new List<int>()
    //    {
    //        1, 2, 3,4,5
    //    };
    //}


    [Authorize]
    [AllowAnonymous]
    [HttpGet("checkUserNameIsExisted/{userName}")]
    public bool CheckUserNameIsExisted(string userName)
    {
        return _userService.CheckUserNameIsExisted(userName);
    }


    private string GenerateJwtToken(string usernameOrEmail)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, usernameOrEmail),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }






    [AllowAnonymous]
    [HttpPost("addUser")]
    public bool AddUser(UserDto.UserRegistrationDto userRegistrationDto)
    {
        return _userService.AddUser(userRegistrationDto);
    }

    [AllowAnonymous]
    [HttpPost("email")]
    public bool AddUser([FromBody] SendCodeRequest req)
    {
        //_emailService.SendEmailAsync("tutumax@qq.com", req.Email, "fiafih", "加我免费领", new List<IFormFile>());
        //return true;
        return true;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult UserLogin([FromBody] UserLoginDto dto)
    {
        if (1==1)
        {
            var token = GenerateJwtToken(dto.UserNameOrEmail);
            return Ok(new { success = true, token });
        }

        return Ok(new { success = false, message = "用户名或密码错误" });

    }

    [HttpPost("send-code")]
    public IActionResult SendCode([FromBody] SendCodeRequest req)
    {
        //现在userService的字典添加
        string code=_verificationCodeService.AddVerificationCodeToDic(req);
        _emailService.SendEmailAsync("tutumax@qq.com", req.Email, "注册验证", $"您的验证码为:{code}", new List<IFormFile>());
        return Ok();
        //var code = _verificationCodeService.GenerateCode(req.Email);
        //// TODO: 这里可以真正发邮件，或仅返回给前端做演示
        //return Ok(new { Code = code });
    }

    // 2. 注册并校验验证码
    //[HttpPost("register")]
    //public IActionResult Register([FromBody] RegisterRequest req)
    //{
    //    if (!_verificationCodeService.ValidateCode(req.Email, req.Code))
    //        return BadRequest("验证码无效或已过期");

    //    // TODO: 真正写库注册用户
    //    return Ok("注册成功");
    //}
    public record SendCodeRequest(string Email,string UserName);
    public record RegisterRequest(string Email, string Code);


}