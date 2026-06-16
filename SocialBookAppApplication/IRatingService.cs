

using SocialBookAppDomain;

namespace SocialBookAppApplication;

public interface IRatingService
{
    public double GetRatingPercentage(List<Review> reviews);
}
