
namespace SocialBookAppDomain;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int ISBNNumber { get; set; }
    public List<Author> Authors { get; set; } = new List<Author>();
    public List<Publisher> Publishers { get; set; } = new List<Publisher>();
    public DateTime Published { get; set; }
    public Genre Genre { get; set; }
    public string Language { get; set; }
    public BookFormat Format { get; set; }
    public bool IsChildFriendly { get; set; }
    public int Pages { get; set; }
    public int Chapters { get; set; }
    public string Blurb { get; set; }
    public List<Review> Reviews { get; set; } = new List<Review>();
}
