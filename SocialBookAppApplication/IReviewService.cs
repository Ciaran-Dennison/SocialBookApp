using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IReviewService
{
    public List<Review> GetAllReviews();
    public void EditReview(Review review, int id);
}
