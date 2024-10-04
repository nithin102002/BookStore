using BookStore.Models;
using BookStore.Services;
using InfluxDB.Client.Core.Flux.Domain;
using Microsoft.AspNetCore.Http;

namespace BookStore.Utilities
{
    public static class BookParse
    {
        public static List<BookData> ParseBooks(List<FluxTable> fluxTables)
        {
            try
            {
                var bookDictionary = new Dictionary<String, BookData>();
                foreach (var table in fluxTables)
                {
                    foreach (var record in table.Records)
                    {
                        var idValue = record.GetValueByKey("BookId")?.ToString();
                        if (!Guid.TryParse(idValue, out Guid id))
                            continue;
                        if (!bookDictionary.TryGetValue(idValue, out var book))
                        {
                            book = new BookData
                            {
                                BookId = id,
                            };
                            bookDictionary[idValue] = book;
                        }
                        SetBookField(book, record);
                    }
                }
                return bookDictionary.Values.ToList();
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }

        private static void SetBookField(BookData book, FluxRecord record)
        {
            try
            {
                var value = record.GetValue()?.ToString() ?? string.Empty;
                switch (record.GetField())
                {
                    case "Title":
                        book.Title = value;
                        break;

                    case "Author":
                        book.Author = value;
                        break;

                    case "CreatedAt":
                        book.CreatedAt = ParseDate(value);
                        break;

                    case "UpdatedAt":
                        book.UpdatedAt = ParseDate(value);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }

        private static DateTime ParseDate(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return DateTime.MinValue;

                if (DateTime.TryParse(value, out DateTime parsedDate))
                    return parsedDate;
                Console.WriteLine($"Failed to parse date:{value}");
                return DateTime.MinValue;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message );
            }

        }

        public static BookData ParseBook(List<FluxTable> fluxTable, Guid bookId)
        {
            try
            {
                var recordById = fluxTable
                    .SelectMany(table => table.Records)
                    .Where(record => record.GetValueByKey("BookId") != null)
                    .GroupBy(record => record.GetValueByKey("BookId")?.ToString())
                    .ToDictionary(group => group.Key, group => group.ToList());

                if (recordById.TryGetValue(bookId.ToString(), out var records))
                {
                    var book = new BookData
                    {
                        BookId = bookId,
                    };
                    PopulateBookFields(book, records);
                    return book;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }

        private static void PopulateBookFields(BookData book, List<FluxRecord> records)
        {
            try
            {
                foreach (var record in records)
                {
                    SetBookField(book, record);
                }
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }
    }
}
