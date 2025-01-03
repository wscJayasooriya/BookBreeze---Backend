using E_Book.Dto;
using E_Book.Models;
using E_Book.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class BookController(BookService bookService) : Controller
    {
        [HttpGet("getAll")]
        [AllowAnonymous]
        public IActionResult GetAllBooks()
        {
            return Ok(bookService.GetAllBooks());
        }

        [HttpGet("getAllBooks")]
        [AllowAnonymous]
        public IActionResult GetAllBooksForWeb()
        {
            return Ok(bookService.GetAllBooksForWeb());
        }

        [HttpGet("recentlyAll")]
        [Authorize]
        public IActionResult GetRecentlyAddedBooks()
        {
            return Ok(bookService.GetRecentlyAddedBooks());
        }

        [HttpGet("{isBn}")]
        public IActionResult GetBookByIsBn(string isBn)
        {
            return Ok(bookService.GetBookByIsBn(isBn));
        }

        [HttpGet("getBookById/{bookId}")]
        public async Task<IActionResult> GetBookById(Guid bookId)
        {
            var response = await bookService.GetBookByIdAsync(bookId);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [HttpDelete("{isBn}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(string isBn)
        {
            return Ok(bookService.RemoveBook(isBn));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBook([FromForm] BookDto bookDto)
        {
            if (bookDto == null)
                return BadRequest(new ResponseDto<object>
                {
                    Success = false,
                    Message = "Invalid book data provided."
                });

            var response = await bookService.SaveBook(bookDto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBook([FromBody] BookDto bookDto)
        {
            return Ok(bookService.UpdateBook(bookDto));
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookCount()
        {
            var count = await bookService.GetBooksCountAsync();
            return Ok(new
            {
                Success = true,
                Message = "Book count retrieved successfully.",
                Data = count
            });
        }

    }
}
