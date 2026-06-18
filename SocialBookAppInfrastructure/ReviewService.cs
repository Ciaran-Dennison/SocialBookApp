using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class ReviewService : IReviewService
{
    private readonly SocialBookAppContext _context;

    public ReviewService(SocialBookAppContext context)
    {
        _context = context;
    }

    public List<Review> GetAllReviews()
    {
        return _context.Reviews.ToList();
    }

    public void EditReview(Review review, int id)
    {
        var existingReview = _context.Reviews.FirstOrDefault(r => r.Id == id);
        if (existingReview == null)
        {
            throw new ArgumentException("Review not found.");
        }
        existingReview.Book = review.Book;
        existingReview.Rating = review.Rating;
        existingReview.Comment = review.Comment;
        _context.SaveChanges();
    }
}
