namespace E_Book.Dto
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public string OrderID { get; set; }
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal TotalPrice { get; set; }

        public string? Image { get; set; }
        public int IsFeedback { get; set; }
    }
}
