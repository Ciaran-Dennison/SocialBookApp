

namespace SocialBookAppDomain;

public class Publisher
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public List<Book> Books { get; set; } = new List<Book>();
    public List<Book> BestSellers { get; set; } = new List<Book>();
}