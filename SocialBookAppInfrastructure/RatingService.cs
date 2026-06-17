using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppInfrastructure;

public class RatingService : IRatingService
{
    private readonly SocialBookAppContext _context;

    public RatingService(SocialBookAppContext context)
    {
        _context = context;
    }

    public double GetRatingPercentage(List<Review> reviews)
    {
        if (reviews == null || reviews.Count == 0)
        {
            return 0;
        }
        return reviews.Average(r => r.Rating);
    }
}
