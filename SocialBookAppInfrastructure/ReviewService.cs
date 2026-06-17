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

    public void EditReview(Review review, int id)
    {
        throw new NotImplementedException();
    }
}
