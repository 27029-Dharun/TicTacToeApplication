using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Provides persistent storage and retrieval of game records using a JSON file.
/// </summary>
/// <typeparam name="T">The type parameter representing the game record.</typeparam>
internal class JsonGameRepository<T> : IGameRepository<T>
    where T : GameRecord
{
    private readonly List<T> _games;
    private readonly JsonFileManager _jsonFileManager;
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonGameRepository{T}"/> class.
    /// </summary>
    /// <param name="jsonFileManager">The instance of the Json file manager</param>
    /// <param name="path">The path of the file to be stored.</param>
    public JsonGameRepository(JsonFileManager jsonFileManager, string path)
    {
        this._jsonFileManager = jsonFileManager;
        this._filePath = path;
        this._games = this._jsonFileManager.LoadAll<T>(this._filePath);
    }

    /// <inheritdoc/>
    public void AddGameRecord(T gameRecord)
    {
        this._games.Add(gameRecord);
        this._jsonFileManager.WriteAll(this._filePath, this._games);
    }

    /// <inheritdoc/>
    public List<T> GetByUserName(string userName)
    {
        return this._games
            .Where(x => x.PlayerOneName == userName || x.PlayerTwoName == userName)
            .ToList();
    }
}
