

namespace SocialBookAppDomain;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string> Languages { get; set; }
    public List<Book> Books { get; set; }
    public List<Genre> Genres { get; set; }
    public List<Review> Reviews { get; set; }
}
