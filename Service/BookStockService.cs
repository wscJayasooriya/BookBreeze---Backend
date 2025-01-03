using E_Book.Dto;
using E_Book.Repository;

namespace E_Book.Service
{
    public class BookStockService(BookStockRepository repository, BookRepository bookRepository)
    {
        public async Task<BookStockDto?> GetStockByBookId(Guid bookId)
        {
            var stock = await repository.GetStockByBookId(bookId);
            if (stock == null) return null;

            return new BookStockDto
            {
                BookId = stock.BookId,
                Quantity = stock.Quantity
            };
        }

        public async Task<ResponseDto<object>> ManageStock(BookStockDto stockDto)
        {
            try
            {
                await repository.ManageStock(stockDto);
                await bookRepository.UpdateBookStatus(stockDto.BookId, "In");
                return new ResponseDto<object>
                {
                    Success = true,
                    Message = "Stock updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred while updating stock: {ex.Message}"
                };
            }
        }

        public async Task<IEnumerable<BookStockDisplayDto>> GetStocks(string searchTerm)
        {
            return await repository.GetStocks(searchTerm);
        }
    }
}
