using TicTacToe.Models;

namespace TicTacToe.Repositories.Interfaces;

/// <summary>
/// Contains the contract to save the score.
/// </summary>
internal interface IScoreRepository
{
    /// <summary>
    /// Updates the score to the repository.
    /// </summary>
    /// <param name="score">The score to be replaced.</param>
    void SaveScore(Score score);

    /// <summary>
    /// Gets the score by username.
    /// </summary>
    /// <param name="username">The username of the player to get the score.</param>
    /// <returns>The score record of the player.</returns>
    Score GetScore(string username);

    /// <summary>
    /// Gets all users score.
    /// </summary>
    /// <returns>List of the scores.</returns>
    List<Score> GetAll();
}
