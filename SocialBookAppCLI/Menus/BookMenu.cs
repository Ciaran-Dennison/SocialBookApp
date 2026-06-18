using InquirerCore;
using InquirerCore.Prompts;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAppCLI.Menus;

public class BookMenu
{
    private readonly IBookService _bookService;

    public BookMenu(IBookService bookService)
    {
        _bookService = bookService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            var bookOptions = new string[] { "Add Book", "Delete Book", "Get User Books", "Add Review", "List All Books", "Back" };
            var bookMenu = new ListInput("bookMenu", "Book Management", bookOptions);
            new Inquirer(bookMenu).Ask();

            switch (bookMenu.Answer())
            {
                case "1":
                    var titleInput = new Input("title", "Enter title:");
                    var isbnInput = new Input("isbn", "Enter ISBN number:");
                    var languageInput = new Input("language", "Enter language:");
                    var pagesInput = new Input("pages", "Enter number of pages:");
                    var chaptersInput = new Input("chapters", "Enter number of chapters:");
                    var blurbInput = new Input("blurb", "Enter blurb:");
                    var genreInput = new ListInput("genre", "Select genre:", Enum.GetNames(typeof(Genre)));
                    var formatInput = new ListInput("format", "Select format:", Enum.GetNames(typeof(BookFormat)));
                    var childFriendlyInput = new InputConfirmation("childFriendly", "Is it child friendly?");

                    new Inquirer(titleInput, isbnInput, languageInput, pagesInput, chaptersInput, blurbInput, genreInput, formatInput, childFriendlyInput).Ask();

                    DateTime published = DateTime.MinValue;
                    bool validDate = false;
                    while (!validDate)
                    {
                        var publishedInput = new Input("published", "Enter published date (dd/MM/yyyy):");
                        new Inquirer(publishedInput).Ask();
                        if (DateTime.TryParseExact(publishedInput.Answer(), "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out published))
                            validDate = true;
                        else
                            Console.WriteLine("Invalid date format. Please use dd/MM/yyyy.");
                    }

                    if (!int.TryParse(isbnInput.Answer(), out int isbn))
                    {
                        Console.WriteLine("Invalid ISBN.");
                        Console.ReadKey();
                        break;
                    }

                    if (!int.TryParse(pagesInput.Answer(), out int pages))
                    {
                        Console.WriteLine("Invalid page count.");
                        Console.ReadKey();
                        break;
                    }

                    if (!int.TryParse(chaptersInput.Answer(), out int chapters))
                    {
                        Console.WriteLine("Invalid chapter count.");
                        Console.ReadKey();
                        break;
                    }

                    var newBook = new Book
                    {
                        Title = titleInput.Answer(),
                        ISBNNumber = isbn,
                        Language = languageInput.Answer(),
                        Pages = pages,
                        Chapters = chapters,
                        Blurb = blurbInput.Answer(),
                        Genre = (Genre)(int.Parse(genreInput.Answer()) - 1),
                        Format = (BookFormat)(int.Parse(formatInput.Answer()) - 1),
                        IsChildFriendly = childFriendlyInput.Answer() == "y",
                        Published = published
                    };

                    try
                    {
                        _bookService.AddBook(newBook);
                        Console.WriteLine("Book added successfully!");
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

                case "2":
                    var deleteIdInput = new Input("id", "Enter book ID to delete:");
                    var confirmDelete = new InputConfirmation("confirm", "Are you sure you want to delete this book?");
                    new Inquirer(deleteIdInput, confirmDelete).Ask();

                    if (confirmDelete.Answer() == "y")
                    {
                        if (int.TryParse(deleteIdInput.Answer(), out int deleteId))
                        {
                            try
                            {
                                _bookService.DeleteBook(deleteId);
                                Console.WriteLine("Book deleted successfully!");
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
                    }
                    break;

                case "3":
                    var userIdInput = new Input("userId", "Enter user ID:");
                    new Inquirer(userIdInput).Ask();

                    if (int.TryParse(userIdInput.Answer(), out int userId))
                    {
                        try
                        {
                            var books = _bookService.GetUserBooks(userId);
                            if (books.Count == 0)
                            {
                                Console.WriteLine("No books found for this user.");
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

                case "4":
                    var reviewBookIdInput = new Input("bookId", "Enter book ID:");
                    var reviewUserIdInput = new Input("userId", "Enter user ID:");
                    var ratingInput = new Input("rating", "Enter rating (1-5):");
                    var commentInput = new Input("comment", "Enter comment:");
                    new Inquirer(reviewBookIdInput, reviewUserIdInput, ratingInput, commentInput).Ask();

                    if (int.TryParse(reviewBookIdInput.Answer(), out int reviewBookId) &&
                        int.TryParse(reviewUserIdInput.Answer(), out int reviewUserId))
                    {
                        try
                        {
                            var reviewer = new Review
                            {
                                Rating = double.Parse(ratingInput.Answer()),
                                Comment = commentInput.Answer(),
                                CreatedDate = DateTime.Now
                            };

                            _bookService.AddReview(reviewBookId, reviewUserId, reviewer);
                            Console.WriteLine("Review added successfully!");
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

                case "5":
                    var allBooks = _bookService.GetAllBooks();
                    if (allBooks.Count == 0)
                    {
                        Console.WriteLine("No books found.");
                    }
                    else
                    {
                        foreach (var book in allBooks)
                        {
                            Console.WriteLine($"ID: {book.Id} | Title: {book.Title} | Genre: {book.Genre} | Format: {book.Format}");
                        }
                    }
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                    break;

                case "6":
                    return;
            }
        }
    }
}
