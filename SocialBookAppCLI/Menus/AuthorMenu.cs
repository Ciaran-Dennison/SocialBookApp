using InquirerCore;
using InquirerCore.Prompts;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppCLI.Menus;

public class AuthorMenu
{
    private readonly IAuthorService _authorService;

    public AuthorMenu(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            var authorOptions = new string[] { "Get Author", "Get Author Books", "Get All Authors", "Add Author", "Assign Book to Author", "Back" };
            var authorMenu = new ListInput("authorMenu", "Author Management", authorOptions);
            new Inquirer(authorMenu).Ask();

            switch (authorMenu.Answer())
            {
                case "1":
                    var getIdInput = new Input("id", "Enter author ID:");
                    new Inquirer(getIdInput).Ask();

                    if (int.TryParse(getIdInput.Answer(), out int authorId))
                    {
                        try
                        {
                            var author = _authorService.GetAuthorById(authorId);
                            Console.WriteLine($"ID: {author.Id}");
                            Console.WriteLine($"Name: {author.FirstName} {author.LastName}");
                            Console.WriteLine($"Date of Birth: {author.DateOfBirth.ToShortDateString()}");
                            Console.WriteLine($"Email: {author.Email}");
                            Console.WriteLine($"Languages: {string.Join(", ", author.Languages)}");
                            Console.WriteLine($"Genres: {string.Join(", ", author.Genres)}");
                            Console.WriteLine($"Overall Rating: {_authorService.GetRatingPercentage(author.Reviews)}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    break;

                case "2":
                    var booksIdInput = new Input("id", "Enter author ID:");
                    new Inquirer(booksIdInput).Ask();

                    if (int.TryParse(booksIdInput.Answer(), out int booksAuthorId))
                    {
                        try
                        {
                            var books = _authorService.GetAuthorBooks(booksAuthorId);
                            if (books.Count == 0)
                            {
                                Console.WriteLine("No books found for this author.");
                            }
                            else
                            {
                                foreach (var book in books)
                                {
                                    Console.WriteLine($"ID: {book.Id} | Title: {book.Title} | Genre: {book.Genre} | Format: {book.Format}");
                                }
                            }
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    break;

                case "3":
                    try
                    {
                        var authors = _authorService.GetAllAuthors();
                        if (authors.Count == 0)
                        {
                            Console.WriteLine("No authors found.");
                        }
                        else
                        {
                            foreach (var author in authors)
                            {
                                Console.WriteLine($"ID: {author.Id} | Name: {author.FirstName} {author.LastName} | Email: {author.Email}");
                            }
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    break;

                case "4":
                    var firstNameInput = new Input("firstName", "Enter first name:");
                    var lastNameInput = new Input("lastName", "Enter last name:");
                    var emailInput = new Input("email", "Enter email:");
                    var passwordInput = new Input("password", "Enter password:");
                    var languageInput = new Input("language", "Enter language:");
                    var genreInput = new ListInput("genre", "Select genre:", Enum.GetNames(typeof(Genre)));

                    new Inquirer(firstNameInput, lastNameInput, emailInput, passwordInput, languageInput, genreInput).Ask();

                    DateTime dob = DateTime.MinValue;
                    bool validDob = false;
                    while (!validDob)
                    {
                        var dobInput = new Input("dob", "Enter date of birth (dd/MM/yyyy):");
                        new Inquirer(dobInput).Ask();
                        if (DateTime.TryParseExact(dobInput.Answer(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out dob))
                            validDob = true;
                        else
                            Console.WriteLine("Invalid date format. Please use dd/MM/yyyy.");
                    }

                    var newAuthor = new Author
                    {
                        FirstName = firstNameInput.Answer(),
                        LastName = lastNameInput.Answer(),
                        Email = emailInput.Answer(),
                        Password = passwordInput.Answer(),
                        Languages = new List<string> { languageInput.Answer() },
                        Genres = new List<Genre> { (Genre)(int.Parse(genreInput.Answer()) - 1) },
                        DateOfBirth = dob,
                        CreatedDate = DateTime.Now
                    };

                    try
                    {
                        _authorService.AddAuthor(newAuthor);
                        Console.WriteLine("Author added successfully!");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                    }
                    break;

                case "5":
                    var assignAuthorIdInput = new Input("authorId", "Enter author ID:");
                    var assignBookIdInput = new Input("bookId", "Enter book ID:");
                    new Inquirer(assignAuthorIdInput, assignBookIdInput).Ask();

                    if (int.TryParse(assignAuthorIdInput.Answer(), out int assignAuthorId) &&
                        int.TryParse(assignBookIdInput.Answer(), out int assignBookId))
                    {
                        try
                        {
                            _authorService.AssignBookToAuthor(assignAuthorId, assignBookId);
                            Console.WriteLine("Book assigned to author successfully!");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine($"Error: {ex.Message}");
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey();
                        }
                    }
                    break;

                case "6":
                    return;
            }
        }
    }
}