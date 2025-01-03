using System.Diagnostics;
using E_Book.Dto;
using E_Book.Repository;
using E_Book.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class BookStockController(BookStockService _bookStockService) : Controller
    {
        
        [HttpGet("manageStock/{bookId}")]
        [Authorize]
        public async Task<IActionResult> ManageStock(Guid bookId)
        {
            var existingStock = await _bookStockService.GetStockByBookId(bookId);
            if (existingStock == null)
            {
                return NotFound(new { Success = false, Message = "Stock not found for the given book ID." });
            }

            return Ok(existingStock);
        }

        
        [HttpPost("manageStock")]
        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> ManageStock([FromBody] BookStockDto stockDto)
        {
            Debug.WriteLine("User: " + User.Identity.Name); // Log the authenticated username
            Debug.WriteLine("IsAuthenticated: " + User.Identity.IsAuthenticated);

            if (stockDto == null)
            {
                return BadRequest(new { Success = false, Message = "Invalid book stock data provided." });
            }

            var response = await _bookStockService.ManageStock(stockDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

    }
}
