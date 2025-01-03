using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Book.Models
{
    public class OrderPayment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string? OrderID { get; set; } // Foreign Key

        [Required]
        [MaxLength(50)]
        public string PaymentType { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentDate { get; set; }

        // Navigation Property
        [ForeignKey("OrderID")]
        public Orders Orders { get; set; }
    }
}
