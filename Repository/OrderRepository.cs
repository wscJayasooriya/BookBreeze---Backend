using E_Book.Data;
using E_Book.Dto;
using E_Book.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Book.Repository
{
    public class OrderRepository(ApplicationDbContext _context)
    {
        public async Task<string> GenerateOrderIdAsync()
        {
            // Get the last OrderID
            var lastOrder = await _context.Orders.OrderByDescending(o => o.Id).FirstOrDefaultAsync();

            int lastNumber = 0;
            if (lastOrder != null && lastOrder.OrderID.StartsWith("ORD"))
            {
                lastNumber = int.Parse(lastOrder.OrderID.Substring(3));
            }

            // Generate new OrderID
            return $"ORD{(lastNumber + 1).ToString("D5")}";
        }

        public async Task SaveOrderAsync(Orders order, List<OrderItem> orderItems, List<OrderPayment> orderPayments)
        {
            _context.Orders.Add(order);
            _context.OrderItems.AddRange(orderItems);
            _context.OrderPayments.AddRange(orderPayments);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Orders>> GetOrdersByStatusAsync(string? status, bool filterByStatus)
        {
            if (filterByStatus && !string.IsNullOrEmpty(status))
            {
                return await _context.Orders
                    .Include(order => order.OrderItems)
                    .Include(order => order.OrderPayments)
                    .Where(order => order.OrderStatus == status)
                    .OrderByDescending(order => order.OrderDateTime)
                    .ToListAsync();
            }
            else
            {
                return await _context.Orders
                    .Where(order => order.OrderStatus != "Pending")
                    .Include(order => order.OrderItems)
                    .Include(order => order.OrderPayments)
                    .OrderByDescending(order => order.OrderDateTime)
                    .ToListAsync();
            }
        }


        public async Task<Orders?> GetOrderByIdAsync(string orderId)
        {
            return await _context.Orders
                .Include(order => order.OrderItems) // Include OrderItems
                .Include(order => order.OrderPayments) // Include OrderPayments
                .FirstOrDefaultAsync(order => order.OrderID == orderId);
        }

        public async Task<List<Orders>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.OrderPayments)
                .Where(order => order.UserId == userId)
                .ToListAsync();
        }

        public async Task<Orders?> GetMostRecentOrderAsync()
        {
            return await _context.Orders
                .Include(order => order.OrderItems)
                .Include(order => order.OrderPayments)
                .OrderByDescending(order => order.OrderDateTime)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateOrderAsync(Orders order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetOrderCountByStatusAsync(int status)
        {
            return await _context.Orders.CountAsync(o => o.Status == status);
        }

        public async Task<int> GetOrderCountExcludingStatusAsync(int excludedStatus)
        {
            return await _context.Orders.CountAsync(o => o.Status != excludedStatus);
        }

        public async Task<decimal> GetTotalSaleAmountAsync()
        {
            return await _context.Orders
                .Where(o => o.Status != -1)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetTotalSaleQuantityAsync()
        {
            return await _context.Orders
                .Where(o => o.Status != -1)
                .SumAsync(o => o.TotalQty);
        }

        public async Task<decimal> GetTotalReturnAmountAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == -1)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<decimal> GetTotalAmountAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == 10)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task<List<OrderedBookDto>> GetOrderedBooksByUserAsync(int userId)
        {
            return await (from order in _context.Orders
                          join orderItem in _context.OrderItems
                              on order.OrderID equals orderItem.OrderID
                          join book in _context.Books
                              on orderItem.BookId equals book.Id
                          where order.UserId == userId && order.Status != -1 // Exclude canceled orders
                          group new { orderItem, book } by new { orderItem.BookId, book.Author, book.Image, orderItem.BookName } into grouped
                          select new OrderedBookDto
                          {
                              BookId = grouped.Key.BookId,
                              BookName = grouped.Key.BookName,
                              Quantity = grouped.Sum(g => g.orderItem.Qty),
                              Price = grouped.FirstOrDefault().orderItem.Price,
                              TotalPrice = grouped.Sum(g => g.orderItem.TotalPrice),
                              Author = grouped.Key.Author,
                              Image = grouped.Key.Image
                          }).ToListAsync();
        }

        public async Task<List<OrderStatusCountDto>> GetOrderCountByStatus()
        {
            return await _context.Orders
                .GroupBy(o => o.OrderStatus)
                .Select(group => new OrderStatusCountDto
                {
                    OrderStatus = group.Key,
                    OrderCount = group.Count()
                })
                .ToListAsync();
        }


    }
}
