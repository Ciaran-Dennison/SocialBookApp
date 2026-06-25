using Microsoft.AspNetCore.Mvc;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController : ControllerBase
{
    private readonly IAuthorService _authorService;
    public AuthorController(IAuthorService authorService)
    {
        _authorService = authorService;
    }
    /// <summary>
    /// Gets all authors.
    /// </summary>
    /// <returns>A list of all authors.</returns>
    [HttpGet]
    public IActionResult GetAllAuthors()
    {
        return Ok(_authorService.GetAllAuthors());
    }

    /// <summary>
    /// Gets an author by their ID.
    /// </summary>
    /// <param name="id">The ID of the author.</param>
    /// <returns>The author with the specified ID.</returns>
    [HttpGet("{id}")]
    public IActionResult GetAuthorById(int id)
    {
        try
        {
            var author = _authorService.GetAuthorById(id);
            return Ok(author);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets all books written by a specific author.
    /// </summary>
    /// <param name="id">The ID of the author.</param>
    /// <returns>A list of books written by the specified author.</returns>
    [HttpGet("{id}/books")]
    public IActionResult GetAuthorBooks(int id)
    {
        try
        {
            var books = _authorService.GetAuthorBooks(id);
            return Ok(books);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Adds a new author.
    /// </summary>
    /// <param name="author">The author to add.</param>
    /// <returns>The result of the add operation.</returns>
    [HttpPost]
    public IActionResult AddAuthor([FromBody] Author author)
    {
        _authorService.AddAuthor(author);
        return Ok();
    }

    /// <summary>
    /// Assigns a book to a specific author.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <param name="bookId">The ID of the book.</param>
    /// <returns>The result of the update operation.</returns>
    [HttpPut("{authorId}/book/{bookId}")]
    public IActionResult AssignBookToAuthor(int authorId, int bookId)
    {
        try
        {
            _authorService.AssignBookToAuthor(authorId, bookId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets the rating percentage of an author based on their reviews.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}/rating")]
    public IActionResult GetRatingPercentage(int id)
    {
        try
        {
            var author = _authorService.GetAuthorById(id);
            var ratingPercentage = _authorService.GetRatingPercentage(author.Reviews);
            return Ok(ratingPercentage);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Unassigns a book from a specific author.
    /// </summary>
    /// <param name="authorId">The ID of the author.</param>
    /// <param name="bookId">The ID of the book.</param>
    /// <returns>The result of the unassign operation.</returns>
    [HttpDelete("{authorId}/book/{bookId}")]
    public IActionResult UnassignBookFromAuthor(int authorId, int bookId)
    {
        try
        {
            _authorService.UnassignBookFromAuthor(authorId, bookId);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }


    /// <summary>
    /// Updates an existing author's information.
    /// </summary>
    /// <param name="id">The ID of the author to update.</param>
    /// <param name="author">The updated author information.</param>
    /// <returns>The result of the update operation.</returns>
    [HttpPut("{id}")]
    public IActionResult UpdateAuthor(int id, [FromBody] Author author)
    {
        try
        {
            _authorService.UpdateAuthor(id, author);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an author by their ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteAuthor(int id)
    {
        try
        {
            _authorService.DeleteAuthor(id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
