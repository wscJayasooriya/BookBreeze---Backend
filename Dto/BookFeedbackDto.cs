namespace E_Book.Dto
{
    public class BookFeedbackDto
    {
        public Guid BookId { get; set; }
        public string BookName { get; set; }
        public string? Autor { get; set; }
        public int RateCount { get; set; }
        public string Feedback { get; set; }
        public string Order { get; set; }
    }
}
