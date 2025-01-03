using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Book.Models
{
    public class BookFeedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public string? Autor { get; set; }
        public int RateCount { get; set; }
        public string Feedback { get; set; }
        public string Order { get; set; }
    }
}
