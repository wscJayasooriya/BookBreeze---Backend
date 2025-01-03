using E_Book.Dto;
using E_Book.Models;
using E_Book.Repository;
using E_Book.Shared;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;

namespace E_Book.Service
{

    public class BookService()
    {
        private readonly string _imageDirectory = @"E:\ESOFT CMPZ\Projects\AD\AD CW 2\AD CW2 Final Source Code\Images";
        private int _bookCounter = 1;
        private readonly FileService _fileService;
        private readonly BookRepository _repository;

        public BookService(FileService fileService, BookRepository repository) : this()
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public ResponseDto<object> GetAllBooks()
        {
            List<BookDto> books = _repository
                .GetAllBooks()
                .Select(ConvertBook)
                .ToList();

            return new ResponseDto<object>
            {
                Data = books,
                Message = "Books retrieved successfully.",
                Success = true
            };
        }
        public ResponseDto<object> GetAllBooksForWeb()
        {
            List<BookDto> books = _repository
                .GetAllBooks()
                .Where(book => book.IsActive != -1)
                .Select(ConvertBook)
                .ToList();

            return new ResponseDto<object>
            {
                Data = books,
                Message = "Books retrieved successfully.",
                Success = true
            };
        }

        public ResponseDto<object> GetRecentlyAddedBooks()
        {
            List<BookDto> books = _repository
                .GetRecentlyAddedBooks()
                .Select(ConvertBook)
                .ToList();

            return new ResponseDto<object>
            {
                Data = books,
                Message = "Books retrieved successfully.",
                Success = true
            };
        }


        public ResponseDto<object> GetBookByIsBn(string isBn)
        {
            var book = _repository.FindByIsbn(isBn);
            if (book == null)
            {
                return new ResponseDto<object>
                {
                    Data = null,
                    Message = "Book not found.",
                    Success = false
                };
            }
            return new ResponseDto<object>
            {
                Data = ConvertBook(book),
                Message = "Book retrieved successfully.",
                Success = true
            };
        }

        public async Task<ResponseDto<BookDetailsDto>> GetBookByIdAsync(Guid bookId)
        {
            var book = await _repository.GetBookByIdAsync(bookId);
            if (book == null)
            {
                return new ResponseDto<BookDetailsDto>
                {
                    Success = false,
                    Message = "Book not found."
                };
            }

            var bookDetails = new BookDetailsDto
            {
                BookId = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                Stock = book.Stock?.Quantity ?? 0,
                Image = book.Image
            };

            return new ResponseDto<BookDetailsDto>
            {
                Success = true,
                Message = "Book retrieved successfully.",
                Data = bookDetails
            };
        }

        public ResponseDto<object> RemoveBook(string isBn)
        {
            var book = _repository.FindByIsbn(isBn);
            if (book == null)
            {
                return new ResponseDto<object>
                {
                    Data = null,
                    Message = "Book not found.",
                    Success = false
                };
            }
            book.UpdatedDate = DateTime.Now;
            book.UpdatedBy = "Admin";
            book.IsActive = -1;
            book.Status = "Inactive";
            _repository.UpdateBook(book);
            return new ResponseDto<object>
            {
                Data = null,
                Message = "Book removed successfully.",
                Success = true
            };
        }


        public async Task<ResponseDto<object>> SaveBook(BookDto bookDto)
        {
            var book = _repository.FindByIsbn(bookDto.ISBN);

            if (book != null)
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = $"Book with ISBN {bookDto.ISBN} already exists."
                };

            try
            {
                if (bookDto.ImageFile != null)
                {
                    if (bookDto.ImageFile.Length > 1 * 1024 * 1024)
                        return new ResponseDto<object>
                        {
                            Success = false,
                            Message = "Image file size cannot exceed 1 MB."
                        };

                    string[] allowedExtensions = { ".jpeg", ".jpg", ".png" };
                    string imageName = await _fileService.SaveFile(bookDto.ImageFile, allowedExtensions);
                    bookDto.Image = imageName;
                    bookDto.Status = "Out";
                }
                _repository.SaveBook(new Book
                {
                    Title = bookDto.Title,
                    Author = bookDto.Author,
                    Genre = bookDto.Genre,
                    PublishedDate = bookDto.PublishedDate,
                    Price = bookDto.Price,
                    IsActive = 1,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now,
                    UpdatedBy = bookDto.UpdatedBy,
                    ISBN = bookDto.ISBN,
                    Image = bookDto.Image,
                    Status = bookDto.Status,
                    Publisher = bookDto.Publisher,
                    Language = bookDto.Language

                });

                return new ResponseDto<object>
                {
                    Success = true,
                    Message = "Book saved successfully."
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


        public async Task<ResponseDto<object>> UpdateBook(BookDto bookDto)
        {
            var book = _repository.FindByIsbn(bookDto.ISBN);
            if (book == null)
                return new ResponseDto<object>
                {
                    Success = false,
                    Message = "Book not found."
                };
            try
            {
                string oldImage = "";
                if (bookDto.ImageFile != null)
                {
                    if (bookDto.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not exceed 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await _fileService.SaveFile(bookDto.ImageFile, allowedExtensions);
                    // hold the old image name. Because we will delete this image after updating the new
                    oldImage = bookDto.Image;
                    bookDto.Image = imageName;
                }
                book.Title = bookDto.Title;
                book.Author = bookDto.Author;
                book.Genre = bookDto.Genre;
                book.Price = bookDto.Price;
                book.PublishedDate = bookDto.PublishedDate;
                book.ISBN = bookDto.ISBN;
                book.Image = bookDto.Image;
                book.Status = bookDto.Status;
                book.Publisher = bookDto.Publisher;
                book.Language = bookDto.Language;
                book.UpdatedDate = DateTime.Now;
                book.UpdatedBy = "Admin";
                _repository.UpdateBook(book);
                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    _fileService.DeleteFile(oldImage);
                }
                return new ResponseDto<object>
                {
                    Success = true,
                    Message = "Book updated successfully."
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

        private string ConvertToBase64(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return null;

            string fullPath = Path.Combine(_imageDirectory, imageName);
            if (!File.Exists(fullPath)) return null;

            byte[] imageBytes = File.ReadAllBytes(fullPath);
            return $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
        }

        private BookDto ConvertBook(Book? book)
        {
            if (book == null)
                return new BookDto();
            //string imageUrl = string.IsNullOrEmpty(book.Image) ? null : GetImageUrl(book.Image);
            string imageBase64 = string.IsNullOrEmpty(book.Image) ? null : ConvertToBase64(book.Image);
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Genre = book.Genre,
                PublishedDate = book.PublishedDate,
                Stock = book.Stock?.Quantity ?? 0,
                Price = book.Price,
                IsActive = book.IsActive,
                CreatedDate = book.CreatedDate,
                UpdatedDate = book.UpdatedDate,
                UpdatedBy = book.UpdatedBy,
                ISBN = book.ISBN,
                Image = imageBase64,
                Status = book.Status,
                Publisher = book.Publisher,
                Language = book.Language
            };
        }

        public async Task<int> GetBooksCountAsync()
        {
            return await _repository.GetBooksCountAsync();
        }

    }
}
