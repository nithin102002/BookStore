using BookStore.Models;
using BookStore.Services;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("bookstore/api/v1/book")]
    public class BookController : Controller
    {
        private readonly IBookService _bookService;

        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBook([FromBody] BookData book)
        {
            try
            {
                if (book == null)
                {
                    return BadRequest(new { Status = 0, message = "Invalid Book Data" });
                }
                var result = await _bookService.CreateOrUpdateBook(book, false);
                if (result)
                {
                    return StatusCode(201, new { Status = 1, message = "Book Created Successfully" });
                }
                return StatusCode(500, new { Status = 0, message = "An unexpected error occurred while creating the book." });
            }
            catch (BookException ex)
            {
                // Return a simplified error message
                return StatusCode(500, new { Status = 0, message = "Book processing error: " + ex.Message });
            }
            catch (Exception ex)
            {
                // Return a simplified error message
                return StatusCode(500, new { Status = 0, message = "An unexpected error occurred while creating the book." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBooks()
        {
            try
            {
                var books = await _bookService.GetAllBooks();
                if (books != null && books.Count > 0)
                {
                    return Ok(new { Status = 1, Data = books, Message = "Books retrieved successfully." });
                }
                return NotFound(new { Status = 0, Message = "No Books found." });
            }
            catch (BookException ex)
            {
                return StatusCode(500, new { Status = 0, Message = "Book processing error: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 0, Message = "An unexpected error occurred while getting the Books." });
            }
        }

        [HttpGet("{bookId}")]
        public async Task<IActionResult> GetBookById(Guid bookId)
        {
            if (bookId == Guid.Empty) // Check if bookId is valid
            {
                return BadRequest(new { Status = 0, Message = "Invalid BookId." });
            }
            try
            {
                var result = await _bookService.GetBookById(bookId);
                if (result != null)
                {
                    return Ok(new { Status = 1, Data = result, Message = "Book retrieved successfully." });
                }
                return NotFound(new { Status = 0, Message = "Book not found." });
            }
            catch (BookException ex)
            {
                return StatusCode(500, new { Status = 0, Message = "Book processing error: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 0, Message = "An unexpected error occurred while getting the Book." });
            }
        }

        [HttpPut("{bookId}")]
        public async Task<IActionResult> UpdateBook(Guid bookId, [FromBody] BookData book)
        {
            if (bookId == Guid.Empty || book == null || bookId != book.BookId)
            {
                return BadRequest(new { Status = 0, Message = "Invalid request data." });
            }
            try
            {
                var result = await _bookService.UpdateBook(book, bookId);
                if (result)
                {
                    return Ok(new { Status = 1, Message = "Book updated successfully." });
                }
                return StatusCode(500, new { Status = 0, Message = "An unexpected error occurred while updating the Book." });
            }
            catch (BookException ex)
            {
                return StatusCode(500, new { Status = 0, Message = "Book processing error: " + ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 0, Message = "An unexpected error occurred while updating the book." });
            }
        }

        [HttpDelete("{bookId}")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            if (bookId == Guid.Empty) // Check if bookId is valid
            {
                return BadRequest(new { Status = 0, Message = "Invalid BookId." });
            }
            try
            {
                var result = await _bookService.DeleteBook(bookId);
                if (result)
                {
                    return NoContent();
                }
                return StatusCode(500, new { Status = 0, Message = "An unexpected error occurred while deleting the book." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = 0, Message = "An unexpected error occurred while deleting the book." });
            }
        }
    }
}
