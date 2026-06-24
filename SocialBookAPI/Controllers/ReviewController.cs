using Microsoft.AspNetCore.Mvc;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }
    /// <summary>
    /// Gets all reviews.
    /// </summary>
    /// <returns>A list of all reviews.</returns>
    [HttpGet]
    public IActionResult GetAllReviews()
    {
        return Ok(_reviewService.GetAllReviews());
    }
    /// <summary>
    /// Edits a review by its ID.
    /// </summary>
    /// <param name="id">The ID of the review to edit.</param>
    /// <param name="review">The updated review data.</param>
    /// <returns>No content if successful, or NotFound if the review does not exist.</returns>
    [HttpPut("{id}")]
    public IActionResult EditReview(int id, [FromBody] Review review)
    {
        try
        {
            _reviewService.EditReview(review, id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
