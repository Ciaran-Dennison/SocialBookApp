
namespace SocialBookAppDomain;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string> Languages { get; set; }
    public List<Genre> FavouriteGenres { get; set; }
    public PrivacyLevel PrivacyLevel { get; set; }
}
