using TicTacToe.Models;

namespace TicTacToe.Repositories.Interfaces;

/// <summary>
/// Defines methods for adding and retrieving game records.
/// </summary>
/// <typeparam name="T">The type parameter for a game record.</typeparam>
internal interface IGameRepository<T>
    where T : GameRecord
{
    /// <summary>
    /// Adds a game record to the repository.
    /// </summary>
    /// <param name="gameRecord">The game record to be added.</param>
    void AddGameRecord(T gameRecord);

    /// <summary>
    /// Gets the list of games played by the user with the given username.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <returns>The list of games played by the user.</returns>
    List<T> GetByUserName(string userName);
}
