using E_Book.Dto;
using E_Book.Models;
using E_Book.Repository;
using E_Book.Shared;

namespace E_Book.Service
{
    public class BookFeedbackService(BookFeedbackRepository repository)
    {
        private readonly BookFeedbackRepository _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        public async Task<ResponseDto<object>> SaveFeedback(BookFeedbackDto feedbackDto)
        {
            var bookFeedback = _repository.FindByFeedbackDetails(feedbackDto.BookId, feedbackDto.Order);

            if (bookFeedback != null)
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Feedback already exists."
                };
            try
            {
                _repository.SaveFeedback(new BookFeedback
                {
                    BookId = feedbackDto.BookId,
                    Order = feedbackDto.Order,
                    RateCount = feedbackDto.RateCount,
                    Feedback = feedbackDto.Feedback,
                    BookName = feedbackDto.BookName,
                    Autor = feedbackDto.Autor
                });
                return new ResponseDto<object>
                {
                    Success = true,
                    Message = "Book Feedback saved successfully."
                };
            }
            catch (Exception ex)
            {
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"An error occurred: {ex.Message}"
                };
            }
        }
    }
}
