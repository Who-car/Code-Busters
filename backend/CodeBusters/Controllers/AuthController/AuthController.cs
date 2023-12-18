using CodeBusters.Models;
using MyAspHelper.Abstract;
using MyAspHelper.Attributes;
using MyAspHelper.Attributes.HttpMethods;
using MyAspHelper.AuthSchemas;

namespace CodeBusters.Controllers.AuthController;

public class AuthController : Controller
{
    [HttpGet]
    [Route("api/Auth/token")]
    public async Task<ActionResult> CheckToken()
    {
        var token = ContextResult.AuthToken;

        if (JwtHelper<User>.ValidateToken(token) != Guid.Empty)
            return Ok();
        return Unauthorized("Not a valid token");
    }
}