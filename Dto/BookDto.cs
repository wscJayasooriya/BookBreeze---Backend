namespace E_Book.Dto
{
    public class BookDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int IsActive { get; set; }
        public string Status { get; set; }
        public DateTime PublishedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string ISBN { get; set; }
    }
}
