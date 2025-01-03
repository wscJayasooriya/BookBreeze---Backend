using System.ComponentModel.DataAnnotations;

namespace E_Book.Dto
{
    public class BookStockDto
    {
        public Guid BookId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be a non-negative value.")]
        public int Quantity { get; set; }
    }

    public class BookStockDisplayDto
    {
        public Guid BookId { get; set; }
        public int Quantity { get; set; }
        public string? BookName { get; set; }
    }
}
