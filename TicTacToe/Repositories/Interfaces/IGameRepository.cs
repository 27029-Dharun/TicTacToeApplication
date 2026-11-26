using TicTacToe.Models;

namespace TicTacToe.Repositories.Interfaces;

/// <summary>
/// Defines methods for adding and retrieving game records.
/// </summary>
internal interface IGameRepository
{
    /// <summary>
    /// Adds a game record to the repository.
    /// </summary>
    /// <param name="gameRecord">The game record to be added.</param>
    void AddGameRecord(GameRecord gameRecord);

    /// <summary>
    /// Gets the list of games played by the user with the given username.
    /// </summary>
    /// <param name="userName">The username of the player.</param>
    /// <returns>The list of games played by the user.</returns>
    List<GameRecord> GetByUserName(string userName);
}
