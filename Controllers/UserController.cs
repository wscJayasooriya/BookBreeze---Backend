using E_Book.Dto;
using E_Book.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class UserController(IUserService userService) : Controller
    {
        [HttpGet("getAllUsers")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllUsers()
        {
            return Ok(userService.GetAllUsers());
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveUSer([FromBody] UserDto userDto)
        {
            if (userDto == null)
                return BadRequest(new ResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid user data provided."
                });

            var response = await userService.SaveUser(userDto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

    }
}
