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

    [HttpPost]
    [Route("login")]
    public IActionResult Login(UserLoginDto userLoginDto)
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
    public bool AddUser([FromBody] SendCodeRequest req)
    {
        //_emailService.SendEmailAsync("tutumax@qq.com", req.Email, "fiafih", "���������", new List<IFormFile>());
        //return true;
        return true;
    }

    [HttpPost("send-code")]
    public IActionResult SendCode([FromBody] SendCodeRequest req)
    {
        _emailService.SendEmailAsync("tutumax@qq.com", req.Email, "fiafih", "���������", new List<IFormFile>());
        return Ok();
        //var code = _verificationCodeService.GenerateCode(req.Email);
        //// TODO: ��������������ʼ���������ظ�ǰ������ʾ
        //return Ok(new { Code = code });
    }

    // 2. ע�ᲢУ����֤��
    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        if (!_verificationCodeService.ValidateCode(req.Email, req.Code))
            return BadRequest("��֤����Ч���ѹ���");

        // TODO: ����д��ע���û�
        return Ok("ע��ɹ�");
    }
    public record SendCodeRequest(string Email);
    public record RegisterRequest(string Email, string Code);


}