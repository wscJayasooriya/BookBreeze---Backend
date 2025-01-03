namespace E_Book.Models
{
    public class BookStock
    {
        public int Id { get; set; }
        public Guid BookId { get; set; }
        public int Quantity { get; set; }

        public Book? Book { get; set; }
    }
}
