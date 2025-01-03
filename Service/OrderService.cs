using E_Book.Dto;
using E_Book.Models;
using E_Book.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace E_Book.Service
{
    public class OrderService(OrderRepository _orderRepository, BookStockRepository bookStockRepository, BookRepository bookRepository)
    {
        private readonly string _imageDirectory = @"E:\ESOFT CMPZ\Projects\AD\AD CW 2\AD CW2 Final Source Code\Images";
        public async Task<ResponseDto<object>> PlaceOrderAsync(OrderDto orderDto)
        {
            try
            {
                // Generate new OrderID
                string orderId = await _orderRepository.GenerateOrderIdAsync();

                // Create the order entity
                var order = new Orders
                {
                    OrderID = orderId,
                    OrderDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    TotalAmount = orderDto.TotalAmount,
                    TotalQty = orderDto.OrderItems.Sum(i => i.Qty),
                    CustomerEmail = orderDto.CustomerEmail,
                    CustomerName = orderDto.CustomerName,
                    CustomerAddress = orderDto.CustomerAddress,
                    CustomerContact = orderDto.CustomerContact,
                    UserId = orderDto.UserId,
                    Status = 0, // Default: Pending
                    OrderStatus = "Pending"
                };

                // Map items
                var orderItems = orderDto.OrderItems.Select(item => new OrderItem
                {
                    OrderID = orderId,
                    BookId = item.BookId,
                    BookName = item.BookName,
                    Price = item.Price,
                    Qty = item.Qty,
                    TotalPrice = item.TotalPrice,
                    IsFeedback = 0
                }).ToList();

                // Map payment
                var orderPayments = orderDto.OrderPayments.Select(payment => new OrderPayment
                {
                    OrderID = orderId,
                    PaymentType = payment.PaymentType,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                }).ToList();


                // Save to repository
                await _orderRepository.SaveOrderAsync(order, orderItems, orderPayments);
                await bookStockRepository.UpdateStockAsync(orderDto.OrderItems);

                return new ResponseDto<object>
                {
                    Success = true,
                    Message = "Order placed successfully.",
                    Data = new { OrderID = orderId }
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred while placing the order: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDto<List<OrderDto>>> GetOrdersByStatusAsync(string? status, bool filterByStatus)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status, filterByStatus);

            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                OrderID = order.OrderID,
                OrderDateTime = order.OrderDateTime,
                TotalAmount = order.TotalAmount,
                TotalQty = order.TotalQty,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                CustomerContact = order.CustomerContact,
                UserId = order.UserId,
                Status = order.Status,
                OrderStatus = order.OrderStatus,
                ApproveDateTime = order.ApproveDateTime,
                DeliveredDateTime = order.DeliveredDateTime,
                CancelDateTime = order.CancelDateTime,
                OrderItems = order.OrderItems?.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    OrderID = item.OrderID,
                    BookId = item.BookId,
                    BookName = item.BookName,
                    Price = item.Price,
                    Qty = item.Qty,
                    TotalPrice = item.TotalPrice
                }).ToList(),
                OrderPayments = order.OrderPayments?.Select(payment => new OrderPaymentDto
                {
                    Id = payment.Id,
                    OrderID = payment.OrderID,
                    PaymentType = payment.PaymentType,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                }).ToList()
            }).ToList();

            return new ResponseDto<List<OrderDto>>
            {
                Success = true,
                Message = "Orders retrieved successfully.",
                Data = orderDtos
            };
        }


        public async Task<ResponseDto<OrderDto>> GetOrderByIdAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return new ResponseDto<OrderDto>
                {
                    Success = false,
                    Message = $"Order with ID {orderId} not found.",
                    Data = null
                };
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderID = order.OrderID,
                OrderDateTime = order.OrderDateTime,
                TotalAmount = order.TotalAmount,
                TotalQty = order.TotalQty,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                CustomerContact = order.CustomerContact,
                UserId = order.UserId,
                Status = order.Status,
                OrderStatus = order.OrderStatus,
                ApproveDateTime = order.ApproveDateTime,
                DeliveredDateTime = order.DeliveredDateTime,
                CancelDateTime = order.CancelDateTime,
                OrderItems = order.OrderItems.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    OrderID = item.OrderID,
                    BookId = item.BookId,
                    BookName = item.BookName,
                    Price = item.Price,
                    Qty = item.Qty,
                    TotalPrice = item.TotalPrice,
                    Image = getImage(item.BookId)
                }).ToList(),
                OrderPayments = order.OrderPayments.Select(payment => new OrderPaymentDto
                {
                    Id = payment.Id,
                    OrderID = payment.OrderID,
                    PaymentType = payment.PaymentType,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                }).ToList()
            };

            return new ResponseDto<OrderDto>
            {
                Success = true,
                Message = "Order retrieved successfully.",
                Data = orderDto
            };
        }

        private string? getImage(Guid itemBookId)
        {
            var book = bookRepository.FindByBookId(itemBookId);
            string imageBase64 = string.IsNullOrEmpty(book.Image) ? null : ConvertToBase64(book.Image);
            return imageBase64;
        }

        public async Task<ResponseDto<List<OrderDto>>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            var orderDtos = orders.Select(order => new OrderDto
            {
                Id = order.Id,
                OrderID = order.OrderID,
                OrderDateTime = order.OrderDateTime,
                TotalAmount = order.TotalAmount,
                TotalQty = order.TotalQty,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                CustomerContact = order.CustomerContact,
                UserId = order.UserId,
                Status = order.Status,
                OrderStatus = order.OrderStatus,
                ApproveDateTime = order.ApproveDateTime,
                DeliveredDateTime = order.DeliveredDateTime,
                CancelDateTime = order.CancelDateTime,
                OrderItems = order.OrderItems?.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    OrderID = item.OrderID,
                    BookId = item.BookId,
                    BookName = item.BookName,
                    Price = item.Price,
                    Qty = item.Qty,
                    TotalPrice = item.TotalPrice
                }).ToList(),
                OrderPayments = order.OrderPayments?.Select(payment => new OrderPaymentDto
                {
                    Id = payment.Id,
                    OrderID = payment.OrderID,
                    PaymentType = payment.PaymentType,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                }).ToList()
            }).ToList();

            return new ResponseDto<List<OrderDto>>
            {
                Success = true,
                Message = "Orders retrieved successfully.",
                Data = orderDtos
            };
        }

        public async Task<ResponseDto<OrderDto>> GetMostRecentOrderAsync()
        {
            var order = await _orderRepository.GetMostRecentOrderAsync();

            if (order == null)
            {
                return new ResponseDto<OrderDto>
                {
                    Success = false,
                    Message = "No orders found."
                };
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderID = order.OrderID,
                OrderDateTime = order.OrderDateTime,
                TotalAmount = order.TotalAmount,
                TotalQty = order.TotalQty,
                CustomerEmail = order.CustomerEmail,
                CustomerName = order.CustomerName,
                CustomerAddress = order.CustomerAddress,
                CustomerContact = order.CustomerContact,
                UserId = order.UserId,
                Status = order.Status,
                OrderStatus = order.OrderStatus,
                ApproveDateTime = order.ApproveDateTime,
                DeliveredDateTime = order.DeliveredDateTime,
                CancelDateTime = order.CancelDateTime,
                OrderItems = order.OrderItems?.Select(item => new OrderItemDto
                {
                    Id = item.Id,
                    OrderID = item.OrderID,
                    BookId = item.BookId,
                    BookName = item.BookName,
                    Price = item.Price,
                    Qty = item.Qty,
                    TotalPrice = item.TotalPrice
                }).ToList(),
                OrderPayments = order.OrderPayments?.Select(payment => new OrderPaymentDto
                {
                    Id = payment.Id,
                    OrderID = payment.OrderID,
                    PaymentType = payment.PaymentType,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate
                }).ToList()
            };

            return new ResponseDto<OrderDto>
            {
                Success = true,
                Message = "Most recent order retrieved successfully.",
                Data = orderDto
            };
        }

        public async Task<ResponseDto<object>> CancelOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return new ResponseDto<object>()
                {
                    Success = false,
                    Message = "Not order found"
                };
            }

            if (order.Status == -1)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Order is already canceled."
                };
            }

            order.Status = -1; // Cancel Status
            order.OrderStatus = "Cancelled";
            order.CancelDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            await _orderRepository.UpdateOrderAsync(order);

            return new ResponseDto<object>
            {
                Success = true,
                Message = "Order canceled successfully."
            };
        }

        public async Task<ResponseDto<object>> ApproveOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Order not found."
                };
            }

            if (order.Status == 5)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Order is already approved."
                };
            }

            order.Status = 5; // Approve Status
            order.OrderStatus = "Approved";
            order.ApproveDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            await _orderRepository.UpdateOrderAsync(order);

            return new ResponseDto<object>
            {
                Success = true,
                Message = "Order approve successfully."
            };
        }

        public async Task<ResponseDto<object>> DeliveredOrderAsync(string orderId)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Order not found."
                };
            }

            if (order.Status == 10)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Order is already delivered."
                };
            }

            order.Status = 10; // Delivery Status
            order.OrderStatus = "Delivered";
            order.DeliveredDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            await _orderRepository.UpdateOrderAsync(order);

            return new ResponseDto<object>
            {
                Success = true,
                Message = "Order delivery successfully."
            };
        }

        public async Task<int> GetOrderCountByStatusAsync(int status)
        {
            return await _orderRepository.GetOrderCountByStatusAsync(status);
        }

        public async Task<int> GetOrderCountExcludingStatusAsync(int excludedStatus)
        {
            return await _orderRepository.GetOrderCountExcludingStatusAsync(excludedStatus);
        }

        public async Task<ResponseDto<decimal>> GetTotalSaleAmountAsync()
        {
            try
            {
                var totalSaleAmount = await _orderRepository.GetTotalSaleAmountAsync();

                return new ResponseDto<decimal>
                {
                    Success = true,
                    Message = "Total sale amount retrieved successfully.",
                    Data = totalSaleAmount
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<decimal>
                {
                    Success = false,
                    Message = $"An error occurred while calculating the total sale amount: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDto<decimal>> GetTotalReturnAmountAsync()
        {
            try
            {
                var totalReturnAmount = await _orderRepository.GetTotalReturnAmountAsync();

                return new ResponseDto<decimal>
                {
                    Success = true,
                    Message = "Total return amount retrieved successfully.",
                    Data = totalReturnAmount
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<decimal>
                {
                    Success = false,
                    Message = $"An error occurred while calculating the total return amount: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDto<decimal>> GetTotalAmountAsync()
        {
            try
            {
                var totalAmount = await _orderRepository.GetTotalAmountAsync();

                return new ResponseDto<decimal>
                {
                    Success = true,
                    Message = "Total amount retrieved successfully.",
                    Data = totalAmount
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<decimal>
                {
                    Success = false,
                    Message = $"An error occurred while calculating the total amount: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDto<decimal>> GetTotalSaleQuantityAsync()
        {
            try
            {
                var totalSaleQty = await _orderRepository.GetTotalSaleQuantityAsync();

                return new ResponseDto<decimal>
                {
                    Success = true,
                    Message = "Total sale qty retrieved successfully.",
                    Data = totalSaleQty
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<decimal>
                {
                    Success = false,
                    Message = $"An error occurred while calculating the total sale qty: {ex.Message}"
                };
            }
        }

        public async Task<ResponseDto<List<OrderedBookDto>>> GetOrderedBooksByUserAsync(int userId)
        {
            var orderedBooks = await _orderRepository.GetOrderedBooksByUserAsync(userId);
            //string imageBase64 = string.IsNullOrEmpty(orderedBooks.Image) ? null : ConvertToBase64(orderedBooks.Image);
            if (orderedBooks == null || !orderedBooks.Any())
            {
                return new ResponseDto<List<OrderedBookDto>>
                {
                    Success = false,
                    Message = "No orders found for the specified user.",
                    Data = null
                };
            }

            return new ResponseDto<List<OrderedBookDto>>
            {
                Success = true,
                Message = "Ordered books retrieved successfully.",
                Data = orderedBooks
            };
        }

        private string ConvertToBase64(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return null;

            string fullPath = Path.Combine(_imageDirectory, imageName);
            if (!File.Exists(fullPath)) return null;

            byte[] imageBytes = File.ReadAllBytes(fullPath);
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }

        public async Task<ResponseDto<object>> GetOrderCountByStatus()
        {
            // Fetch the data from the repository
            var orderCounts = await _orderRepository.GetOrderCountByStatus();

            // Check if data exists
            if (orderCounts == null || !orderCounts.Any())
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "No orders found for the specified user.",
                    Data = null
                };
            }

            return new ResponseDto<object>
            {
                Success = true,
                Message = " retrieved successfully",
                Data = orderCounts
            };
        }
    }
}
