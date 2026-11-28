using TicTacToe.Models;
using TicTacToe.Models.Enums;
using TicTacToe.Repositories;

namespace TicTacToe.Services;

/// <summary>
/// Contains the game logics.
/// </summary>
internal class SnakeWaterGunService
{
    /// <summary>
    /// Represents the output for the win.
    /// </summary>
    public const int UserWins = 1;

    /// <summary>
    /// Represents the output for the computer win.
    /// </summary>
    public const int ComputerWins = -1;

    private readonly Random _random = new Random();
    private readonly RepositorySelector _repositorySelector;

    /// <summary>
    /// Initializes a new instance of the <see cref="SnakeWaterGunService"/> class.
    /// </summary>
    /// <param name="repositorySelector">The repositor selector instance.</param>
    public SnakeWaterGunService(RepositorySelector repositorySelector)
    {
        this._repositorySelector = repositorySelector;
    }

    /// <summary>
    /// Saves the game in repository.
    /// </summary>
    /// <param name="session">The user session</param>
    /// <param name="bestOf">The best of matches.</param>
    /// <param name="userScore">The score scored by the user.</param>
    /// <param name="computerScore">Scores gained by the computer.</param>
    public void SaveGame(UserSession session, int bestOf, int userScore, int computerScore)
    {
        string winner = userScore > computerScore ? session.UserName : "Computer";

        SnakeWaterGunGameRecord gameRecord = new SnakeWaterGunGameRecord()
        {
            BestOf = bestOf,
            PlayerOneName = session.UserName,
            PlayerTwoName = "Computer",
            PlayerOneScore = userScore,
            PlayerTwoScore = computerScore,
            WinnerName = winner,
            PlayedAt = DateTime.UtcNow,
            EndReason = EndReason.Normal,
        };

        this._repositorySelector.GetSnakeWaterGunRepository(session).AddGameRecord(gameRecord);
    }

    /// <summary>
    /// Gets the game history.
    /// </summary>
    /// <param name="session">The user session.</param>
    /// <returns>The list of the game records.</returns>
    public List<SnakeWaterGunGameRecord> GetGameHistory(UserSession session)
    {
        return this._repositorySelector.GetSnakeWaterGunRepository(session)
            .GetByUserName(session.UserName)
            .OrderByDescending(x => x.PlayedAt)
            .Take(100)
            .ToList();
    }

    /// <summary>
    /// Determines the winners in the game.
    /// </summary>
    /// <param name="userChoice">The choice selected by the user.</param>
    /// <param name="computerChoice">The choice selected by the computer.</param>
    /// <returns>The winner.</returns>
    public int DetermineWinner(SnakeWaterGunChoice userChoice, SnakeWaterGunChoice computerChoice)
    {
        if (userChoice == computerChoice)
        {
            return 0;
        }

        if ((userChoice.Equals(SnakeWaterGunChoice.Snake) && computerChoice.Equals(SnakeWaterGunChoice.Water)) ||
           (userChoice.Equals(SnakeWaterGunChoice.Water) && computerChoice.Equals(SnakeWaterGunChoice.Gun)) ||
           (userChoice.Equals(SnakeWaterGunChoice.Gun) && computerChoice.Equals(SnakeWaterGunChoice.Snake)))
        {
            return UserWins;
        }

        return ComputerWins;
    }

    /// <summary>
    /// Gets the computer choice randomly.
    /// </summary>
    /// <returns>The choice chosen by the computer.</returns>
    public SnakeWaterGunChoice GetComputerChoice()
    {
        int choice = this._random.Next(1, 4);
        return (SnakeWaterGunChoice)choice;
    }
}
