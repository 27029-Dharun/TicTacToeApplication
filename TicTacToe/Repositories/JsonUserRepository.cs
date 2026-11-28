using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Provides a JSON file based implementation of <see cref="IUserRepository"/> for persisting and retrieving user accounts.
/// </summary>
internal class JsonUserRepository : IUserRepository
{
    private readonly List<User> _users;
    private readonly JsonFileManager _jsonFileManager;
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonUserRepository"/> class.
    /// </summary>
    /// <param name="jsonFileManager">The instance of the Json file manager</param>
    /// <param name="path">The path of the file to be stored.</param>
    public JsonUserRepository(JsonFileManager jsonFileManager, string path)
    {
        this._jsonFileManager = jsonFileManager;
        this._filePath = path;
        this._users = this._jsonFileManager.LoadAll<User>(this._filePath);
    }

    /// <inheritdoc/>
    public void AddUser(User user)
    {
        this._users.Add(user);
        this._jsonFileManager.WriteAll(this._filePath, this._users);
    }

    /// <inheritdoc/>
    public User? GetUser(string userName)
    {
        return this._users.FirstOrDefault(user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public bool Exists(string userName)
    {
        return this._users.Any(user => user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
    }

    /// <inheritdoc/>
    public bool UpdateUser(User user)
    {
        int index = this._users.FindIndex(existing => existing.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase));

        if (index == -1)
        {
            return false;
        }

        this._users[index] = user;
        this._jsonFileManager.WriteAll(this._filePath, this._users);
        return true;
    }
}
