using E_Book.Models;
using E_Book.Service;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class AuthController : Controller
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public IActionResult Register(UserRegister user)
        {
            _userService.Register(user);
            return Ok(new { message = "User registered successfully" });
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin user)
        {
            var authResponse = _userService.Authenticate(user.Username, user.Password);
            if (authResponse == null) return Unauthorized(new { message = "Invalid credentials" });
            return Ok(authResponse);
        }

    }
}
