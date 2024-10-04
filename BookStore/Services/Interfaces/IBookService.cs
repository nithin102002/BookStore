using BookStore.Models;

namespace BookStore.Services.Interfaces
{
    public interface IBookService
    {
        Task<bool> CreateOrUpdateBook(BookData book, bool isUpdated);
        Task<List<BookData>> GetAllBooks();
        Task<BookData> GetBookById(Guid bookId);
        Task<bool> UpdateBook(BookData book,Guid bookId);
        Task<bool> DeleteBook(Guid bookId);

    }
}
