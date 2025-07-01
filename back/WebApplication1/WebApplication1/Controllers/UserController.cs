using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExplorerSettings(GroupName = "user")]
public class UserController:ControllerBase
{
    [HttpGet]
    [ActionName("ffffff")]
    [Route("user-by-name/{id}")]
    public List<int> GetSomeNumbers(int id)
    {
        return new List<int>()
        {
            1, 2, 3,4,5
        };
    }
}