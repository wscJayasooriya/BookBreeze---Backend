using E_Book.Dto;
using E_Book.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class CustomerController(CustomerService customerService) : Controller
    {
        [HttpGet("getAll")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllCustomers()
        {
            return Ok(customerService.GetAllCustomers());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveCustomer(CustomerDto customerDto)
        {
            var response = await customerService.SaveCustomer(customerDto);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetByEmail")]
        [Authorize]
        public async Task<IActionResult> GetCustomerByEmail(string email)
        {
            var response = await customerService.GetCustomerByEmailAsync(email);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("GetByUserId")]
        [Authorize]
        public async Task<IActionResult> GetCustomerById(int userId)
        {
            var response = await customerService.GetCustomerByIdAsync(userId);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomerCount()
        {
            var count = await customerService.GetCustomerCountAsync();
            return Ok(new
            {
                Success = true,
                Message = "Customer count retrieved successfully.",
                Data = count
            });
        }


    }
}
