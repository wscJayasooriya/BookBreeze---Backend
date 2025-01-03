using E_Book.Dto;
using E_Book.Service;
using E_Book.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Book.Controllers
{
    [ApiController]
    [Route("api/bookBreeze/[controller]")]
    public class OrderController(OrderService orderService, EmailService emailService) : Controller
    {
        [HttpPost("place-order")]
        [Authorize]
        public async Task<IActionResult> PlaceOrder([FromBody] OrderDto orderDto)
        {
            if (orderDto == null || orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            {
                return BadRequest(new { Success = false, Message = "Invalid order data" });
            }

            var response = await orderService.PlaceOrderAsync(orderDto);
            if (!response.Success)
                return BadRequest(response);
            try
            {
                var emailBody = emailService.GenerateOrderEmailBody(orderDto, response.Data);
                await emailService.SendEmailAsync(orderDto.CustomerEmail, "Order Confirmation - BookBreeze", emailBody);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Success = false, Message = "Failed to send email" });
            }

            return Ok(response);
        }


        [HttpGet("GetByStatus")]
        [Authorize]
        public async Task<IActionResult> GetByStatus(string? status, bool filterByStatus = true)
        {
            var response = await orderService.GetOrdersByStatusAsync(status, filterByStatus);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        [HttpGet("GetByOrderID")]
        [Authorize]
        public async Task<IActionResult> GetByOrderID(string orderId)
        {
            var response = await orderService.GetOrderByIdAsync(orderId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }


        [HttpGet("GetByUserId")]
        [Authorize]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var response = await orderService.GetOrdersByUserIdAsync(userId);

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpGet("GetMostRecentOrder")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetMostRecentOrder()
        {
            var response = await orderService.GetMostRecentOrderAsync();

            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }

        [HttpPut("cancelOrder/{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId)
        {
            var response = await orderService.CancelOrderAsync(orderId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("approveOrder/{orderId}")]
        public async Task<IActionResult> ApproveOrder(string orderId)
        {
            var response = await orderService.ApproveOrderAsync(orderId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPut("deliveryOrder/{orderId}")]
        public async Task<IActionResult> DeliveredOrder(string orderId)
        {
            var response = await orderService.DeliveredOrderAsync(orderId);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("count/canceled")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCanceledOrderCount()
        {
            var count = await orderService.GetOrderCountByStatusAsync(-1);
            return Ok(new
            {
                Success = true,
                Message = "Canceled order count retrieved successfully.",
                Data = count
            });
        }

        [HttpGet("count/excluding-canceled")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetOrderCountExcludingCanceled()
        {
            var count = await orderService.GetOrderCountExcludingStatusAsync(-1);
            return Ok(new
            {
                Success = true,
                Message = "Order count excluding canceled orders retrieved successfully.",
                Data = count
            });
        }

        [HttpGet("totalSaleAmount")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalSaleAmount()
        {
            var response = await orderService.GetTotalSaleAmountAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("totalSaleQuantity")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalSaleQuantity()
        {
            var response = await orderService.GetTotalSaleQuantityAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("totalReturnAmount")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalReturnAmount()
        {
            var response = await orderService.GetTotalReturnAmountAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("totalAmount")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalAmount()
        {
            var response = await orderService.GetTotalAmountAsync();

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpGet("getOrderedBooksByUser/{userId}")]
        public async Task<IActionResult> GetOrderedBooksByUser(int userId)
        {
            var response = await orderService.GetOrderedBooksByUserAsync(userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }


        [HttpGet("getOrderCount")]
        public async Task<IActionResult> GetOrderCountByStatus()
        {
            var response = await orderService.GetOrderCountByStatus();
            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);



        }
    }



}
