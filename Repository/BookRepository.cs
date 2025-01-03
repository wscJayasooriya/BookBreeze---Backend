using E_Book.Data;
using E_Book.Models;
using Microsoft.EntityFrameworkCore;

namespace E_Book.Repository
{


    public class BookRepository(ApplicationDbContext context)
    {

        internal List<Book> GetAllBooks()
        {
            return context.Books
                .Include(b => b.Stock)
                .OrderByDescending(b => b.CreatedDate)
                .ToList();
        }

        internal List<Book> GetRecentlyAddedBooks()
        {
            return context.Books
                .Include(b => b.Stock)
                .OrderByDescending(b => b.CreatedDate)
                .Take(5)
                .ToList();
        }
        public void SaveBook(Book book)
        {
            context.Add(book);
            context.SaveChanges();
        }

        public void UpdateBook(Book book)
        {
            context.Update(book);
            context.SaveChanges();
        }

        public void DeleteBook(string isBn)
        {
            var book = FindByIsbn(isBn);
            if (book == null) throw new KeyNotFoundException("Book not found.");
            context.Books.Remove(book);
            context.SaveChanges();
        }

        public async Task UpdateBookStatus(Guid bookId, string status)
        {
            var book = await context.Books.FindAsync(bookId);
            if (book != null)
            {
                book.Status = status;
                context.Books.Update(book);
                await context.SaveChangesAsync();
            }
        }

        internal Book? FindByIsbn(string iSbn) => context.Books.FirstOrDefault(b => b != null && b.ISBN.Equals(iSbn));

        internal Book? FindByBookId(Guid bookId) => context.Books.FirstOrDefault(b => b != null && b.Id.Equals(bookId));

        public async Task<Book?> GetBookByIdAsync(Guid bookId)
        {
            return await context.Books
                .Include(b => b.Stock)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public async Task<int> GetBooksCountAsync()
        {
            return await context.Books.CountAsync();
        }

    }
}
