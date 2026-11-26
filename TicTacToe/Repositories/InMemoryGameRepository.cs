using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Stores the game record in in-memory.
/// </summary>
internal class InMemoryGameRepository : IGameRepository
{
    private readonly List<GameRecord> _games = new List<GameRecord>();

    /// <inheritdoc/>
    public void AddGameRecord(GameRecord gameRecord)
    {
        this._games.Add(gameRecord);
    }

    /// <inheritdoc/>
    public List<GameRecord> GetByUserName(string userName)
    {
        return this._games.Where(x => x.PlayerOneName == userName || x.PlayerTwoName == userName).ToList();
    }

    /// <summary>
    /// Clears the game record from the in-memory list.
    /// </summary>
    internal void Remove()
    {
        this._games.Clear();
    }
}
