using BookStore.Models;
using BookStore.Services.Interfaces;
using BookStore.Utilities;
using InfluxDB.Client;
using Microsoft.Extensions.Options;
namespace BookStore.Services
{
    public class BookService : IBookService
    {
        private readonly InfluxDBClient _influxDBClients;
        private readonly string _bucket;
        private readonly string _organization;
        private readonly ILogger<BookService> _logger;

        public BookService(IOptions<InfluxDBSettings> influxDBSettings,ILogger<BookService> logger) 
        {
            var settings=influxDBSettings.Value;
            _influxDBClients= new InfluxDBClient(settings.Url,settings.Token);
            _bucket= settings.Bucket;
            _organization = settings.Organization;
            _logger = logger;
        }
        // Creates or Update Book
        public async Task<bool> CreateOrUpdateBook(BookData book, bool isUpdated)
        {
            try
            {
                var point = BookPointerBuilder.Build(book, isUpdated);
                var writeApi = _influxDBClients.GetWriteApiAsync();
                await writeApi.WritePointAsync(point, _bucket, _organization);
                return true;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }

            
        }




        //Get all Books
        public async Task<List<BookData>> GetAllBooks()
        {
            try
            {
                var fluxQuery = $"from(bucket:\"{_bucket}\") |> range(start: -30d,stop: now()) |> filter(fn:(r)=> r._measurement==\"Books\")";
                var fluxTable = await _influxDBClients.GetQueryApi().QueryAsync(fluxQuery, _organization);
                var books = BookParse.ParseBooks(fluxTable);
                return books;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
         }

        

        // Get Book By Id
       public async Task<BookData> GetBookById(Guid bookId)
        {
            try
            {
                var fluxQuery = $"from(bucket: \"{_bucket}\") |> range(start: -30d, stop: now()) |> filter(fn: (r) => r[\"_measurement\"] == \"Books\") |> filter(fn: (r) => r[\"BookId\"] == \"{bookId}\")";
                var fluxTable = await _influxDBClients.GetQueryApi().QueryAsync(fluxQuery, _organization);
                var book = BookParse.ParseBook(fluxTable, bookId);
                if (book == null)
                {
                    return null;
                }
                return book;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }

        

        //Update a Book
        public async Task<bool> UpdateBook(BookData book, Guid bookId)
        {
            try
            {
                var existingBook = await DeleteBook(bookId);
                if (existingBook == null)
                {
                    throw new Exception("An unexpected error occured while updating the site.");

                }
                var result = await CreateOrUpdateBook(book, true);
                if (result == true)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new BookException(ex.Message);
            }
        }
        // Delete a Book
        public async Task<bool> DeleteBook(Guid bookId)
        {
            try
            {
                var book = await GetBookById(bookId);
                if (book == null)
                {
                    throw new Exception("Book not found");
                }

                var deleteApi = _influxDBClients.GetDeleteApi();
                var predicate = $"_measurement=\"Books\" AND BookId=\"{bookId}\"";
                var start = DateTime.UtcNow.AddDays(-30);
                var stop = DateTime.Now;

                await deleteApi.Delete(start, stop, predicate, _bucket, _organization);
                return true;

            }
            catch (Exception e)
            {
                throw new BookException(e.Message);
            }
        }
    }
}
