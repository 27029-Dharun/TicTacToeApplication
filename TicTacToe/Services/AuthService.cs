using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Services;

/// <summary>
/// Contains the business logic for service layer.
/// </summary>
internal class AuthService
{
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthService"/> class.
    /// </summary>
    /// <param name="userRepository">The instance of the repository.</param>
    public AuthService(IUserRepository userRepository)
    {
        this._userRepository = userRepository;
    }

    /// <summary>
    /// Checks the unique username of the player, hashes the password and creates an instance.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="name">The name of the player.</param>
    /// <param name="password">The password entered by the user.</param>
    /// <param name="confirmPassword">Confirm password.</param>
    /// <param name="email">The email of the user.</param>
    /// <returns>A tuple with status and message returned from the method.</returns>
    public (bool success, string message) CreateUser(string userName, string name, string password, string confirmPassword, string email)
    {
        if (confirmPassword == password)
        {
            return (false, "Password didn't match");
        }

        if (this._userRepository.Exists(userName) || userName.Equals("Computer", StringComparison.OrdinalIgnoreCase) || userName.Equals("Guest", StringComparison.OrdinalIgnoreCase))
        {
            return (false, "Username already exists\n");
        }

        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        User user = new User(userName, name, email, hashedPassword, DateTime.UtcNow);

        this._userRepository.AddUser(user);
        return (true, "User created successfully\n");
    }

    /// <summary>
    /// Validates the credentials and creates a user session.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <param name="password">The password entered by the user.</param>
    /// <returns>The user session created.</returns>
    public UserSession? LogIn(string userName, string password)
    {
        User? user = this._userRepository.GetUser(userName);

        if (user == null)
        {
            return null;
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);

        if (!isValid)
        {
            return null;
        }

        return new UserSession(user.UserName, false);
    }

    /// <summary>
    /// Creates a guest session.
    /// </summary>
    /// <returns>A guest session created.</returns>
    public UserSession LogInGuest()
    {
        return new UserSession("Guest", true);
    }

    /// <summary>
    /// Edits the name of the user.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="session">The logged in user session.</param>
    /// <returns>True if the name changed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The argument null exception when username doesn't exists.</exception>
    internal bool EditName(string name, UserSession session)
    {
        User user = this._userRepository.GetUser(session.UserName) ?? throw new ArgumentNullException("Username not found");
        user.Name = name;

        this._userRepository.UpdateUser(user);
        return true;
    }

    /// <summary>
    /// Edit the password.
    /// </summary>
    /// <param name="oldPassword">The old password of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <param name="session">The session of the logged in user.</param>
    /// <returns>The boolean true if the password changed; otherwise, false.</returns>
    /// <exception cref="ArgumentNullException">The argument null exception.</exception>
    internal bool EditPassword(string oldPassword, string password, UserSession session)
    {
        User user = this._userRepository.GetUser(session.UserName) ?? throw new ArgumentNullException("Username not found");

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.HashedPassword);

        if (!isValid)
        {
            return false;
        }

        user.HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

        this._userRepository.UpdateUser(user);
        return true;
    }

    /// <summary>
    /// Gets the user details.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <returns>The user details of the username.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the username is not found.</exception>
    internal (string userName, string fullName, string email, DateTime joinDate) GetUserDetails(string userName)
    {
        User user = this._userRepository.GetUser(userName) ?? throw new KeyNotFoundException();

        return (user.UserName, user.Name, user.Email, user.JoinDate);
    }
}