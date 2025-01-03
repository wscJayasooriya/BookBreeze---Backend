using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace E_Book.Models
{
    public class OrderItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [JsonIgnore]
        [MaxLength(50)]
        public string? OrderID { get; set; } // Foreign Key

        [Required]
        public Guid BookId { get; set; }

        [Required]
        [MaxLength(150)]
        public string BookName { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int Qty { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Navigation Property
        [ForeignKey("OrderID")]
        public Orders Orders { get; set; }
        [MaxLength(1)]
        public int IsFeedback { get; set; }
    }
}
