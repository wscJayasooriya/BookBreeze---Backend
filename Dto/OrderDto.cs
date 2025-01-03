namespace E_Book.Dto
{
    public class OrderDto
    {
        public int Id { get; set; }
        public string OrderID { get; set; }
        public string OrderDateTime { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalQty { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerContact { get; set; }
        public int UserId { get; set; }
        public int Status { get; set; }
        public string OrderStatus { get; set; }
        public string? ApproveDateTime { get; set; }
        public string? DeliveredDateTime { get; set; }
        public string? CancelDateTime { get; set; }

        // Navigation properties for related data
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public List<OrderPaymentDto> OrderPayments { get; set; } = new List<OrderPaymentDto>();
    }
}
