using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Provides persistent storage and retrieval of game records using a JSON file.
/// </summary>
internal class JsonGameRepository : IGameRepository
{
    private readonly List<GameRecord> _games;
    private readonly JsonFileManager _jsonFileManager;
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonGameRepository"/> class.
    /// </summary>
    /// <param name="jsonFileManager">The instance of the Json file manager</param>
    /// <param name="path">The path of the file to be stored.</param>
    public JsonGameRepository(JsonFileManager jsonFileManager, string path)
    {
        this._jsonFileManager = jsonFileManager;
        this._filePath = path;
        this._games = this._jsonFileManager.LoadAll<GameRecord>(this._filePath);
    }

    /// <inheritdoc/>
    public void AddGameRecord(GameRecord gameRecord)
    {
        this._games.Add(gameRecord);
        this._jsonFileManager.WriteAll(this._filePath, this._games);
    }

    /// <inheritdoc/>
    public List<GameRecord> GetByUserName(string userName)
    {
        return this._games.Where(x => x.PlayerOneName == userName || x.PlayerTwoName == userName).ToList();
    }
}
