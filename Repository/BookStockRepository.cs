using E_Book.Data;
using E_Book.Dto;
using E_Book.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Book.Repository
{
    public class BookStockRepository()
    {
        private readonly ApplicationDbContext _context;

        public BookStockRepository(ApplicationDbContext context) : this()
        {
            _context = context;
        }

        public async Task<BookStock?> GetStockByBookId(Guid bookId)
        {
            return await _context.BookStocks.FirstOrDefaultAsync(s => s.BookId == bookId);
        }

        public async Task ManageStock(BookStockDto stockToManage)
        {
            var existingStock = await GetStockByBookId(stockToManage.BookId);

            if (existingStock == null)
            {
                var stock = new BookStock
                {
                    BookId = stockToManage.BookId,
                    Quantity = stockToManage.Quantity
                };
                await _context.BookStocks.AddAsync(stock);
            }
            else
            {
                existingStock.Quantity = stockToManage.Quantity;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<BookStockDisplayDto>> GetStocks(string searchTerm = "")
        {
            return await (from book in _context.Books
                          join stock in _context.BookStocks
                              on book.Id equals stock.BookId into bookStockGroup
                          from bookStock in bookStockGroup.DefaultIfEmpty()
                          where string.IsNullOrWhiteSpace(searchTerm) || book.Title.ToLower().Contains(searchTerm.ToLower())
                          select new BookStockDisplayDto
                          {
                              BookId = book.Id,
                              BookName = book.Title,
                              Quantity = bookStock != null ? bookStock.Quantity : 0
                          }).ToListAsync();
        }

        public async Task<BookStock> GetBookStockAsync(Guid bookId)
        {
            return await _context.BookStocks.FirstOrDefaultAsync(bs => bs.BookId == bookId);
        }

        public async Task UpdateStockAsync(IEnumerable<OrderItemDto> orderItems)
        {
            foreach (var item in orderItems)
            {
                var bookStock = await _context.BookStocks.FirstOrDefaultAsync(bs => bs.BookId == item.BookId);
                if (bookStock != null)
                {
                    bookStock.Quantity -= item.Qty;
                    // If the stock becomes 0, update the book's status to "Out"
                    if (bookStock.Quantity <= 0)
                    {
                        bookStock.Quantity = 0; // Ensure it doesn't go below 0
                        var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == item.BookId);
                        if (book != null)
                        {
                            book.Status = "Out";
                            _context.Books.Update(book);
                        }
                    }
                    _context.BookStocks.Update(bookStock);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
