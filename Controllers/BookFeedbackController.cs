using E_Book.Dto;
using E_Book.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class BookFeedbackController(BookFeedbackService feedbackService) : Controller
    {
        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SaveFeedback([FromBody] BookFeedbackDto feedbackDto)
        {
            if (feedbackDto == null)
                return BadRequest(new ResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid book feedback data provided."
                });

            var response = await feedbackService.SaveFeedback(feedbackDto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
