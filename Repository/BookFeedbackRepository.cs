using E_Book.Data;
using E_Book.Models;

namespace E_Book.Repository
{
    public class BookFeedbackRepository(ApplicationDbContext context)
    {
        internal BookFeedback? FindByFeedbackDetails(Guid feedbackDtoBookId, string feedbackDtoOrder)
        {
            return context.BookFeedbacks.FirstOrDefault(b =>
                b.BookId == feedbackDtoBookId &&
                b.Order.ToLower() == feedbackDtoOrder.ToLower());
        }



        public void SaveFeedback(BookFeedback bookFeedback)
        {
            context.Add(bookFeedback);
            context.SaveChanges();
        }
    }
}
