using Microsoft.AspNetCore.Mvc;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }


    /// <summary>
    /// Gets the rating percentage of a book based on its reviews.
    /// </summary>
    /// <param name="id">The ID of the book.</param>
    /// <returns>The rating percentage of the book.</returns>
    [HttpGet("{id}/rating")]
    public IActionResult GetRatingPercentage(int id)
    {
        try
        {
            var book = _bookService.GetBookById(id);
            var ratingPercentage = _bookService.GetRatingPercentage(book.Reviews);
            return Ok(ratingPercentage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets all books.
    /// </summary>
    /// <returns>A list of all books.</returns>
    [HttpGet]
    public IActionResult GetAllBooks()
    {
        return Ok(_bookService.GetAllBooks());
    }


    /// <summary>
    /// Gets a book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book.</param>
    /// <returns>The book with the specified ID.</returns>
    [HttpGet("{id}")]
    public IActionResult GetBookById(int id)
    {
        try
        {
            var book = _bookService.GetBookById(id);
            return Ok(book);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets all books associated with a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of books associated with the specified user.</returns>
    [HttpGet("user/{userId}")]
    public IActionResult GetUserBooks(int userId)
    {
        try
        {
            var books = _bookService.GetUserBooks(userId);
            return Ok(books);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Adds a review for a specific book by a specific user.
    /// </summary>
    /// <param name="bookId">The ID of the book.</param>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="review">The review to be added.</param>
    /// <returns></returns>
    [HttpPost("{bookId}/review/{userId}")]
    public IActionResult AddReview(int bookId, int userId, [FromBody] Review review)
    {
        try
        {
            _bookService.AddReview(bookId, userId, review);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Adds a new book to the collection.
    /// </summary>
    /// <param name="book">The book to be added.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPost]
    public IActionResult AddBook([FromBody] Book book)
    {
        _bookService.AddBook(book);
        return Ok();
    }

    /// <summary>
    /// Deletes a book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to be deleted.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteBook(int id)
    {
        try
        {
            _bookService.DeleteBook(id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Updates a book by its ID.
    /// </summary>
    /// <param name="id">The ID of the book to update.</param>
    /// <param name="book">The updated book information.</param>
    /// <returns>An IActionResult indicating the result of the operation.</returns>
    [HttpPut("{id}")]
    public IActionResult UpdateBook(int id, [FromBody] Book book)
    {
        try
        {
            _bookService.UpdateBook(id, book);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
