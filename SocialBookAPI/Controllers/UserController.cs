using Microsoft.AspNetCore.Mvc;
using SocialBookAppApplication;
using SocialBookAppDomain;

namespace SocialBookAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }


    /// <summary>
    /// Gets all users.
    /// </summary>
    /// <returns>A list of all users.</returns>
    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(_userService.GetAllUsers());
    }

    /// <summary>
    /// Gets a user by their username.
    /// </summary>
    /// <param name="username">The username of the user.</param>
    /// <returns>The user with the specified username.</returns>
    [HttpGet("username/{username}")]
    public IActionResult GetUserByUsername(string username)
    {
        try
        {
            var user = _userService.GetUserByUsername(username);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Gets a user by their ID.    
    /// </summary>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The user with the specified ID.</returns>
    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var user = _userService.GetUserById(id);
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="user">The user to create.</param>
    /// <returns>The created user.</returns>
    [HttpPost]
    public IActionResult CreateUser([FromBody] User user)
    {
        _userService.CreateUser(user);
        return Ok();
    }

    /// <summary>
    /// Updates the details of an existing user.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="user">The updated user details.</param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public IActionResult UpdateDetails(int id, [FromBody] User user)
    {
        try
        {
            _userService.UpdateDetails(user, id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Updates the favourite genres of a user.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="genres">The list of favourite genres to update.</param>
    /// <returns></returns>
    [HttpPut("{id}/genres")]
    public IActionResult UpdateFavouriteGenres(int id, [FromBody] List<Genre> genres)
    {
        try
        {
            _userService.UpdateFavouriteGenres(genres, id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Adds a language to a user's profile.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="language">The language to add to the user's profile.</param>
    /// <returns></returns>
    [HttpPut("{id}/language")]
    public IActionResult AddLanguage(int id, [FromBody] string language)
    {
        try
        {
            _userService.AddLanguage(language, id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a user by their ID.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        try
        {
            _userService.DeleteUser(id);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
