using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Stores the score in a json file to maintain persistence.
/// </summary>
internal class JsonScoreRepository : IScoreRepository
{
    private readonly List<Score> _scores;
    private readonly JsonFileManager _jsonFileManager;
    private readonly string _filePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonScoreRepository"/> class.
    /// </summary>
    /// <param name="jsonFileManager">The instance of the Json file manager</param>
    /// <param name="path">The path of the file to be stored.</param>
    public JsonScoreRepository(JsonFileManager jsonFileManager, string path)
    {
        this._jsonFileManager = jsonFileManager;
        this._filePath = path;
        this._scores = this._jsonFileManager.LoadAll<Score>(this._filePath);
    }

    /// <inheritdoc/>
    public List<Score> GetAll()
    {
        return this._scores;
    }

    /// <inheritdoc/>
    public void SaveScore(Score score)
    {
        int existingIndex = this._scores.FindIndex(s => s.Username == score.Username);

        if (existingIndex != -1)
        {
            this._scores[existingIndex] = score;
        }
        else
        {
            this._scores.Add(score);
        }

        this._jsonFileManager.WriteAll(this._filePath, this._scores);
    }

    /// <inheritdoc/>
    public Score GetScore(string username)
    {
        return this._scores.FirstOrDefault(s => s.Username == username) ?? new Score(username);
    }
}
