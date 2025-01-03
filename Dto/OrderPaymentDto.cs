namespace E_Book.Dto
{
    public class OrderPaymentDto
    {
        public int Id { get; set; }
        public string OrderID { get; set; } // Relates to Order.OrderID
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string PaymentDate { get; set; }
    }
}
