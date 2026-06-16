

namespace SocialBookAppDomain;

public class Review
{
    public int Id { get; set; }
    public User User { get; set; }
    public Book Book { get; set; }
    public double Rating { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedDate { get; set; }
}
