using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Stores the score in in-memory.
/// </summary>
internal class InMemoryScoreRepository : IScoreRepository
{
    private readonly List<Score> _scores = new List<Score>();

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
    }

    /// <inheritdoc/>
    public Score GetScore(string username)
    {
        return this._scores.FirstOrDefault(s => s.Username == username) ?? new Score(username, 0, 0, 0, 0);
    }

    /// <summary>
    /// Clears the record from the in-memory list.
    /// </summary>
    internal void Remove()
    {
        this._scores.Clear();
    }
}
