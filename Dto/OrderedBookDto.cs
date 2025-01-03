namespace E_Book.Dto
{
    public class OrderedBookDto
    {
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public string OrderDate { get; set; }
        public string Author { get; set; }
        public string? Image { get; set; }
    }
}
