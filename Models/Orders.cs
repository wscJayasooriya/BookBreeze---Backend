using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Book.Models
{
    public class Orders
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string? OrderID { get; set; }

        [Required]
        public string OrderDateTime { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public int TotalQty { get; set; }

        [Required]
        [EmailAddress]
        public string CustomerEmail { get; set; }
        [Required]
        public string CustomerName { get; set; }
        [Required]
        public string CustomerAddress { get; set; }
        [Required]
        public string CustomerContact { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [MaxLength(50)]
        public string OrderStatus { get; set; }

        public string? ApproveDateTime { get; set; }
        public string? DeliveredDateTime { get; set; }
        public string? CancelDateTime { get; set; }

        // Navigation Property
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<OrderPayment> OrderPayments { get; set; }
    }
}
