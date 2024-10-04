using BookStore.Models;
using BookStore.Services;
using InfluxDB.Client.Writes;

namespace BookStore.Utilities
{
    public static class BookPointerBuilder
    {
        public static PointData Build(BookData book, bool isUpdate)
        {
            try
            {
                var bookID = Guid.NewGuid().ToString();
                var createAt = "";

                if (isUpdate)
                {
                    bookID = book.BookId.ToString();
                    createAt = book.CreatedAt.ToString("o");
                }
                else
                {
                    bookID = Guid.NewGuid().ToString();
                    createAt = DateTime.UtcNow.ToString("o");
                }
                var updatedAt = DateTime.UtcNow.ToString("o");

                var point = PointData.Measurement("Books")
                             .Tag("BookId", bookID)
                             .Field("Title", book.Title)
                             .Field("Author", book.Author)
                             .Field("CreatedAt", createAt)
                             .Field("UpdatedAt", updatedAt);
                return point;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }
    }
}
