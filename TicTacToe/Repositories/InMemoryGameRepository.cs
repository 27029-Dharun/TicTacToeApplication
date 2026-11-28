using TicTacToe.Models;
using TicTacToe.Repositories.Interfaces;

namespace TicTacToe.Repositories;

/// <summary>
/// Stores the game record in in-memory.
/// </summary>
/// <typeparam name="T">The type parameter of the game.</typeparam>
internal class InMemoryGameRepository<T> : IGameRepository<T>
    where T : GameRecord
{
    private readonly List<T> _games = new List<T>();

    /// <inheritdoc/>
    public void AddGameRecord(T gameRecord)
    {
        this._games.Add(gameRecord);
    }

    /// <inheritdoc/>
    public List<T> GetByUserName(string userName)
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
